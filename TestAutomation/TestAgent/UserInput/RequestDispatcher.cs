namespace TestAgent.UserInput
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.InputHandler.RequestHandlers;

    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly IEnumerable<IRequestHandler> handlers;

        public RequestDispatcher(IEnumerable<IRequestHandler> handlers)
        {
            this.handlers = handlers.ToList();
        }

        public async Task<IInputResponse> ProcessAsync(IInputRequest request)
        {
            try
            {
                var handler = handlers.FirstOrDefault(h => h.CanHandle(request));

                if (handler == null)
                    throw new NotImplementedException();

                return await handler.ExecuteAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
