namespace Treatment.Core.UseCases
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using JetBrains.Annotations;

    public class CSharpProjectFileUpdater
    {
        [NotNull] private readonly XDocument _doc;
        [NotNull] private readonly XNamespace _msbuildNamespace;
        private bool _hasChanges;

        private CSharpProjectFileUpdater(XDocument doc)
        {
            _msbuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public static CSharpProjectFileUpdater Create(Stream stream)
        {
            var doc = XDocument.Load(stream);
            return new CSharpProjectFileUpdater(doc);
        }

        [PublicAPI]
        public bool HasChanges => _hasChanges;

        // [NotNull, PublicAPI]
        // //private const string SEARCH = @"<HintPath>[\.\.\\]+Packages\\(.+\.dll)</HintPath>";
        // //private const string REPLACE = @"<HintPath>$(PackagesDir)\$1</HintPath>";
        // public CSharpProjectFileUpdater UpdateRelativeHintPathsWithVariable(string packagesVariable = "$(PackagesDir)")
        // {
        //     if (packagesVariable == null) // empty is okay
        //         throw new ArgumentNullException(nameof(packagesVariable));
        //
        //     throw new NotImplementedException();
        //     return this;
        // }

        [NotNull, PublicAPI]
        public CSharpProjectFileUpdater RemoveEmptyItemGroups()
        {
            if (!_doc.Root.HasElements)
                return this;

            var itemGroups = _doc
                             .Element(_msbuildNamespace + "Project")
                             .Elements(_msbuildNamespace + "ItemGroup")
                             .Where(itemGroup => itemGroup.HasElements == false)
                             .ToArray();

            foreach (var itemGroup in itemGroups)
            {
                itemGroup.Remove();
                _hasChanges = true;
            }

            return this;
        }

        [NotNull, PublicAPI]
        public CSharpProjectFileUpdater RemoveAppConfig()
        {
            if (!_doc.Root.HasElements)
                return this;

            var itemGroups = _doc
                             .Element(_msbuildNamespace + "Project")
                             .Elements(_msbuildNamespace + "ItemGroup")
                             .Where(itemGroup => itemGroup.Elements(_msbuildNamespace + "None") != null
                                                 &&
                                                 itemGroup.Elements(_msbuildNamespace + "None")
                                                          .Any(noneElement => noneElement.Attribute("Include") != null
                                                                              &&
                                                                              (noneElement.Attribute("Include").Value == "app.config" || noneElement.Attribute("Include").Value == "App.config")));

            foreach (var itemGroup in itemGroups)
            foreach (var noneElement in itemGroup.Elements(_msbuildNamespace + "None"))
            {
                var value = noneElement.Attribute("Include")?.Value;
                if (string.IsNullOrWhiteSpace(value) || string.Compare(value, "app.config", StringComparison.InvariantCultureIgnoreCase) != 0)
                    continue;

                noneElement.Remove();
                _hasChanges = true;
            }

            return this;
        }

        [PublicAPI]
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _doc.Save(stream);
        }
    }
}