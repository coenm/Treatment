namespace TestAgent.RequestHandlers.Control
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Medallion.Shell;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Control;
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

            var executable = request.Executable;
            if (string.IsNullOrWhiteSpace(executable) || File.Exists(executable))
                executable = sutExecutable.Executable;

            IControlResponse response;

            if (string.IsNullOrWhiteSpace(executable) || !File.Exists(executable))
            {
                response = new StartSutResponse
                    {
                        Executable = executable,
                        Success = false,
                    };
                return Task.FromResult(response);
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
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", $"tcp://localhost:{FixedSettings.SutPublishPort}"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", $"tcp://localhost:{FixedSettings.SutReqRspPort}"), // sut handles the mouse and keyboard requests.
                    });
                });

            context.SetSutProcess(command);

            response = new StartSutResponse
                {
                    Executable = executable,
                    Success = true,
                };
            return Task.FromResult(response);

            // var result = await command.Task.ConfigureAwait(true);
            // Console.WriteLine(result.StandardOutput);
            // Console.WriteLine(result.StandardError);
        }
    }
}
