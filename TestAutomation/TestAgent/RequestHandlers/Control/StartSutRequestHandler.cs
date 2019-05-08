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
        [NotNull] private readonly ISutExecutable sutExecutable;

        public StartSutRequestHandler([NotNull] IAgentContext context, [NotNull] ISutExecutable sutExecutable)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(sutExecutable, nameof(sutExecutable));

            this.context = context;
            this.sutExecutable = sutExecutable;
        }

        public bool CanHandle(IControlRequest request) => request is StartSutRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as StartSutRequest);

        private Task<IControlResponse> ExecuteAsync(StartSutRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var executable = sutExecutable.Executable;

            if (string.IsNullOrWhiteSpace(executable) || !File.Exists(executable))
            {
                return Task.FromResult(new StartSutResponse
                {
                    Executable = executable,
                    Success = false,
                } as IControlResponse);
            }

            var workingDirectory = request.WorkingDirectory;
            if (string.IsNullOrWhiteSpace(workingDirectory) || Directory.Exists(workingDirectory) == false)
                workingDirectory = new FileInfo(executable).DirectoryName;

            var command = Command.Run(
                executable,
                new string[0],
                options =>
                {
                    options.WorkingDirectory(workingDirectory);
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

            return Task.FromResult(new StartSutResponse
            {
                Executable = executable,
                Success = true,
            } as IControlResponse);

            //            var result = await command.Task.ConfigureAwait(true);
            //
            //            Console.WriteLine(result.StandardOutput);
            //            Console.WriteLine(result.StandardError);
        }
    }
}
