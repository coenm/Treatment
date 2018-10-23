using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Treatment.Contract;

namespace Treatment.Core.Bootstrap
{
    [DebuggerDisplay("{QueryType.Name,nq}")]
    public sealed class QueryInfo
    {
        public QueryInfo(Type queryType)
        {
            QueryType = queryType;
            ResultType = DetermineResultTypes(queryType).Single();
        }

        public Type QueryType { get; }

        public Type ResultType { get; }

        public static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type)
        {
            return from interfaceType in type.GetInterfaces()
                where interfaceType.IsGenericType
                where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
                select interfaceType.GetGenericArguments()[0];
        }
    }
}
