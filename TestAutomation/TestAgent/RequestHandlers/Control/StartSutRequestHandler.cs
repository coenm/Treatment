namespace TestAgent.RequestHandlers.Control
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Contract.Interface;
    using Contract.Interface.Control;
    using JetBrains.Annotations;
    using Medallion.Shell;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class StartSutRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IAgentContext context;

        public StartSutRequestHandler([NotNull] IAgentContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public bool CanHandle(IRequest request) => request is StartSutRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as StartSutRequest);

        private Task<IResponse> ExecuteAsync(StartSutRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (!File.Exists(request.Executable))
            {
                return Task.FromResult(new OkResponse
                {
                    Msg = "File does not exists."
                } as IResponse);
            }

            var command = Command.Run(
                request.Executable,
                new string[0],
                options =>
                {
                    options.WorkingDirectory(request.WorkingDirectory);
                    options.CancellationToken(context.CancellationToken);
                    options.EnvironmentVariables(new[]
                    {
                        new KeyValuePair<string, string>("ENABLE_TEST_AUTOMATION", "true"),
                        new KeyValuePair<string, string>("TA_KEY", string.Empty),
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", $"tcp://localhost:{Settings.SutPublishPort}"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", $"tcp://localhost:{Settings.SutReqRspPort}"), // sut handles the mouse and keyboard requests.
                    });
                });

            context.SetSutProcess(command);

            return Task.FromResult(new OkResponse { Msg = "Started" } as IResponse);


//            // inproc://publish
//            using (var agentPublishSocket = new ZSocket(context, ZSocketType.PUB))
//            {
//                agentPublishSocket.Connect("inproc://publish");
//                agentPublishSocket.Send(new ZMessage(new[]
//                {
//                    new ZFrame("AGENT"),
//                    new ZFrame("Started"),
//                }));
//            }

            //            var result = await command.Task.ConfigureAwait(true);
            //
            //            Console.WriteLine(result.StandardOutput);
            //            Console.WriteLine(result.StandardError);
        }
    }
}
