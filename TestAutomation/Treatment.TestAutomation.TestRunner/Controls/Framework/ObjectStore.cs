namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Collections.Generic;

    public class ObjectStore
    {
        private Dictionary<Guid, object> objects;

        public ObjectStore()
        {
            objects = new Dictionary<Guid, object>();
        }

        
    }
}
