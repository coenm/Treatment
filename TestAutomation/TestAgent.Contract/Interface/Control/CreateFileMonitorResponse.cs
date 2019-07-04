namespace TestAgent.Contract.Interface.Control
{
    using System;

    public class CreateFileMonitorResponse : IControlResponse
    {
        public Guid MonitorGuid { get; set; }
    }
}