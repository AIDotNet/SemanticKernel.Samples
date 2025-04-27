// 3. 开始使用Plugins

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0010

var kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "您的密钥");

kernelBuilder.Plugins.AddFromType<TimeInformation>();

var kernel = kernelBuilder.Build();

// 用提示符调用SK，要求AI提供它无法提供的信息，并可能产生幻觉
Console.WriteLine(await kernel.InvokePromptAsync("离圣诞节还有几天？"));

OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

// 用提示符调用内核，并允许AI自动调用函数
Console.WriteLine(await kernel.InvokePromptAsync("离圣诞节还有几天？解释你的想法", new(settings)));

class TimeInformation
{
    [KernelFunction]
    [Description("获取当前时间帮助用户解决时区不准确。")]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}