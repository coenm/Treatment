namespace Treatment.Contract.Plugin.SourceControl
{
    using System;

    [Flags]
    public enum FileStatus
    {
        New = 1 << 0,

        Modified = 1 << 1,

        Unchanged = 1 << 2,

        Exist = 1 << 3,

        /// <summary>
        /// File does not exists on file system (not in or outside of the repository)
        /// </summary>
        NotExist = 1 << 4,

        /// <summary>
        /// State unknown. Can be that file is outside of version control repository root.
        /// </summary>
        Unknown = 1 << 5,
    }

    /*
    public enum SourceControlFileStatus
    {
        /// <summary>
        /// Unversioned item is scheduled for addition.
        /// </summary>
        Added,

        /// <summary>
        /// The contents (as opposed to the properties) of the item conflict with updates received from the repository.
        /// </summary>
        Conflicted,

        /// <summary>
        /// Item is scheduled for deletion.
        /// </summary>
        Deleted,

        /// <summary>
        /// Item is being ignored in the source control system.
        /// </summary>
        Ignored,

        /// <summary>
        /// Item has been modified.
        /// </summary>
        Modified,

        /// <summary>
        /// Item has been replaced in the working copy. This means the file was scheduled for deletion, and then a new file with the same name was scheduled for addition in its place.
        /// </summary>
        Replaced,

        /// <summary>
        /// Item is not (yet) under version control.
        /// </summary>
        Unversioned,

        /// <summary>
        /// Item is missing (e.g., moved or deleted without using the source control tool). This also indicates that a directory is incomplete (a checkout or update was interrupted).
        /// </summary>
        Missing,

        /// <summary>
        /// File does not exists on the file system.
        /// </summary>
        NotExists,

        Unknown,

        Unchanged
    }
    */
}