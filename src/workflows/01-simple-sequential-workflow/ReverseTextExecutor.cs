using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Agents.AI.Workflows;

/// <summary>
/// Second executor: reverses the input text and completes the workflow.
/// </summary>
internal sealed class ReverseTextExecutor()
    : ReflectingExecutor<ReverseTextExecutor>("ReverseTextExecutor"),
      IMessageHandler<string, string>
{
    public ValueTask<string> HandleAsync(
        string input,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        // Reverse the input text
        return ValueTask.FromResult(new string([.. input.Reverse()]));
    }
}