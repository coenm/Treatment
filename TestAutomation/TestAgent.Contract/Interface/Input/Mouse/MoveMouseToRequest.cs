﻿namespace TestAgent.Contract.Interface.Input.Mouse
{
    using Base;
    using JetBrains.Annotations;

    [PublicAPI]
    public class MoveMouseToRequest : IRequest
    {
        public Point Position { get; set; }
    }
}