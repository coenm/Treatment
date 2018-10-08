namespace Treatment.Contract
{
    using JetBrains.Annotations;

    public struct ProgressData
    {
        public int CurrentStep { get; set; }

        public int TotalSteps { get; set; }

        [CanBeNull]
        public string Message { get; set; }
    }
}