using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Samples6;

#pragma warning disable SKEXP0010

var kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "您的密钥");

kernelBuilder.Plugins.AddFromType<TimeInformation>();
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, SamplesFunctionFilter>();


Kernel kernel = kernelBuilder.Build();

// Invoke the kernel with a prompt and allow the AI to automatically invoke functions
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
Console.WriteLine(await kernel.InvokePromptAsync("离圣诞节还有几天？解释你的想法。", new(settings)));

/// <summary>
/// A plugin that returns the current time.
/// </summary>
sealed class TimeInformation
{
    [KernelFunction]
    [Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}