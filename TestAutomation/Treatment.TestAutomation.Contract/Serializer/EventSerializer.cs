﻿namespace Treatment.TestAutomation.Contract.Serializer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public static class EventSerializer
    {
        private static readonly Type EventType = typeof(IEvent);
        private static readonly List<Type> EventTypes = EventType.Assembly
            .GetTypes()
            .Where(type => EventType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        [PublicAPI]
        public static (string, string) Serialize([NotNull] IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (EventTypes.All(x => x != @event.GetType()))
                throw new ArgumentNullException(nameof(@event));

            return (@event.GetType().FullName, JsonConvert.SerializeObject(@event));
        }

        [PublicAPI]
        public static IEvent DeserializeEvent([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = EventTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IEvent;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
