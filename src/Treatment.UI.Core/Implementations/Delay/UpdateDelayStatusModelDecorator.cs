﻿namespace Treatment.UI.Core.Implementations.Delay
{
    using System.Threading;
    using System.Threading.Tasks;
    using Treatment.Helpers.Guards;
    using JetBrains.Annotations;
    using Model;

    [UsedImplicitly]
    internal class UpdateDelayStatusModelDecorator : IDelayService
    {
        [NotNull] private readonly IDelayService decoratee;
        [NotNull] private readonly IStatusFullModel statusModel;

        public UpdateDelayStatusModelDecorator(
            [NotNull] IDelayService decoratee,
            [NotNull] IStatusFullModel statusModel)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            Guard.NotNull(statusModel, nameof(statusModel));
            this.decoratee = decoratee;
            this.statusModel = statusModel;
        }

        public async Task DelayAsync(CancellationToken ct = default)
        {
            using (statusModel.NotifyDelay())
            {
                await decoratee.DelayAsync(ct).ConfigureAwait(false);
            }
        }
    }
}
