namespace TestAgent.Contract.Serializer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using TestAgent.Contract.Interface;

    public static class TestAgentRequestResponseSerializer
    {
        private static readonly List<Type> RequestTypes = typeof(IControlRequest).Assembly
            .GetTypes()
            .Where(type => typeof(IControlRequest).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        private static readonly List<Type> ResponseTypes = typeof(IControlResponse).Assembly
            .GetTypes()
            .Where(type => typeof(IControlResponse).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        public static (string, string) Serialize([NotNull] IControlRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return (request.GetType().FullName, JsonConvert.SerializeObject(request));
        }

        public static (string, string) Serialize([NotNull] IControlResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return (response.GetType().FullName, JsonConvert.SerializeObject(response));
        }

        public static IControlRequest DeserializeRequest([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = RequestTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IControlRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IControlResponse DeserializeResponse([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = ResponseTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IControlResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
