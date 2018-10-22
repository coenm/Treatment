namespace Treatment.Console.Tests.E2ETests.Fakes
{
    using System;
    using System.Text;

    using Treatment.Console.Console;

    internal class FakeConsoleAdapter : IConsole
    {
        private readonly StringBuilder text;

        public FakeConsoleAdapter()
        {
            text = new StringBuilder();
        }

        public void WriteLine()
        {
            text.AppendLine();
        }

        public void WriteLine(string value)
        {
            text.AppendLine(value);
        }

        public int Read()
        {
            text.AppendLine("! Read !");
            return 0;
        }

        public ConsoleKeyInfo ReadKey()
        {
            text.AppendLine("! ReadKey !");
            return new ConsoleKeyInfo();
        }

        public string ReadLine()
        {
            text.AppendLine("! ReadLine !");
            return string.Empty;
        }

        public override string ToString()
        {
            return text.ToString();
        }
    }
}
