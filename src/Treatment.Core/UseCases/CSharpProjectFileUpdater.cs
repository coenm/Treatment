namespace Treatment.Core.UseCases
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using JetBrains.Annotations;

    public class CSharpProjectFileUpdater
    {
        [NotNull]
        private readonly XDocument doc;
        [NotNull]
        private readonly XNamespace msbuildNamespace;

        private CSharpProjectFileUpdater(XDocument doc)
        {
            msbuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            this.doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        [PublicAPI]
        public bool HasChanges { get; private set; }

        public static CSharpProjectFileUpdater Create(Stream stream)
        {
            var doc = XDocument.Load(stream/*, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo*/);
            return new CSharpProjectFileUpdater(doc);
        }

        /*
        [NotNull, PublicAPI]
        //private const string SEARCH = @"<HintPath>[\.\.\\]+Packages\\(.+\.dll)</HintPath>";
        //private const string REPLACE = @"<HintPath>$(PackagesDir)\$1</HintPath>";
        public CSharpProjectFileUpdater UpdateRelativeHintPathsWithVariable(string packagesVariable = "$(PackagesDir)")
        {
            if (packagesVariable == null) // empty is okay
                throw new ArgumentNullException(nameof(packagesVariable));

            throw new NotImplementedException();
            return this;
        }
        */

        [NotNull]
        [PublicAPI]
        public CSharpProjectFileUpdater RemoveEmptyItemGroups()
        {
            if (doc.Root == null)
                return this;

            if (!doc.Root.HasElements)
                return this;

            var itemGroups = doc
                             .Element(msbuildNamespace + "Project")
                             .Elements(msbuildNamespace + "ItemGroup")
                             .Where(itemGroup => itemGroup.HasElements == false);

            foreach (var itemGroup in itemGroups)
            {
                itemGroup.Remove();
                HasChanges = true;
            }

            return this;
        }

        /// <summary>
        /// Removes the 'app.config' or 'App.config' file from the project (csproj) file.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public CSharpProjectFileUpdater RemoveAppConfig()
        {
            if (doc.Root == null)
                return this;

            if (!doc.Root.HasElements)
                return this;

            var itemGroups = doc
                             .Element(msbuildNamespace + "Project")
                             .Elements(msbuildNamespace + "ItemGroup")
                             .Where(itemGroup => itemGroup.Elements(msbuildNamespace + "None") != null
                                                 &&
                                                 itemGroup.Elements(msbuildNamespace + "None")
                                                          .Any(noneElement => noneElement.Attribute("Include") != null
                                                                              &&
                                                                              (noneElement.Attribute("Include").Value == "app.config"
                                                                               ||
                                                                               noneElement.Attribute("Include").Value == "App.config")));

            foreach (var itemGroup in itemGroups)
            {
                foreach (var noneElement in itemGroup.Elements(msbuildNamespace + "None"))
                {
                    var value = noneElement.Attribute("Include")?.Value;
                    if (string.IsNullOrWhiteSpace(value) || string.Compare(value, "app.config", StringComparison.InvariantCultureIgnoreCase) != 0)
                        continue;

                    noneElement.Remove();
                    HasChanges = true;
                }
            }

            return this;
        }

        /// <summary>
        /// Write the updated file to stream.
        /// </summary>
        /// <param name="stream">Stream to write the csproj document to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="stream"/> is not writable.</exception>
        [PublicAPI]
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.CanWrite == false)
                throw new NotSupportedException($"Cannot write to stream '{nameof(stream)}'.");

            doc.Save(stream/*, SaveOptions.DisableFormatting*/);
        }
    }
}
