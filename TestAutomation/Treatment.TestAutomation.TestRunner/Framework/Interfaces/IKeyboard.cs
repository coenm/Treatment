namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface.Input.Enums;

    public interface IKeyboard : IDisposable
    {
        Task<bool> PressAsync(params VirtualKeyCode[] keys);

        Task<bool> KeyDownAsync(params VirtualKeyCode[] keys);

        Task<bool> KeyUpAsync(params VirtualKeyCode[] keys);

        Task<bool> KeyCombinationPressAsync(params VirtualKeyCode[] keys);
    }
}
