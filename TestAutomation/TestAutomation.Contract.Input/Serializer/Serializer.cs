namespace TestAutomation.Input.Contract.Serializer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interface;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    public static class InputRequestResponseSerializer
    {
        private static readonly List<Type> RequestTypes = typeof(IInputRequest).Assembly
            .GetTypes()
            .Where(type => typeof(IInputRequest).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        private static readonly List<Type> ResponseTypes = typeof(IInputResponse).Assembly
            .GetTypes()
            .Where(type => typeof(IInputResponse).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();


        public static (string, string) Serialize([NotNull] IInputRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return (request.GetType().FullName, JsonConvert.SerializeObject(request));
        }

        public static (string, string) Serialize([NotNull] IInputResponse inputResponse)
        {
            if (inputResponse == null)
                throw new ArgumentNullException(nameof(inputResponse));

            return (inputResponse.GetType().FullName, JsonConvert.SerializeObject(inputResponse));
        }

        public static IInputRequest DeserializeRequest([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = RequestTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IInputRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IInputResponse DeserializeResponse([NotNull] string type, [NotNull] string payload)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                var payloadType = ResponseTypes.SingleOrDefault(x => x.FullName != null && x.FullName.Equals(type));
                if (payloadType == null)
                    throw new ArgumentException($"Could not find type '{type}'", nameof(type));

                return JsonConvert.DeserializeObject(payload, payloadType) as IInputResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
