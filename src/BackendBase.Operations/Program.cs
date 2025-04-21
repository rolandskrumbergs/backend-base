using BackendBase.Operations;
using Microsoft.Extensions.DependencyInjection;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

using var host = OperationsHost.Create(CommonOptions.ParseFromCommandLineArgs(args));

await host.StartAsync();

using var scope = host.Services.CreateScope();

//var result = await Parser.Default
//    .ParseArguments(args)
//    .MapResult();

await host.StopAsync();

//return result;
