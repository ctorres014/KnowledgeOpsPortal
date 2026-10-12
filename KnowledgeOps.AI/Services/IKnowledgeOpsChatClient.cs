using KnowledgeOps.AI.Prompts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace KnowledgeOps.AI.Services;

public interface IKnowledgeOpsChatClient
{
    Task<string> ReplyAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default);
    Task<string> GetCurrentDateAsync(CancellationToken cancellationToken = default);
    Task<string> AskWithPromptAsync(string prompt, CancellationToken cancellationToken = default);
    Task<string> SummarizeRequestAsync(string requestTitle,
        string requestDetails,
        string audience,
        CancellationToken cancellationToken = default);
}

internal sealed class KnowledgeOpsChatClient(
    IChatCompletionService chatCompletionService,
    Kernel kernel) : IKnowledgeOpsChatClient
{
    public async Task<string> ReplyAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default)
    {
        var setting = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        var response = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: setting,
            kernel: kernel,
            cancellationToken: cancellationToken);
        return response.Content ?? string.Empty;
    }

    public async Task<string> GetCurrentDateAsync(CancellationToken cancellationToken = default)
    {
        var result = await kernel.InvokeAsync("Time", 
            "Today", cancellationToken: cancellationToken);
        return result.ToString();
    }

    public async Task<string> AskWithPromptAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var result = await kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.ToString();
    }

    public async Task<string> SummarizeRequestAsync(string requestTitle, string requestDetails, string audience,
        CancellationToken cancellationToken = default)
    {
        var arguments = new KernelArguments()
        {
            ["requestTitle"] = requestTitle,
            ["requestDetails"] = requestDetails,
            ["audience"] = audience
        };
        
        var result = await kernel.InvokePromptAsync(
            KnowledgeOpsPromptTemplates.RequestSummary,
            arguments,
            cancellationToken: cancellationToken);

        return result.ToString();
    }
}