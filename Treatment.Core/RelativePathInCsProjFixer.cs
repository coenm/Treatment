namespace Treatment.Core
{
    using System.Text.RegularExpressions;

    using Treatment.Core.Interfaces;

    public class RelativePathInCsProjFixer
    {
        private const string SEARCH = @"<HintPath>[\.\.\\]+Packages\\(.+\.dll)</HintPath>";
        private const string REPLACE = @"<HintPath>$(PackagesDir)\$1</HintPath>";

        private readonly IFileSystem _filesystem;
        private readonly IFileSearch _fileSearcher;
        private readonly Regex _regex;

        public RelativePathInCsProjFixer(IFileSystem filesystem, IFileSearch fileSearcher)
        {
            _filesystem = filesystem;
            _fileSearcher = fileSearcher;
            _regex = new Regex(SEARCH, RegexOptions.Compiled);
        }

        public string[] GetCsFiles(string rootpath)
        {
            return _fileSearcher.FindFilesIncludingSubdirectories(rootpath, "*.csproj");
        }

        public void FixSingleFile(string filename)
        {
            var originalContent = _filesystem.GetFileContent(filename);

            var updatedContent = FixContent(originalContent);

            if (originalContent.Equals(updatedContent))
                return;

            _filesystem.SaveContent(filename, updatedContent);
        }

        private string FixContent(string data)
        {
            var result = data;
            var match = _regex.Match(result);
            while (match.Success)
            {
                result = _regex.Replace(data, REPLACE);
                match = _regex.Match(result);
            }

            return result;
        }
    }
}