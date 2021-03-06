﻿namespace Treatment.Core.Bootstrap.Plugin
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;
    using NLog;
    using Treatment.Helpers.Guards;

    public static class PluginFinder
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<Assembly> FindPluginAssemblies([NotNull] string baseDirectory)
        {
            Guard.NotNullOrWhiteSpace(baseDirectory, nameof(baseDirectory));

            Logger.Debug(() => $"Plugin base directory {baseDirectory}");

            var assemblies = GetPluginAssembliesInDirectory(baseDirectory);

            foreach (var dir in GetPluginDirectories(baseDirectory))
            {
                assemblies = assemblies.Concat(GetPluginAssembliesInDirectory(dir));
            }

            LogFoundAssemblies(assemblies);

            return assemblies;
        }

        private static IEnumerable<string> GetPluginDirectories(string baseDirectory)
        {
            var pluginBaseDirectory = Path.Combine(baseDirectory, "Plugins");

            if (!Directory.Exists(pluginBaseDirectory))
                return Enumerable.Empty<string>();

            return Directory.EnumerateDirectories(pluginBaseDirectory);
        }

        private static IEnumerable<Assembly> GetPluginAssembliesInDirectory(string baseDirectory)
        {
            return new DirectoryInfo(baseDirectory)
                .GetFiles()
                .Where(file =>
                    file.Name.StartsWith("Treatment.Plugin.")
                    &&
                    file.Extension.ToLower() == ".dll")
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));
        }

        private static void LogFoundAssemblies([NotNull] IEnumerable<Assembly> assemblies)
        {
            DebugGuard.NotNull(assemblies, nameof(assemblies));

            if (!Logger.IsDebugEnabled)
                return;

            var assembliesArray = assemblies.ToArray();
            if (assembliesArray.Any())
            {
                Logger.Debug("Found plugins:");
                foreach (var assembly in assembliesArray)
                    Logger.Debug($"- {assembly.FullName}");
            }
            else
            {
                Logger.Debug("No plugins found");
            }
        }
    }
}
