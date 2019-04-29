﻿namespace TestAgent.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interface;

    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly IEnumerable<IRequestHandler> handlers;

        public RequestDispatcher(IEnumerable<IRequestHandler> handlers)
        {
            this.handlers = handlers.ToList();
        }

        public async Task<IResponse> ProcessAsync(IRequest request)
        {
            var handler = handlers.FirstOrDefault(h => h.CanHandle(request));

            if (handler == null)
                throw new NotImplementedException();

            return await handler.ExecuteAsync(request);
        }
    }
}
