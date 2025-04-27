using Microsoft.SemanticKernel;
using Samples4;

#pragma warning disable SKEXP0010

var kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4.1-mini", new Uri("https://api.token-ai.cn/v1"),
        "您的密钥");
        
var kernel = kernelBuilder.Build();


var generateStoryYaml = EmbeddedResource.Read("Resources.GenerateStory.yaml");
var function = kernel.CreateFunctionFromPromptYaml(generateStoryYaml);

// Invoke the prompt function and display the result
Console.WriteLine(await kernel.InvokeAsync(function, arguments: new()
{
    { "topic", "Dog" },
    { "length", "3" },
}));
