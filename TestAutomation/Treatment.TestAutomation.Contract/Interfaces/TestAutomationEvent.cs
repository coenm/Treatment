﻿namespace Treatment.TestAutomation.Contract.Interfaces
{
    using System;

    using Events;

    [Obsolete]
    public class TestAutomationEvent : IEvent
    {
        public Guid Guid { get; set; }

        public string Control { get; set; }

        public string EventName { get; set; }

        public object Payload { get; set; }
    }
}