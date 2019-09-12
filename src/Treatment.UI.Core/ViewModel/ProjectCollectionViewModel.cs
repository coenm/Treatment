namespace Treatment.UI.Core.ViewModel
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
    using NLog;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Core.Configuration;
    using Treatment.UI.Core.Implementations.Delay;
    using Treatment.UI.Core.Model;
    using Wpf.Framework.ViewModel;

    public class ProjectCollectionViewModel : ViewModelBase, IProjectCollectionViewModel, IInitializableViewModel, IDisposable
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly IStatusFullModel statusModel;
        [NotNull] private readonly IProjectViewModelFactory projectViewModelFactory;
        [NotNull] private readonly IFileSearch fileSearch;
        [NotNull] private readonly IConfigurationService configurationService;
        [NotNull] private readonly IDelayService delayService;

        public ProjectCollectionViewModel(
            [NotNull] IProjectViewModelFactory projectViewModelFactory,
            [NotNull] IStatusFullModel statusModel,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfigurationService configurationService,
            [NotNull] IDelayService delayService)
        {
            Guard.NotNull(statusModel, nameof(statusModel));
            Guard.NotNull(projectViewModelFactory, nameof(projectViewModelFactory));
            Guard.NotNull(fileSearch, nameof(fileSearch));
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(delayService, nameof(delayService));

            this.statusModel = statusModel;
            this.projectViewModelFactory = projectViewModelFactory;
            this.fileSearch = fileSearch;
            this.configurationService = configurationService;
            this.delayService = delayService;

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
            statusModel.UpdateStatus("Loading projects ..");
            var config = await configurationService.GetAsync();
            var rootPath = config.RootDirectory;

            await delayService.DelayAsync(); // stupid delay to see something happening ;-)
            var items = CreateProjectViewModelsFromDirectory(rootPath).ToList();
            foreach (var item in items)
            {
                Projects.Add(item);
                await delayService.DelayAsync(); // stupid delay to see something happening ;-)
            }

            switch (items.Count)
            {
                case 0:
                    statusModel.UpdateStatus("Could not load any projects");
                    break;
                case 1:
                    statusModel.UpdateStatus("One project loaded.");
                    break;
                default:
                    statusModel.UpdateStatus($"{items.Count} Projects loaded.");
                    break;
            }
        }

        [NotNull]
        private IEnumerable<ProjectViewModel> CreateProjectViewModelsFromDirectory(string rootPath)
        {
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

                    folderName = Path.GetFileName(rootDirectoryInfo.FullName);
                    if (Hash(folderName) == "^xlNfDu.edW?Mb0YN41!m%rZkn1{%&giDcm9<5A7")
                        rootDirectoryInfo = rootDirectoryInfo.Parent;

                    if (rootDirectoryInfo == null)
                        continue;
                }
                catch (Exception e)
                {
                    // Log and swallow
                    Logger.Error(e, "Something went terrably wrong processing the found solution files.");
                    continue;
                }

                yield return projectViewModelFactory.Create(rootDirectoryInfo.Name, rootDirectoryInfo.FullName);
            }
        }
    }
}
