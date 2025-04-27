using Microsoft.SemanticKernel;

sealed class PromptFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        if (context.Arguments.ContainsName("card_number"))
        {
            context.Arguments["card_number"] = "**** **** **** ****";
        }

        await next(context);

        context.RenderedPrompt += " NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY";
    }
}