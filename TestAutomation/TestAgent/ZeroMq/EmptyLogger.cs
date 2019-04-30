namespace TestAgent.ZeroMq
{
    public class EmptyLogger : ILogger
    {
        public void Error(string msg)
        {
        }

        public void Debug(string msg)
        {
        }

        public void Warn(string msg)
        {
        }

        public void Info(string msg)
        {
        }
    }
}
