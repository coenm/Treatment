﻿namespace Treatment.UI.Core.Configuration
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IConfigurationService : IReadOnlyConfigurationService
    {
        Task<bool> UpdateAsync([NotNull] ApplicationSettings configuration);
    }
}
