namespace Treatment.Plugin.Everything
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class Everything32Api
    {
        private const int EVERYTHING_OK = 0;
        private const int EVERYTHING_ERROR_MEMORY = 1;
        private const int EVERYTHING_ERROR_IPC = 2;
        private const int EVERYTHING_ERROR_REGISTERCLASSEX = 3;
        private const int EVERYTHING_ERROR_CREATEWINDOW = 4;
        private const int EVERYTHING_ERROR_CREATETHREAD = 5;
        private const int EVERYTHING_ERROR_INVALIDINDEX = 6;
        private const int EVERYTHING_ERROR_INVALIDCALL = 7;

        private const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        private const int EVERYTHING_REQUEST_PATH = 0x00000002;
        private const int EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME = 0x00000004;
        private const int EVERYTHING_REQUEST_EXTENSION = 0x00000008;
        private const int EVERYTHING_REQUEST_SIZE = 0x00000010;
        private const int EVERYTHING_REQUEST_DATE_CREATED = 0x00000020;
        private const int EVERYTHING_REQUEST_DATE_MODIFIED = 0x00000040;
        private const int EVERYTHING_REQUEST_DATE_ACCESSED = 0x00000080;
        private const int EVERYTHING_REQUEST_ATTRIBUTES = 0x00000100;
        private const int EVERYTHING_REQUEST_FILE_LIST_FILE_NAME = 0x00000200;
        private const int EVERYTHING_REQUEST_RUN_COUNT = 0x00000400;
        private const int EVERYTHING_REQUEST_DATE_RUN = 0x00000800;
        private const int EVERYTHING_REQUEST_DATE_RECENTLY_CHANGED = 0x00001000;
        private const int EVERYTHING_REQUEST_HIGHLIGHTED_FILE_NAME = 0x00002000;
        private const int EVERYTHING_REQUEST_HIGHLIGHTED_PATH = 0x00004000;
        private const int EVERYTHING_REQUEST_HIGHLIGHTED_FULL_PATH_AND_FILE_NAME = 0x00008000;

        private const int EVERYTHING_SORT_NAME_ASCENDING = 1;
        private const int EVERYTHING_SORT_NAME_DESCENDING = 2;
        private const int EVERYTHING_SORT_PATH_ASCENDING = 3;
        private const int EVERYTHING_SORT_PATH_DESCENDING = 4;
        private const int EVERYTHING_SORT_SIZE_ASCENDING = 5;
        private const int EVERYTHING_SORT_SIZE_DESCENDING = 6;
        private const int EVERYTHING_SORT_EXTENSION_ASCENDING = 7;
        private const int EVERYTHING_SORT_EXTENSION_DESCENDING = 8;
        private const int EVERYTHING_SORT_TYPE_NAME_ASCENDING = 9;
        private const int EVERYTHING_SORT_TYPE_NAME_DESCENDING = 10;
        private const int EVERYTHING_SORT_DATE_CREATED_ASCENDING = 11;
        private const int EVERYTHING_SORT_DATE_CREATED_DESCENDING = 12;
        private const int EVERYTHING_SORT_DATE_MODIFIED_ASCENDING = 13;
        private const int EVERYTHING_SORT_DATE_MODIFIED_DESCENDING = 14;
        private const int EVERYTHING_SORT_ATTRIBUTES_ASCENDING = 15;
        private const int EVERYTHING_SORT_ATTRIBUTES_DESCENDING = 16;
        private const int EVERYTHING_SORT_FILE_LIST_FILENAME_ASCENDING = 17;
        private const int EVERYTHING_SORT_FILE_LIST_FILENAME_DESCENDING = 18;
        private const int EVERYTHING_SORT_RUN_COUNT_ASCENDING = 19;
        private const int EVERYTHING_SORT_RUN_COUNT_DESCENDING = 20;
        private const int EVERYTHING_SORT_DATE_RECENTLY_CHANGED_ASCENDING = 21;
        private const int EVERYTHING_SORT_DATE_RECENTLY_CHANGED_DESCENDING = 22;
        private const int EVERYTHING_SORT_DATE_ACCESSED_ASCENDING = 23;
        private const int EVERYTHING_SORT_DATE_ACCESSED_DESCENDING = 24;
        private const int EVERYTHING_SORT_DATE_RUN_ASCENDING = 25;
        private const int EVERYTHING_SORT_DATE_RUN_DESCENDING = 26;

        private const int EVERYTHING_TARGET_MACHINE_X86 = 1;
        private const int EVERYTHING_TARGET_MACHINE_X64 = 2;
        private const int EVERYTHING_TARGET_MACHINE_ARM = 3;

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_SetSearch(string lpSearchString);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetMatchPath(bool bEnable);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetMatchCase(bool bEnable);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetRegex(bool bEnable);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetMax(uint dwMax);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetOffset(uint dwOffset);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetMatchPath();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetMatchCase();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetMatchWholeWord();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetRegex();
        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetMax();
        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetOffset();
        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetSearch();
        [DllImport("Everything32.dll")]
        public static extern int Everything_GetLastError();

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern bool Everything_Query(bool bWait);

        [DllImport("Everything32.dll")]
        public static extern void Everything_SortResultsByPath();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetNumFileResults();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetNumFolderResults();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetNumResults();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetTotFileResults();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetTotFolderResults();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetTotResults();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsVolumeResult(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsFolderResult(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsFileResult(uint nIndex);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathName(uint nIndex, StringBuilder lpString, uint nMaxCount);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultPath(uint nIndex);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultFileName(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern void Everything_Reset();

        [DllImport("Everything32.dll")]
        public static extern void Everything_CleanUp();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetMajorVersion();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetMinorVersion();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetRevision();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetBuildNumber();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_Exit();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsDBLoaded();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsAdmin();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_IsAppData();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_RebuildDB();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_UpdateAllFolderIndexes();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_SaveDB();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_SaveRunHistory();

        [DllImport("Everything32.dll")]
        public static extern bool Everything_DeleteRunHistory();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetTargetMachine();


        // Everything 1.4
        [DllImport("Everything32.dll")]
        public static extern void Everything_SetSort(uint dwSortType);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetSort();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetResultListSort();

        [DllImport("Everything32.dll")]
        public static extern void Everything_SetRequestFlags(uint dwRequestFlags);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetRequestFlags();

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetResultListRequestFlags();

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultExtension(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultSize(uint nIndex, out long lpFileSize);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultDateCreated(uint nIndex, out long lpFileTime);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultDateModified(uint nIndex, out long lpFileTime);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultDateAccessed(uint nIndex, out long lpFileTime);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetResultAttributes(uint nIndex);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultFileListFileName(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetResultRunCount(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultDateRun(uint nIndex, out long lpFileTime);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_GetResultDateRecentlyChanged(uint nIndex, out long lpFileTime);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultHighlightedFileName(uint nIndex);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultHighlightedPath(uint nIndex);

        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern string Everything_GetResultHighlightedFullPathAndFileName(uint nIndex);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_GetRunCountFromFileName(string lpFileName);

        [DllImport("Everything32.dll")]
        public static extern bool Everything_SetRunCountFromFileName(string lpFileName, uint dwRunCount);

        [DllImport("Everything32.dll")]
        public static extern uint Everything_IncRunCountFromFileName(string lpFileName);

        public static List<string> Search(string query)
        {
            try
            {
                var result = new List<string>();
                const int BUFSIZE = 260;
                var buf = new StringBuilder(BUFSIZE);

                Everything_SetSearch(query);
                Everything_SetRequestFlags(EVERYTHING_REQUEST_FILE_NAME | EVERYTHING_REQUEST_PATH);
                Everything_SetMatchCase(false);
                if (!Everything_Query(true))
                    return result;

                var nrResults = Everything_GetNumResults();

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
                Everything_CleanUp();
            }
        }
    }
}
