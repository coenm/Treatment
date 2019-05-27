namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionsHoldOnExit
    {
        [Option('h', "hold", Default = false, Required = false, HelpText = "Keeps console open when process successfully finished.")]
        bool HoldOnExit { get; [UsedImplicitly] set; }
    }
}