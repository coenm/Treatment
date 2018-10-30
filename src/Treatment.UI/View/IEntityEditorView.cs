﻿namespace Treatment.UI.View
{
    using Treatment.UI.Framework;

    public interface IEntityEditorView<T>
        where T : class
    {
        void Set(IEntityEditorViewModel<T> viewModel);
    }
}
