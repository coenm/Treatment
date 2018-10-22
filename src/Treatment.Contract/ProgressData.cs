namespace Treatment.Contract
{
    using JetBrains.Annotations;

    public struct ProgressData
    {
        public ProgressData(int currentStep, int totalSteps, [CanBeNull] string message = null)
        {
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
            Message = message;
        }

        public ProgressData(string message)
        {
            CurrentStep = null;
            TotalSteps = null;
            Message = message;
        }

        public int? CurrentStep { get; }

        public int? TotalSteps { get; }

        [CanBeNull]
        public string Message { get; }
    }
}