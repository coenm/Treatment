﻿namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Threading.Tasks;

    public interface IMouse : IDisposable
    {
        Task<bool> DoubleClickAsync();

        Task<bool> ClickAsync();

        Task<bool> MoveCursorAsync(int x, int y);

        Task<bool> MouseDownAsync();

        Task<bool> MouseUpAsync();
    }
}
