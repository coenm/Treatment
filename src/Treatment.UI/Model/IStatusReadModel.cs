namespace Treatment.UI.Model
{
    using System;

    public interface IStatusReadModel
    {
        event EventHandler Updated;

        string StatusText { get; }

        string ConfigFilename { get; }
    }
}
