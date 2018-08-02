namespace Treatment.Plugin.Everything
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>Wrapper for Everything.
    /// <see cref="https://www.voidtools.com/support/everything/sdk/csharp/"/>
    /// </summary>
    internal static class Everything64Api
    {
        private const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        private const int EVERYTHING_REQUEST_PATH = 0x00000002;

        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_SetSearch(string lpSearchString);

        [DllImport("Everything64.dll")]
        public static extern void Everything_SetMatchCase(bool bEnable);

        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        public static extern bool Everything_Query(bool bWait);

        [DllImport("Everything64.dll")]
        public static extern uint Everything_GetNumResults();

        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathName(uint nIndex, StringBuilder lpString, uint nMaxCount);

        [DllImport("Everything64.dll")]
        public static extern void Everything_CleanUp();

        [DllImport("Everything64.dll")]
        public static extern uint Everything_GetMajorVersion();

        // Everything 1.4
        [DllImport("Everything64.dll")]
        public static extern void Everything_SetRequestFlags(uint dwRequestFlags);

        public static IEnumerable<string> Search(string query)
        {
            try
            {
                const int BUFSIZE = 260;

                Everything_SetSearch(query);
                Everything_SetRequestFlags(EVERYTHING_REQUEST_FILE_NAME | EVERYTHING_REQUEST_PATH);
                Everything_SetMatchCase(false);

                if (!Everything_Query(true))
                    return Enumerable.Empty<string>();

                var nrResults = Everything_GetNumResults();

                if (nrResults == 0)
                    return Enumerable.Empty<string>();

                var buf = new StringBuilder(BUFSIZE);
                var result = new List<string>((int)nrResults);

                for (uint i = 0; i < nrResults; i++)
                {
                    buf.Clear();
                    Everything_GetResultFullPathName(i, buf, BUFSIZE);
                    result.Add(buf.ToString());
                }

                return result;
            }
            finally
            {
                Ignore(Everything_CleanUp);
            }
        }

        public static bool IsInstalled()
        {
            try
            {
                var _ = Everything_GetMajorVersion();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void Ignore(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {
                // intentionally do nothing
            }
        }
    }
}
