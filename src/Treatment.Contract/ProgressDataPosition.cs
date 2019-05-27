namespace Treatment.Contract
{
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    public struct ProgressDataPosition
    {
        public ProgressDataPosition(int currentValue, int max)
        {
            Guard.MustBeLessThanOrEqualTo(currentValue, max, nameof(currentValue));
            CurrentValue = currentValue;
            Max = max;
        }

        public static ProgressDataPosition NoPosition { get; } = new ProgressDataPosition(-1, -1);

        public int CurrentValue { get; }

        public int Max { get; }

        [MustUseReturnValue]
        [Pure]
        public ProgressDataPosition CreateIncrementalPosition()
        {
            if (CurrentValue < Max)
                return new ProgressDataPosition(CurrentValue + 1, Max);

            return new ProgressDataPosition(CurrentValue, Max);
        }
    }
}
