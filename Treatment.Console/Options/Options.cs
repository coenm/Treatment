namespace Treatment.Console.Options
{
    using CommandLine;

    using JetBrains.Annotations;

    public class Options
    {
        [Option('h', "hold", Default = false, Required = false, HelpText = "Keeps console open when process successfully finished.")]
        public bool HoldOnExit { get; [UsedImplicitly] set; }
    }
}