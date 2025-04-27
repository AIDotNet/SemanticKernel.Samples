using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "您的密钥")
    .Build();

await foreach (var item in kernel.InvokePromptStreamingAsync("您好，我是TokenAI"))
{
    Console.Write(item.ToString());
};