using CommandLine;
using DSx.Client;
using DSx.Host;
using DXs.Common;

var parserResult = Parser.Default.ParseArguments<ClientOptions, HostOptions>(args);
try
{
    await (parserResult.MapResult<ClientOptions, HostOptions, IApplication?>(
        opts => new Client(opts),
        opts => new Host(opts),
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
    list.Add(e.Message);
    if (e is AggregateException a) foreach (var ie in a.InnerExceptions) GetExeptions(ie, list);
    else if (e.InnerException != null) GetExeptions(e.InnerException, list);
    return list;
}