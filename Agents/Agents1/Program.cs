using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Assistants;
using OpenAI.Chat;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

#pragma warning disable OPENAI001
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110

const string parrotName = "Parrot";
const string parrotInstructions = "用海盗的声音重复用户信息，然后用鹦鹉的声音结束。";

var kernel = CreateKernelWithChatCompletion("gpt-4.1", "https://api.token-ai.cn/v1",
    "您的密钥");
// Define the agent
ChatCompletionAgent agent =
    new()
    {
        Name = parrotName,
        Instructions = parrotInstructions,
        Kernel = kernel,
    };

// Respond to user input
await InvokeAgentAsync("幸运的海盗");
await InvokeAgentAsync("我来了，我看见了。");
await InvokeAgentAsync("熟能生巧。");

// Local function to invoke agent and display the conversation messages.
async Task InvokeAgentAsync(string input)
{
    ChatMessageContent message = new(AuthorRole.User, input);
    WriteAgentChatMessage(message);

    await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message))
    {
        WriteAgentChatMessage(response);
    }
}

Kernel CreateKernelWithChatCompletion(string model, string endpoint, string apiKey)
{
    var builder = Kernel.CreateBuilder();

    builder.AddOpenAIChatCompletion(
        model, new Uri(endpoint), apiKey);

    return builder.Build();
}

void WriteAgentChatMessage(ChatMessageContent message)
{
    // Include ChatMessageContent.AuthorName in output, if present.
    string authorExpression = message.Role == AuthorRole.User ? string.Empty : $" - {message.AuthorName ?? "*"}";
    // Include TextContent (via ChatMessageContent.Content), if present.
    string contentExpression = string.IsNullOrWhiteSpace(message.Content) ? string.Empty : message.Content;
    bool isCode = message.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false;
    string codeMarker = isCode ? "\n  [CODE]\n" : " ";
    Console.WriteLine($"\n# {message.Role}{authorExpression}:{codeMarker}{contentExpression}");

    // Provide visibility for inner content (that isn't TextContent).
    foreach (KernelContent item in message.Items)
    {
        if (item is AnnotationContent annotation)
        {
            if (annotation.Kind == AnnotationKind.UrlCitation)
            {
                Console.WriteLine(
                    $"  [{item.GetType().Name}] {annotation.Label}: {annotation.ReferenceId} - {annotation.Title}");
            }
            else
            {
                Console.WriteLine($"  [{item.GetType().Name}] {annotation.Label}: File #{annotation.ReferenceId}");
            }
        }
        else if (item is FileReferenceContent fileReference)
        {
            Console.WriteLine($"  [{item.GetType().Name}] File #{fileReference.FileId}");
        }
        else if (item is ImageContent image)
        {
            Console.WriteLine(
                $"  [{item.GetType().Name}] {image.Uri?.ToString() ?? image.DataUri ?? $"{image.Data?.Length} bytes"}");
        }
        else if (item is FunctionCallContent functionCall)
        {
            Console.WriteLine($"  [{item.GetType().Name}] {functionCall.Id}");
        }
        else if (item is FunctionResultContent functionResult)
        {
            Console.WriteLine(
                $"  [{item.GetType().Name}] {functionResult.CallId} - {JsonSerializer.Serialize(functionResult.Result, JsonSerializerOptions.Default) ?? "*"}");
        }
    }

    if (message.Metadata?.TryGetValue("Usage", out object? usage) ?? false)
    {
        if (usage is RunStepTokenUsage assistantUsage)
        {
            WriteUsage(assistantUsage.TotalTokenCount, assistantUsage.InputTokenCount, assistantUsage.OutputTokenCount);
        }
        else if (usage is ChatTokenUsage chatUsage)
        {
            WriteUsage(chatUsage.TotalTokenCount, chatUsage.InputTokenCount, chatUsage.OutputTokenCount);
        }
    }

    void WriteUsage(long totalTokens, long inputTokens, long outputTokens)
    {
        Console.WriteLine($"  [Usage] Tokens: {totalTokens}, Input: {inputTokens}, Output: {outputTokens}");
    }
}