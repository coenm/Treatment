namespace TestAgent.ZeroMq
{
    public interface ILogger
    {
        void Error(string msg);

        void Debug(string msg);

        void Warn(string msg);

        void Info(string msg);
    }
}
