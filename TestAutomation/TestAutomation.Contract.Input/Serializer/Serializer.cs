namespace TestAutomation.Contract.Input.Serializer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interface;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    public static class RequestResponseSerializer
    {
        private static readonly List<Type> RequestTypes = typeof(IRequest).Assembly
            .GetTypes()
            .Where(type => typeof(IRequest).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        private static readonly List<Type> ResponseTypes = typeof(IResponse).Assembly
            .GetTypes()
            .Where(type => typeof(IResponse).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();


        public static (string, string) Serialize([NotNull] IRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return (request.GetType().FullName, JsonConvert.SerializeObject(request));
        }

        public static (string, string) Serialize([NotNull] IResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return (response.GetType().FullName, JsonConvert.SerializeObject(response));
        }

        public static IRequest DeserializeRequest([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = RequestTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IResponse DeserializeResponse([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = ResponseTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
