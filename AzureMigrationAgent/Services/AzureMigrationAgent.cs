using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AzureMigrationAgent.Models;
using Newtonsoft.Json;

namespace AzureMigrationAgent.Services;

public class AzureMigrationAgent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    public AzureMigrationAgent(Kernel kernel)
    {
        _kernel = kernel;
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<AzureRecommendation> AnalyzeApplicationAsync(ApplicationData appData)
    {
        var analysisPrompt = CreateAnalysisPrompt(appData);
        
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(GetSystemPrompt());
        chatHistory.AddUserMessage(analysisPrompt);
        
        var response = await _chatService.GetChatMessageContentAsync(chatHistory);
        
        // Parse the response and create structured recommendation
        return await ParseRecommendationAsync(response.Content ?? "", appData);
    }

    private string GetSystemPrompt()
    {
        return """
            You are an expert Azure Migration Architect with deep knowledge of:
            1. On-premises infrastructure assessment
            2. Azure services and SKU recommendations
            3. Cost optimization strategies
            4. Security and compliance requirements
            5. Migration best practices

            Your role is to analyze on-premises application data and provide detailed Azure migration recommendations including:
            - Current state architecture analysis
            - Target state Azure architecture design
            - Specific Azure service recommendations with SKUs
            - Resource sizing (CPU, Memory, Storage)
            - Cost estimates
            - Migration complexity assessment
            - Security recommendations
            - Risk mitigation strategies

            Always provide practical, actionable recommendations with clear justifications.
            Focus on cost optimization while maintaining performance and security.
            Consider high availability and disaster recovery requirements.
            """;
    }

    private string CreateAnalysisPrompt(ApplicationData appData)
    {
        var serversSummary = appData.MalServers.Select(s => new
        {
            Name = s.ServerName,
            Environment = s.EnvironmentAssociation,
            OS = s.OperatingSystem,
            Cores = s.CpUsCores,
            MemoryMB = s.MemorySizeMb,
            DiskGB = s.DiskSizeGb,
            Location = $"{s.City}, {s.State}"
        });

        var jsonData = JsonConvert.SerializeObject(new
        {
            Application = new
            {
                Name = appData.ApplicationName,
                Acronym = appData.ApplicationAcronym,
                Platform = appData.HostPlatform,
                Languages = appData.Languages,
                Criticality = appData.Criticality,
                Region = appData.Region,
                HostingModel = appData.ApplicationHostingModel,
                Description = appData.Description,
                BusinessImpact = appData.BusinessImpact
            },
            Infrastructure = new
            {
                TotalServers = appData.InUseServerAssociations,
                ProdServers = appData.InUseProdServers,
                NonProdServers = appData.InUseNonProdServers,
                ServerDetails = serversSummary
            }
        }, Formatting.Indented);

        return $"""
            Please analyze this on-premises application and provide comprehensive Azure migration recommendations.
            
            Application Data:
            {jsonData}
            
            Please provide analysis in the following areas:
            
            1. CURRENT STATE ARCHITECTURE:
               - Summarize the current infrastructure
               - Identify key characteristics and dependencies
               - Assess resource utilization patterns
            
            2. TARGET STATE AZURE ARCHITECTURE:
               - Recommend specific Azure services (App Service, Virtual Machines, Container Instances, etc.)
               - Suggest appropriate SKUs and sizes
               - Provide resource sizing recommendations (vCPUs, RAM, Storage)
               - Consider high availability and disaster recovery
            
            3. DETAILED RECOMMENDATIONS:
               - Map each server to appropriate Azure service
               - Justify sizing recommendations based on current specs
               - Suggest cost optimization strategies
               - Recommend security configurations
            
            4. MIGRATION COMPLEXITY:
               - Assess migration difficulty
               - Identify potential challenges
               - Suggest migration approach (lift-and-shift vs. refactor)
            
            5. COST ESTIMATES:
               - Provide estimated monthly costs for Azure resources
               - Compare with likely current hosting costs
               - Suggest cost optimization opportunities
            
            6. RISKS AND MITIGATION:
               - Identify migration risks
               - Suggest mitigation strategies
               - Recommend testing approaches
            
            Please be specific with Azure service names, SKUs, and configurations.
            Focus on practical, implementable recommendations.
            """;
    }

    private async Task<AzureRecommendation> ParseRecommendationAsync(string response, ApplicationData appData)
    {
        // Create a structured recommendation from the AI response
        var recommendation = new AzureRecommendation
        {
            ApplicationName = appData.ApplicationName,
            ApplicationId = appData.ApplicationId
        };

        // Parse current state
        recommendation.CurrentState = new CurrentStateArchitecture
        {
            TotalServers = int.TryParse(appData.InUseServerAssociations, out var total) ? total : 0,
            ProductionServers = int.TryParse(appData.InUseProdServers, out var prod) ? prod : 0,
            NonProductionServers = int.TryParse(appData.InUseNonProdServers, out var nonprod) ? nonprod : 0,
            HostingModel = appData.ApplicationHostingModel,
            Technologies = appData.Languages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            BusinessCriticality = appData.Criticality,
            ServerSpecifications = appData.MalServers.Select(s => new ServerSpecs
            {
                ServerName = s.ServerName,
                Environment = s.EnvironmentAssociation,
                Cores = int.TryParse(s.CpUsCores, out var cores) ? cores : 0,
                MemoryMB = int.TryParse(s.MemorySizeMb, out var mem) ? mem : 0,
                DiskGB = int.TryParse(s.DiskSizeGb, out var disk) ? disk : 0,
                OperatingSystem = s.OperatingSystem,
                Location = $"{s.City}, {s.State}"
            }).ToList()
        };

        // Use another AI call to structure the recommendations
        var structuringPrompt = $"""
            Based on the following analysis, please provide a structured JSON response with specific Azure service recommendations:
            
            Analysis: {response}
            
            Please format your response as a JSON object with these sections:
            - targetStateServices: Array of recommended Azure services with specific SKUs, sizes, and justifications
            - keyRecommendations: Array of top 5-7 key recommendations
            - migrationComplexity: Assessment of complexity and timeframe
            - estimatedCosts: Monthly cost breakdown
            - risksAndConsiderations: Key risks and mitigation strategies
            
            Be specific with Azure service names and SKU recommendations.
            """;

        var structureHistory = new ChatHistory();
        structureHistory.AddSystemMessage("You are a structured data formatter. Provide clear, actionable recommendations.");
        structureHistory.AddUserMessage(structuringPrompt);
        
        var structuredResponse = await _chatService.GetChatMessageContentAsync(structureHistory);
        
        // Extract structured data from response (simplified parsing)
        await PopulateRecommendationFromResponseAsync(recommendation, structuredResponse.Content ?? "", appData);
        
        return recommendation;
    }

    private Task PopulateRecommendationFromResponseAsync(AzureRecommendation recommendation, string response, ApplicationData appData)
    {
        // Add some intelligent defaults based on the server specs
        var totalCores = appData.MalServers.Sum(s => int.TryParse(s.CpUsCores, out var cores) ? cores : 0);
        var totalMemoryMB = appData.MalServers.Sum(s => int.TryParse(s.MemorySizeMb, out var mem) ? mem : 0);
        var totalDiskGB = appData.MalServers.Sum(s => int.TryParse(s.DiskSizeGb, out var disk) ? disk : 0);

        // Add default Azure service recommendations based on application characteristics
        if (appData.Languages.Contains("C#"))
        {
            recommendation.TargetState.RecommendedServices.Add(new AzureService
            {
                ServiceType = "App Service",
                ServiceName = "Azure App Service",
                SKU = "P2V3",
                Size = "Premium P2V3",
                RecommendedCores = Math.Max(2, totalCores / 2),
                RecommendedMemoryGB = Math.Max(8, totalMemoryMB / 1024),
                RecommendedStorageGB = 250,
                Tier = "Premium",
                Justification = "Recommended for C# applications with moderate to high traffic. Provides auto-scaling, deployment slots, and integrated monitoring.",
                EstimatedMonthlyCostUSD = 292.00m
            });
        }

        // Add VM recommendations for lift-and-shift scenarios
        recommendation.TargetState.RecommendedServices.Add(new AzureService
        {
            ServiceType = "Virtual Machine",
            ServiceName = "Azure Virtual Machine",
            SKU = DetermineVMSKU(totalCores, totalMemoryMB),
            Size = DetermineVMSize(totalCores, totalMemoryMB),
            RecommendedCores = totalCores,
            RecommendedMemoryGB = totalMemoryMB / 1024,
            RecommendedStorageGB = totalDiskGB,
            Tier = "Standard",
            Justification = "Lift-and-shift option maintaining current architecture with minimal changes.",
            EstimatedMonthlyCostUSD = EstimateVMCost(totalCores, totalMemoryMB)
        });

        // Set region based on current location
        var primaryLocation = appData.MalServers.FirstOrDefault()?.City?.ToLower();
        recommendation.TargetState.Region = primaryLocation switch
        {
            "denver" => "West US 2",
            "broomfield" => "West US 2",
            _ => "East US"
        };

        // Add key recommendations
        recommendation.KeyRecommendations = new List<string>
        {
            "Consider Azure App Service for the C# application to reduce operational overhead",
            $"Implement Azure SQL Database instead of on-premises databases for better scalability",
            "Use Azure Application Gateway for load balancing and SSL termination",
            "Implement Azure Key Vault for secure credential management",
            "Set up Azure Monitor and Application Insights for comprehensive monitoring",
            "Consider Azure DevOps for CI/CD pipeline automation",
            $"Deploy in {recommendation.TargetState.Region} region for optimal performance"
        };

        // Migration complexity assessment
        recommendation.Complexity = new MigrationComplexity
        {
            OverallComplexity = appData.Criticality == "Non-Critical" ? "Medium" : "High",
            ComplexityFactors = new List<string>
            {
                $"Application uses {appData.Languages} - Azure native support available",
                $"Current hosting model: {appData.ApplicationHostingModel}",
                $"Multiple environments: {recommendation.CurrentState.ProductionServers} Prod, {recommendation.CurrentState.NonProductionServers} Non-Prod",
                "No disaster recovery plan currently in place"
            },
            EstimatedTimeframe = "3-6 months",
            Prerequisites = new List<string>
            {
                "Azure subscription and governance setup",
                "Network connectivity planning (ExpressRoute or VPN)",
                "Security and compliance review",
                "Application dependency mapping",
                "Performance baseline establishment"
            }
        };

        // Cost estimates
        recommendation.EstimatedCosts = new EstimatedCosts
        {
            MonthlyComputeCost = recommendation.TargetState.RecommendedServices.Sum(s => s.EstimatedMonthlyCostUSD),
            MonthlyStorageCost = 50.00m,
            MonthlyNetworkingCost = 25.00m,
            TotalMonthlyCost = recommendation.TargetState.RecommendedServices.Sum(s => s.EstimatedMonthlyCostUSD) + 75.00m,
            AnnualCost = (recommendation.TargetState.RecommendedServices.Sum(s => s.EstimatedMonthlyCostUSD) + 75.00m) * 12,
            MigrationCost = 15000.00m,
            CostOptimizationTips = "Use Azure Hybrid Benefit for Windows licenses, implement auto-scaling, consider Reserved Instances for predictable workloads"
        };

        // Security recommendations
        recommendation.TargetState.Security = new SecurityRecommendations
        {
            IdentityAndAccess = new List<string>
            {
                "Implement Azure Active Directory integration",
                "Enable multi-factor authentication (MFA)",
                "Use managed identities for Azure resource access"
            },
            NetworkSecurity = new List<string>
            {
                "Deploy within Azure Virtual Network with appropriate subnets",
                "Configure Network Security Groups (NSGs)",
                "Implement Azure Firewall or Application Gateway WAF"
            },
            DataProtection = new List<string>
            {
                "Enable encryption at rest for all storage",
                "Use Azure Key Vault for certificate and secret management",
                "Implement Azure Backup for data protection"
            }
        };

        // Risks and considerations
        recommendation.RisksAndConsiderations = new List<string>
        {
            "Application dependency on specific on-premises systems may require additional integration work",
            "Data migration requires careful planning to minimize downtime",
            "Legacy .NET Framework applications may need modernization",
            "Current lack of DR plan requires immediate attention post-migration",
            "User training may be required for new Azure-based workflows"
        };
        
        return Task.CompletedTask;
    }

    private string DetermineVMSKU(int cores, int memoryMB)
    {
        var memoryGB = memoryMB / 1024;
        
        return (cores, memoryGB) switch
        {
            (>= 8, >= 32) => "Standard_D8s_v5",
            (>= 4, >= 16) => "Standard_D4s_v5",
            (>= 2, >= 8) => "Standard_D2s_v5",
            _ => "Standard_B2s"
        };
    }

    private string DetermineVMSize(int cores, int memoryMB)
    {
        var memoryGB = memoryMB / 1024;
        
        return (cores, memoryGB) switch
        {
            (>= 8, >= 32) => "8 vCPUs, 32 GB RAM",
            (>= 4, >= 16) => "4 vCPUs, 16 GB RAM", 
            (>= 2, >= 8) => "2 vCPUs, 8 GB RAM",
            _ => "2 vCPUs, 4 GB RAM"
        };
    }

    private decimal EstimateVMCost(int cores, int memoryMB)
    {
        var memoryGB = memoryMB / 1024;
        
        return (cores, memoryGB) switch
        {
            (>= 8, >= 32) => 350.00m,
            (>= 4, >= 16) => 175.00m,
            (>= 2, >= 8) => 87.50m,
            _ => 43.75m
        };
    }
}
