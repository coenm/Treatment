namespace Treatment.UI.Model
{
    using System;

    public interface IStatusModel
    {
        event EventHandler Updated;

        string StatusText { get; set; }
    }
}
