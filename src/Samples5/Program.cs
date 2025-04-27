using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010

var kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "你的密钥");

// Add prompt filter to the kernel
kernelBuilder.Services.AddSingleton<IPromptRenderFilter, PromptFilter>();

var kernel = kernelBuilder.Build();

KernelArguments arguments = new() { { "card_number", "4444 3333 2222 1111" } };

var result =
    await kernel.InvokePromptAsync("告诉我有关 {{$card_number}} 这个信用卡的信息？",
        arguments);

Console.WriteLine(result);