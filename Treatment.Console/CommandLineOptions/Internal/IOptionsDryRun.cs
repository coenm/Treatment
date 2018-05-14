namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionsDryRun
    {
        [Option('n', "dry-run", Default = false, Required = false, HelpText = "File changes are not written to disk, only listed in the console.")]
        bool DryRun { get; [UsedImplicitly] set; }
    }
}