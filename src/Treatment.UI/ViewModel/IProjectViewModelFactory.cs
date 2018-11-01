namespace Treatment.UI.ViewModel
{
    public interface IProjectViewModelFactory
    {
        ProjectViewModel Create(string rootDirectoryInfoName, string rootDirectoryInfoFullName);
    }
}
