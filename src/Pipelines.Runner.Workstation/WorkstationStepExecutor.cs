using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;
using Pipelines.Runner.Workstation.Execution;

namespace Pipelines.Runner.Workstation;

public sealed class WorkstationStepExecutor : IStepExecutor
{
    private readonly WorkstationExecutionOptions _options;
    private readonly WorkstationExecutionPolicy _policy;
    private readonly WorkstationScriptWriter _scriptWriter;
    private readonly WorkstationEnvironmentBuilder _environmentBuilder;
    private readonly WorkstationProcessRunner _processRunner;

    public WorkstationStepExecutor()
        : this(new WorkstationExecutionOptions())
    {
    }

    public WorkstationStepExecutor(WorkstationExecutionOptions options)
        : this(
            options,
            new WorkstationExecutionPolicy(),
            new WorkstationScriptWriter(),
            new WorkstationEnvironmentBuilder(options),
            new WorkstationProcessRunner())
    {
    }

    public WorkstationStepExecutor(
        WorkstationExecutionOptions options,
        WorkstationExecutionPolicy policy,
        WorkstationScriptWriter scriptWriter,
        WorkstationEnvironmentBuilder environmentBuilder,
        WorkstationProcessRunner processRunner)
    {
        _options = options;
        _policy = policy;
        _scriptWriter = scriptWriter;
        _environmentBuilder = environmentBuilder;
        _processRunner = processRunner;
    }

    public async Task<StepResult> ExecuteAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        _policy.Validate(context);

        await using var scope = await WorkstationExecutionScope.CreateAsync(
            context,
            _options,
            cancellationToken);

        var command = await _scriptWriter.CreateCommandAsync(
            scope,
            context.Step,
            cancellationToken);

        var environment = _environmentBuilder.Build(scope, context);
        var workingDirectory = WorkstationPathResolver.ResolveWorkingDirectory(
            scope.WorkspacePath,
            context.Step.WorkingDirectory);

        var processResult = await _processRunner.RunAsync(
            new WorkstationProcessSpec
            {
                StepName = context.Step.Name,
                FileName = command.FileName,
                Arguments = command.Arguments,
                WorkingDirectory = workingDirectory,
                Environment = environment,
                TimeoutMinutes = context.Step.TimeoutMinutes
            },
            cancellationToken);

        return processResult.ToStepResult(context.Step);
    }
}
