﻿namespace TreatmentZeroMq.ContextService
{
    using JetBrains.Annotations;

    using ZeroMQ;

    public interface IZeroMqContextService
    {
        [NotNull]
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
