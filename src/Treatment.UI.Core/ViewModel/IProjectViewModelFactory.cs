namespace Treatment.UI.Core.ViewModel
{
    /// <summary>
    /// Factory of <see cref="ProjectViewModel"/>.
    /// </summary>
    public interface IProjectViewModelFactory
    {
        ProjectViewModel Create(string rootDirectoryInfoName, string rootDirectoryInfoFullName);
    }
}
