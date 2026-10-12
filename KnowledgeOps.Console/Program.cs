
using KnowledgeOps.AI;
using KnowledgeOps.AI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;


var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>(optional: false);
builder.Services.AddKnowledgeOps(builder.Configuration);
using var app = builder.Build();

var chat = app.Services.GetRequiredService<IKnowledgeOpsChatClient>();
var chatHistoty = new ChatHistory("You are a concise, helpful assitant for business app demo");
Console.WriteLine("Type messages. Type exit to quit");
while (true)
{
    Console.WriteLine("User > ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Contains("exit", StringComparison.InvariantCultureIgnoreCase))
    {
        break;
    }

    if (input.Equals("/today", StringComparison.InvariantCultureIgnoreCase))
    {
        var today = await chat.GetCurrentDateAsync();
        Console.WriteLine($"System > {today}");
        continue;
    }
    
    if (input.Equals("/prompt-test", StringComparison.OrdinalIgnoreCase))
    {
        var request = """
                          A department submitted a vendor onboarding request, but the tax document is missing.
                          The requester says the vendor needs access by Friday.
                      """;

        var weakPrompt = $"Summarize this request: {request}";

        var betterPrompt = $"""
                                You are a KnowledgeOps assistant helping an operations analyst.
                                Summarize the request below in 4 bullet points:
                                - Business need
                                - Missing information
                                - Recommended next action
                                - Risk if delayed

                                Request:
                                {request}
                            """;

        Console.WriteLine("Weak Prompt Result:");
        Console.WriteLine(await chat.AskWithPromptAsync(weakPrompt));

        Console.WriteLine();
        Console.WriteLine("Application Prompt Result:");
        Console.WriteLine(await chat.AskWithPromptAsync(betterPrompt));

        continue;
    }

    if (input.Equals("/request-summary", StringComparison.InvariantCultureIgnoreCase))
    {
        var result = await chat.SummarizeRequestAsync(
            "Vendor Onboarding Request",
            "The vendor needs access to Friday, but the tax compliance document is missing.",
            "an operations manager");
        Console.WriteLine($"System > {result}");
        continue;
    }
    chatHistoty.AddUserMessage(input);
    var response = await chat.ReplyAsync(chatHistoty);
    chatHistoty.AddAssistantMessage(response);
    Console.WriteLine($"Assitant > {response}");
}