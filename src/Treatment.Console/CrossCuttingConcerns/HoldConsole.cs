namespace Treatment.Console.CrossCuttingConcerns
{
    using JetBrains.Annotations;

    using Treatment.Console.Console;

    [UsedImplicitly]
    public class HoldConsole : IHoldConsole
    {
        [NotNull]
        private readonly IConsole console;

        public HoldConsole([NotNull] IConsole console)
        {
            this.console = console;
        }

        public void Hold()
        {
            console.WriteLine();
            console.WriteLine("Press enter to exit");
            console.ReadLine();
        }
    }
}
