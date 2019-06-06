namespace Treatment.Plugin.Svn.Implementation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using JetBrains.Annotations;
    using SharpSvn;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Helpers.FileSystem;
    using Treatment.Helpers.Guards;

    internal class SvnReadOnlySourceControl : IReadOnlySourceControl
    {
        private readonly IFileSystem filesystem;

        public SvnReadOnlySourceControl([NotNull] IFileSystem filesystem)
        {
            Guard.NotNull(filesystem, nameof(filesystem));
            this.filesystem = filesystem;
        }

        public bool TryGetSvnRoot([NotNull] string path, out string rootPath)
        {
            Guard.NotNull(path, nameof(path));

            try
            {
                using (var client = new SvnClient())
                {
                    var result = client.GetInfo(path, out var info);
                    rootPath = result ? info.WorkingCopyRoot : null;
                    return result;
                }
            }
            catch (SvnInvalidNodeKindException)
            {
                // expected when root does not exists.
                // do nothing specific
            }
            catch (Exception)
            {
                // not expected.
                // log exception (todo)
            }

            rootPath = null;
            return false;
        }

        public string GetOriginal(string filename)
        {
            throw new NotImplementedException();
        }

        public FileStatus GetFileStatus(string path)
        {
            Guard.NotNull(path, nameof(path));

            try
            {
                using (var client = new SvnClient())
                {
                    if (!client.GetStatus(path, out var statuses))
                    {
                        throw new Exception("Could not get status");
                    }

                    if (!statuses.Any())
                    {
                        return FileStatus.Unchanged;
                    }

                    if (statuses.Count > 1)
                    {
                        throw new Exception("Not a file");
                    }

                    return MapStatus(statuses.Single().LocalContentStatus);
                }
            }
            catch (SvnInvalidNodeKindException)
            {
                // expected when root does not exists.
                // do nothing specific
                if (filesystem.FileExists(path))
                    return FileStatus.Unknown;

                return FileStatus.NotExist;
            }
            catch (SvnWorkingCopyPathNotFoundException)
            {
                // path can exist on filesystem but then the file is placed in a path that is not under version control
                if (filesystem.FileExists(path))
                    return FileStatus.New;

                return FileStatus.NotExist;
            }
            catch (Exception e)
            {
                // not expected.
                // log exception (todo)
                throw e;
            }

            // if (File.Exists(path))
            //     return FileStatus.Exist;

            // return FileStatus.Unknown;
        }

        public string GetModifications(string fileName)
        {
            if (!filesystem.FileExists(fileName))
                throw new Exception("File does not exists");

            try
            {
                var working = new SvnPathTarget(fileName, SvnRevisionType.Working);
                var @base = new SvnPathTarget(fileName, SvnRevisionType.Base);

                using (var client = new SvnClient())
                using (var ms = new MemoryStream())
                {
                    if (!client.Diff(@base, working, ms))
                        return null;

                    ms.Position = 0;
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static FileStatus MapStatus(SvnStatus svnStatus)
        {
            switch (svnStatus)
            {
                case SvnStatus.Incomplete:
                    break;
                case SvnStatus.External:
                    break;
                case SvnStatus.Obstructed:
                    break;
                case SvnStatus.Ignored:
                    return FileStatus.New;

                case SvnStatus.Conflicted:
                case SvnStatus.Merged:
                case SvnStatus.Modified:
                    return FileStatus.Modified;

                case SvnStatus.Replaced:
                    return FileStatus.New;
                case SvnStatus.Deleted:
                    return FileStatus.Modified; // not sure
                case SvnStatus.Missing:
                    return FileStatus.NotExist;
                case SvnStatus.Added:
                    return FileStatus.New;
                case SvnStatus.Normal:
                    break;
                case SvnStatus.NotVersioned:
                    return FileStatus.New;
                case SvnStatus.None:
                    return FileStatus.NotExist;
                case SvnStatus.Zero:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(svnStatus), svnStatus, null);
            }

            throw new ArgumentOutOfRangeException(nameof(svnStatus), svnStatus, null);
        }
    }
}
