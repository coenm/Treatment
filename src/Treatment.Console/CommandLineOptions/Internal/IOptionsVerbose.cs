namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionsVerbose
    {
        [Option('v', "verbose", Default = 0, Required = false, HelpText = "Verbosity level ranging from 0 (disabled) to 3 (max).")]
        int Verbose { get; [UsedImplicitly] set; }
    }
}