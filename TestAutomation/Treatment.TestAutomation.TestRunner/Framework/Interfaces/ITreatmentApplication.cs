namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public interface ITreatmentApplication : IApplication
    {
        bool Created { get; }

        ApplicationActivationState State { get; }
    }
}
