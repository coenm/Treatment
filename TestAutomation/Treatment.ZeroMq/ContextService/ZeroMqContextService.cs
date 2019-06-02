namespace TreatmentZeroMq.ContextService
{
    using System;

    using ZeroMQ;

    public class ZeroMqContextService : IZeroMqContextService
    {
        private readonly object syncLock = new object();
        private ZContext context;

        public ZContext GetContext()
        {
            lock (syncLock)
            {
                return context ?? (context = new ZContext());
            }
        }

        public void DisposeCurrentContext()
        {
            lock (syncLock)
            {
                if (context == null)
                    return;

                try
                {
                    // Terminate is blocking
                    if (!context.Terminate(out var error))
                        throw new ZException(error, "Could not Terminate the ZeroMq context");

                    context.Dispose();
                }
                catch (Exception)
                {
                    // _logger.Error($"Could not dispose the ZeroMq context. {e.Message}", e, Priority.High);
                }
                finally
                {
                    context = null;
                }
            }
        }
    }
}
