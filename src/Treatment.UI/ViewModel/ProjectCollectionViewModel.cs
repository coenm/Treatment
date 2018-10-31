namespace Treatment.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using CoenM.Encoding;
    using JetBrains.Annotations;
    using Nito.Mvvm;

    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;

    public class ProjectCollectionViewModel : ViewModelBase, IProjectCollectionViewModel, IInitializableViewModel, IDisposable
    {
        [NotNull] private readonly IProjectViewModelFactory projectViewModelFactory;
        [NotNull] private readonly IFileSearch fileSearch;
        [NotNull] private readonly IConfiguration configuration;

        public ProjectCollectionViewModel(
            [NotNull] IProjectViewModelFactory projectViewModelFactory,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfiguration configuration)
        {
            this.projectViewModelFactory = Guard.NotNull(projectViewModelFactory, nameof(projectViewModelFactory));
            this.fileSearch = Guard.NotNull(fileSearch, nameof(fileSearch));
            this.configuration = Guard.NotNull(configuration, nameof(configuration));

            Projects = new ObservableCollection<ProjectViewModel>();
            Initialize = new CapturingExceptionAsyncCommand(async _ => await LoadProjectsAsync());
        }

        public ObservableCollection<ProjectViewModel> Projects { get; }

        System.Windows.Input.ICommand IInitializableViewModel.Initialize => Initialize;

        public CapturingExceptionAsyncCommand Initialize { get; }

        public void Dispose()
        {
        }

        private static string Hash([CanBeNull] string filename)
        {
            if (filename == null)
                return string.Empty;

            using (var crypt = new SHA256Managed())
            {
                var result = crypt.ComputeHash(
                                               crypt.ComputeHash(
                                                                 new byte[] { 0, 1, 3, 5, 7, 9 }
                                                                     .Concat(Encoding.ASCII.GetBytes(filename))
                                                                     .ToArray())
                                                    .Concat(new byte[] { 34, 22, 230, 33, 33, 0 })
                                                    .ToArray());

                return Z85.Encode(result);
            }
        }

        private async Task LoadProjectsAsync()
        {
            await Task.Delay(3000); // stupid delay to see something happening ;-)
            var items = CreateProjectViewModelsFromDirectory();
            foreach (var item in items)
            {
                Projects.Add(item);
                await Task.Delay(1000); // again stupid delay to see something happening ;-)
            }
        }

        [NotNull]
        private IEnumerable<ProjectViewModel> CreateProjectViewModelsFromDirectory()
        {
            var rootPath = configuration.RootPath;
            if (rootPath == null)
                yield break;

            if (!Directory.Exists(rootPath))
                yield break;

            var files = fileSearch.FindFilesIncludingSubdirectories(rootPath, "*.sln");

            foreach (var file in files)
            {
                DirectoryInfo rootDirectoryInfo;
                try
                {
                    var filename = Path.GetFileName(file);
                    if (filename == null)
                        continue;

                    // No need to expose the filename to look for at github ;-)
                    if (Hash(filename) != "fTABLb)<0:PI1+6/8C%b5gd>4nRK{6SerJz+C)ik")
                        continue;

                    var fullDirectory = Path.GetDirectoryName(file);
                    var folderName = Path.GetFileName(fullDirectory);

                    if (folderName != "sln")
                        continue;

                    rootDirectoryInfo = Directory.GetParent(fullDirectory);
                    if (rootDirectoryInfo == null)
                        continue;
                }
                catch (Exception)
                {
                    // swallow
                    continue;
                }

                yield return projectViewModelFactory.Create(rootDirectoryInfo.Name, rootDirectoryInfo.FullName);
            }
        }
    }
}
