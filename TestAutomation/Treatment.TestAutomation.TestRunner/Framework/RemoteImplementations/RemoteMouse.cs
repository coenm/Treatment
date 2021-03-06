﻿namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface.Base;
    using TestAgent.Contract.Interface.Input.Mouse;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteMouse : IMouse
    {
        private readonly IExecuteInput execute;

        public RemoteMouse(IExecuteInput execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public void Dispose()
        {
        }

        public async Task<bool> DoubleClickAsync()
        {
            var req = new DoubleClickRequest
              {
                  Button = MouseButtons.Left,
              };

            var result = await execute.ExecuteInput(req);

            return result is DoubleClickResponse;
        }

        public async Task<bool> ClickAsync()
        {
            var req = new SingleClickRequest
              {
                  Button = MouseButtons.Left,
              };

            var result = await execute.ExecuteInput(req);

            return result is SingleClickResponse;
        }

        public async Task<bool> MoveCursorAsync(int x, int y)
        {
            var req = new MoveMouseToRequest
              {
                  Position = new Point
                     {
                         X = x,
                         Y = y,
                     },
              };

            var result = await execute.ExecuteInput(req);

            return result is MoveMouseToResponse;
        }

        public async Task<bool> MouseDownAsync()
        {
            var req = new MouseDownRequest();

            var result = await execute.ExecuteInput(req);

            return result is MouseDownResponse;
        }

        public async Task<bool> MouseUpAsync()
        {
            var req = new MouseUpRequest();

            var result = await execute.ExecuteInput(req);

            return result is MouseUpResponse;
        }
    }
}
