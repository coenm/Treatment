namespace Treatment.Core.Statistics
{
    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class FakeSummaryWriter : ISummaryWriter
    {
        public void OutputSummary()
        {
            // intentionally do nothing.
        }
    }
}   