using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows;


// Create the executors
UppercaseExecutor uppercase = new();
ReverseTextExecutor reverse = new();

// Build the workflow by connecting executors sequentially
WorkflowBuilder builder = new(uppercase);

builder
    .AddEdge(uppercase, reverse)
    .WithOutputFrom(reverse);

var workflow = builder.Build();

// Execute the workflow with input data
await using Run run = await InProcessExecution.RunAsync(workflow, "!dlrow yadved, olleh");


foreach (WorkflowEvent evt in run.NewEvents)
{
    switch (evt)
    {
        case ExecutorCompletedEvent executorComplete:
            Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            break;
        case WorkflowOutputEvent workflowOutput:
            Console.WriteLine($"Workflow '{workflowOutput.SourceId}' outputs: {workflowOutput.Data}");
            break;
    }
}