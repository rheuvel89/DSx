using CommandLine;
using DSx.Collector;
using DSx.Host;
using DSx.Receiver;
using DXs.Common;

var parserResult = Parser.Default.ParseArguments<HostOptions, CollectorOptions, ReceiverOptions>(args);
try
{
    await (parserResult.MapResult<HostOptions, CollectorOptions, ReceiverOptions, IApplication?>(
        opts =>
        {
            if (opts.PluginPath == null) return new Host(opts);
            Console.Clear();
            Console.Write($"Loading converter plugins from external dll files can pose a security risk. Load only files that you know the source of and trust.{Environment.NewLine}If you understand and accept the risks press 'Y': ");
            var c = Console.ReadKey();
            Console.WriteLine();
            return c.KeyChar switch
            {
                'y' or 'Y' => new Host(opts),
                _ => null
            };
        },
        opts => new Collector(opts),
        opts => new Receiver(opts),
        _ => null
    )?.Start() ?? Task.FromResult(1));

}
catch (Exception e)
{
    var messages = GetExeptions(e);
    Console.Error.WriteLine($"Error: {Environment.NewLine}{string.Join(Environment.NewLine, messages)}");
    return 1;
}
return 0;

static IList<string> GetExeptions(Exception e, IList<string>? list = null)
{
    list ??= new List<string>();
    list.Add(e.Message + Environment.NewLine + e.StackTrace);
    if (e is AggregateException a) foreach (var ie in a.InnerExceptions) GetExeptions(ie, list);
    else if (e.InnerException != null) GetExeptions(e.InnerException, list);
    return list;
}