using KnowledgeOps.AI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

namespace KnowledgeOps.AI;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKnowledgeOps(this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configuration Options Patterns
        services.AddOptions<OllamaAIOptions>()
            .Bind(configuration.GetSection("OllamaAI"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // Configuration DI
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaAIOptions>>().Value;
            
            var builder = Kernel.CreateBuilder();
            builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information));
            
            // Set Configuration Ollama
            builder.AddOllamaChatCompletion(
                modelId: options.DeploymentName, 
                endpoint: new Uri(options.Endpoint)
            );
            builder.Plugins.AddFromType<TimePlugin>("Time");
            builder.Plugins.AddFromType<ConversationSummaryPlugin>("Summarization");
            return builder.Build();
        });

        services.AddSingleton(sp =>
            sp.GetRequiredService<Kernel>()
                .GetRequiredService<IChatCompletionService>());
        
        services.AddSingleton<IKnowledgeOpsChatClient, KnowledgeOpsChatClient>();
        
        return services;
    }
}