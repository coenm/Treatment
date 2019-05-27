namespace Treatment.Contract
{
    using JetBrains.Annotations;

    public struct ProgressData
    {
        private ProgressData(ProgressDataPosition position, ProgressState state)
        {
            Position = position;
            State = state;
        }

        public ProgressState State { get; }

        public ProgressDataPosition Position { get; }

        [MustUseReturnValue]
        [PublicAPI]
        public static ProgressData InProgressWithoutPosition()
        {
            return InProgress(ProgressDataPosition.NoPosition);
        }

        [MustUseReturnValue]
        [PublicAPI]
        public static ProgressData InProgress(ProgressDataPosition position)
        {
            return new ProgressData(position, ProgressState.Busy);
        }

        [MustUseReturnValue]
        [PublicAPI]
        public static ProgressData FinishedSuccessfully()
        {
            return new ProgressData(ProgressDataPosition.NoPosition, ProgressState.FinishedSuccessfully);
        }

        [MustUseReturnValue]
        [PublicAPI]
        public static ProgressData FinishedWithError(ProgressDataPosition position)
        {
            return new ProgressData(position, ProgressState.FinishedWithError);
        }
    }
}
