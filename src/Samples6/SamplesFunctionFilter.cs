using System.Text.Json;
using Microsoft.SemanticKernel;

namespace Samples6;

public class SamplesFunctionFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"Invoking {context.Function.Name}");

        await next(context);

        var metadata = context.Result?.Metadata;

        if (metadata is not null && metadata.TryGetValue("Usage", out var usage))
        {
            Console.WriteLine($"Token usage: {JsonSerializer.Serialize(usage)}");
        }
    }
}