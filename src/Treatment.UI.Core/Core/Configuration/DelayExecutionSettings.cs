﻿namespace Treatment.UI.Core.Core.Configuration
{
    public class DelayExecutionSettings
    {
        public bool Enabled { get; set; }

        public int MinMilliseconds { get; set; }

        public int MaxMilliseconds { get; set; }
    }
}