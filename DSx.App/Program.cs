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
    Console.Error.WriteLine($"Error: {e.Message}");
    return 1;
}
return 0;