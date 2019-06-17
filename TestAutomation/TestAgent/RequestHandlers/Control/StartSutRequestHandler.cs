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
    using TestAgent.Model.Configuration;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class StartSutRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IAgentContext context;
        [NotNull] private readonly IReadOnlyConfigurationService resolveSutExecutable;

        public StartSutRequestHandler([NotNull] IAgentContext context, [NotNull] IReadOnlyConfigurationService resolveSutExecutable)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(resolveSutExecutable, nameof(resolveSutExecutable));

            this.context = context;
            this.resolveSutExecutable = resolveSutExecutable;
        }

        public bool CanHandle(IControlRequest request) => request is StartSutRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as StartSutRequest);

        private async Task<IControlResponse> ExecuteAsync(StartSutRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var executable = request.Executable;
            if (string.IsNullOrWhiteSpace(executable) || File.Exists(executable))
                executable = (await resolveSutExecutable.GetAsync().ConfigureAwait(false)).Executable;

            if (string.IsNullOrWhiteSpace(executable) || !File.Exists(executable))
            {
                return new StartSutResponse
                    {
                        Executable = executable,
                        Success = false,
                    };
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

            return new StartSutResponse
            {
                Executable = executable, Success = true,
            };

            // var result = await command.Task.ConfigureAwait(true);
            // Console.WriteLine(result.StandardOutput);
            // Console.WriteLine(result.StandardError);
        }
    }
}
