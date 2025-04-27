// 2. 使用聊天完成对话示例

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0010

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "您的密钥")
    .Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory history = [];

history.AddUserMessage("Hello, how are you?");

// 使用同步对话
var response = await chatCompletionService.GetChatMessageContentAsync(
    history,
    kernel: kernel
);

Console.WriteLine(response.Content);


// 使用异步对话
await foreach (var item in chatCompletionService.GetStreamingChatMessageContentsAsync(
                   history,
                   kernel: kernel
               ))
{
    Console.Write(item.Content);
}