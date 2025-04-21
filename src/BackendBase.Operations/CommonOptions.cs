using CommandLine;

namespace BackendBase.Operations;
public sealed class CommonOptions
{
    [Option('e', "environment", Required = true)]
    public string Environment { get; set; } = Environments.Local;

    public static CommonOptions ParseFromCommandLineArgs(string[] args)
    {
        var environment = Environments.Local;

        var indexOfDashArg = Array.FindIndex(args, arg => arg == "-e");
        if (indexOfDashArg > -1)
        {
            environment = args[indexOfDashArg + 1];
        }

        var indexOfDashDashArg = Array.FindIndex(args, arg => arg == "--environment");
        if (indexOfDashDashArg > -1)
        {
            environment = args[indexOfDashDashArg + 1];
        }

        return new CommonOptions
        {
            Environment = environment
        };
    }
}
