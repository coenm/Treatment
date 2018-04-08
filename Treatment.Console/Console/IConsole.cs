namespace Treatment.Console.Console
{
    using System;

    /// <summary>
    /// Abstraction of System.Console. Created only for unittest purposes.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Writes the current line terminator to the standard output stream.
        /// </summary>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurred.
        /// </exception>
        void WriteLine();

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurred.
        /// </exception>
        void WriteLine(string value);

        /// <summary>
        /// Reads the next character from the standard input stream.
        /// </summary>
        /// <returns>
        /// The next character from the input stream, or negative one (<c>-1</c>) if there are
        /// currently no more characters to be read.
        /// </returns>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurred.
        /// </exception>
        int Read();

        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed
        /// key is displayed in the console window.
        /// </summary>
        /// <returns>
        /// A <c>System.ConsoleKeyInfo</c> object that describes the <c>System.ConsoleKey</c>
        /// constant and Unicode character, if any, that correspond to the pressed console key.
        /// The <c>System.ConsoleKeyInfo</c> object also describes, in a bitwise combination
        /// of <c>System.ConsoleModifiers</c> values, whether one or more Shift, Alt, or Ctrl
        /// modifier keys was pressed simultaneously with the console key.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <c>Custom.ConsoleWrapper.In</c> property is redirected from some
        /// stream other than the console.
        /// </exception>
        ConsoleKeyInfo ReadKey();

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>
        /// The next line of characters from the input stream, or <c>null</c> if no more lines
        /// are available.
        /// </returns>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurred.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        /// There is insufficient memory to allocate a buffer for the returned string.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The number of characters in the next line of characters is greater than
        /// <c>System.Int32.MaxValue</c>.
        /// </exception>
        string ReadLine();
    }
}