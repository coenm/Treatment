namespace Treatment.Console.Console
{
    using System;

    public class ConsoleAdapter : IConsole
    {
        private ConsoleAdapter()
        {
        }

        public static ConsoleAdapter Instance { get; } = new ConsoleAdapter();

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public int Read()
        {
            return Console.Read();
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}