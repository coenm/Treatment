namespace Treatment.Console.CrossCuttingConcerns
{
    using JetBrains.Annotations;

    using Treatment.Console.Console;

    [UsedImplicitly]
    public class HoldConsole : IHoldConsole
    {
        [NotNull]
        private readonly IConsole _console;

        public HoldConsole([NotNull] IConsole console)
        {
            _console = console;
        }

        public void Hold()
        {
            _console.WriteLine();
            _console.WriteLine("Press enter to exit");
            _console.ReadLine();
        }
    }
}