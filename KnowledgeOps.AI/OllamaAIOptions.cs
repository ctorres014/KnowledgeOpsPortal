using System.ComponentModel.DataAnnotations;

namespace KnowledgeOps.AI;

public class OllamaAIOptions
{
    public const string SectionName = "OllamaAI";
    [Required]
    public string DeploymentName { get; set; } = string.Empty;
    [Required]
    public string Endpoint { get; set; } = string.Empty;
    
    public string ApiKey { get; set; } = string.Empty;
}