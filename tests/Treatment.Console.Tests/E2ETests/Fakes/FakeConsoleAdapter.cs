namespace Treatment.Console.Tests.E2ETests.Fakes
{
    using System;
    using System.IO;
    using System.Text;

    using Treatment.Console.Console;

    internal class FakeConsoleAdapter : IConsole
    {
        private readonly StringBuilder _text;

        public FakeConsoleAdapter()
        {
            _text = new StringBuilder();
        }

        public void WriteLine()
        {
            _text.AppendLine();
        }

        public void WriteLine(string value)
        {
            _text.AppendLine(value);
        }

        public int Read()
        {
            _text.AppendLine("! Read !");
            return 0;
        }

        public ConsoleKeyInfo ReadKey()
        {
            _text.AppendLine("! ReadKey !");
            return new ConsoleKeyInfo();
        }

        public string ReadLine()
        {
            _text.AppendLine("! ReadLine !");
            return string.Empty;
        }

        public override string ToString()
        {
            return _text.ToString();
        }
    }
}