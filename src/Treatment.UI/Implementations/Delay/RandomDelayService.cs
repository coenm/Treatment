namespace Treatment.UI.Implementations.Delay
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    internal class RandomDelayService : IDelayService
    {
        private readonly int minMilliseconds;
        private readonly int maxMilliseconds;
        [NotNull] private readonly Random random;

        public RandomDelayService(int minMilliseconds, int maxMilliseconds)
        {
            this.minMilliseconds = minMilliseconds;
            this.maxMilliseconds = maxMilliseconds;
            random = new Random();
        }

        public async Task DelayAsync(CancellationToken ct = default)
        {
            var millisecondsDelay = random.Next(minMilliseconds, maxMilliseconds);
            await Task.Delay(millisecondsDelay, ct).ConfigureAwait(false);
        }
    }
}
