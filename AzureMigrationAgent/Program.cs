using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using AzureMigrationAgent.Models;
using AzureMigrationAgent.Services;
using Newtonsoft.Json;

namespace AzureMigrationAgent;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Setup logging
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        var logger = loggerFactory.CreateLogger<Program>();

        Console.WriteLine("=== Azure Migration Analysis Agent ===");
        Console.WriteLine("Initializing Semantic Kernel...\n");

        try
        {
            // Create Kernel with OpenAI
            var kernelBuilder = Kernel.CreateBuilder();
            
            // Check if using Azure OpenAI or OpenAI
            var openAIKey = configuration["OpenAI:ApiKey"];
            var azureOpenAIEndpoint = configuration["Azure:OpenAI:Endpoint"];
            
            if (!string.IsNullOrEmpty(azureOpenAIEndpoint) && azureOpenAIEndpoint != "YOUR_AZURE_OPENAI_ENDPOINT_HERE")
            {
                // Use Azure OpenAI
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: configuration["Azure:OpenAI:DeploymentName"] ?? "gpt-4o",
                    endpoint: azureOpenAIEndpoint,
                    apiKey: configuration["Azure:OpenAI:ApiKey"] ?? throw new InvalidOperationException("Azure OpenAI API key not configured"));
                
                Console.WriteLine("✓ Using Azure OpenAI");
            }
            else if (!string.IsNullOrEmpty(openAIKey) && openAIKey != "YOUR_OPENAI_API_KEY_HERE")
            {
                // Use OpenAI
                kernelBuilder.AddOpenAIChatCompletion(
                    modelId: configuration["OpenAI:ModelId"] ?? "gpt-4o",
                    apiKey: openAIKey);
                
                Console.WriteLine("✓ Using OpenAI");
            }
            else
            {
                Console.WriteLine("❌ Please configure either OpenAI or Azure OpenAI credentials in appsettings.json");
                Console.WriteLine("   Update either 'OpenAI:ApiKey' or 'Azure:OpenAI:Endpoint' and 'Azure:OpenAI:ApiKey'");
                return;
            }

            kernelBuilder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Warning));
            var kernel = kernelBuilder.Build();

            // Create the migration agent
            var migrationAgent = new Services.AzureMigrationAgent(kernel);

            // Load and analyze the application data
            await AnalyzeApplicationDataAsync(migrationAgent, logger);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during analysis");
            Console.WriteLine($"❌ Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static async Task AnalyzeApplicationDataAsync(Services.AzureMigrationAgent migrationAgent, ILogger logger)
    {
        try
        {
            // Load the application data from the JSON file
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "appdata.json");
            
            // If not found in parent directory, look in current directory
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "appdata.json");
            }
            
            // If still not found, prompt user
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine("appdata.json not found in current or parent directory.");
                Console.Write("Please enter the full path to the appdata.json file: ");
                var userPath = Console.ReadLine();
                if (!string.IsNullOrEmpty(userPath) && File.Exists(userPath))
                {
                    jsonPath = userPath;
                }
                else
                {
                    Console.WriteLine("❌ File not found. Exiting.");
                    return;
                }
            }

            Console.WriteLine($"Loading application data from: {jsonPath}");
            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var applicationData = JsonConvert.DeserializeObject<ApplicationData>(jsonContent);

            if (applicationData == null)
            {
                Console.WriteLine("❌ Failed to parse application data from JSON file.");
                return;
            }

            Console.WriteLine($"✓ Loaded application: {applicationData.ApplicationName} ({applicationData.ApplicationAcronym})");
            Console.WriteLine("🔍 Analyzing current infrastructure and generating Azure migration recommendations...\n");

            // Analyze the application
            var recommendation = await migrationAgent.AnalyzeApplicationAsync(applicationData);

            // Display the results
            DisplayResults(recommendation);

            // Save results to file
            await SaveResultsAsync(recommendation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing application data");
            Console.WriteLine($"❌ Error analyzing data: {ex.Message}");
        }
    }

    private static void DisplayResults(AzureRecommendation recommendation)
    {
        Console.WriteLine("" +
            "═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"🏢 AZURE MIGRATION ANALYSIS: {recommendation.ApplicationName}");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════\n");

        // Current State
        Console.WriteLine("📊 CURRENT STATE ARCHITECTURE:");
        Console.WriteLine($"   • Total Servers: {recommendation.CurrentState.TotalServers}");
        Console.WriteLine($"   • Production: {recommendation.CurrentState.ProductionServers} | Non-Production: {recommendation.CurrentState.NonProductionServers}");
        Console.WriteLine($"   • Hosting Model: {recommendation.CurrentState.HostingModel}");
        Console.WriteLine($"   • Technologies: {string.Join(", ", recommendation.CurrentState.Technologies)}");
        Console.WriteLine($"   • Business Criticality: {recommendation.CurrentState.BusinessCriticality}");
        
        if (recommendation.CurrentState.ServerSpecifications.Any())
        {
            Console.WriteLine("\n   Server Specifications:");
            foreach (var server in recommendation.CurrentState.ServerSpecifications)
            {
                Console.WriteLine($"     ▪ {server.ServerName} ({server.Environment}): {server.Cores} cores, {server.MemoryMB/1024}GB RAM, {server.DiskGB}GB disk");
            }
        }

        // Target State
        Console.WriteLine("\n🎯 TARGET STATE AZURE ARCHITECTURE:");
        Console.WriteLine($"   • Recommended Region: {recommendation.TargetState.Region}");
        Console.WriteLine("   • Recommended Azure Services:");
        
        foreach (var service in recommendation.TargetState.RecommendedServices)
        {
            Console.WriteLine($"     ▪ {service.ServiceName} ({service.SKU})");
            Console.WriteLine($"       - Size: {service.Size}");
            Console.WriteLine($"       - Resources: {service.RecommendedCores} vCPUs, {service.RecommendedMemoryGB}GB RAM, {service.RecommendedStorageGB}GB storage");
            Console.WriteLine($"       - Estimated Cost: ${service.EstimatedMonthlyCostUSD:F2}/month");
            Console.WriteLine($"       - Justification: {service.Justification}");
            Console.WriteLine();
        }

        // Key Recommendations
        Console.WriteLine("💡 KEY RECOMMENDATIONS:");
        foreach (var recommendation_item in recommendation.KeyRecommendations)
        {
            Console.WriteLine($"   • {recommendation_item}");
        }

        // Migration Complexity
        Console.WriteLine("\n⚙️ MIGRATION COMPLEXITY ASSESSMENT:");
        Console.WriteLine($"   • Overall Complexity: {recommendation.Complexity.OverallComplexity}");
        Console.WriteLine($"   • Estimated Timeframe: {recommendation.Complexity.EstimatedTimeframe}");
        Console.WriteLine("   • Complexity Factors:");
        foreach (var factor in recommendation.Complexity.ComplexityFactors)
        {
            Console.WriteLine($"     ▪ {factor}");
        }
        
        Console.WriteLine("   • Prerequisites:");
        foreach (var prereq in recommendation.Complexity.Prerequisites)
        {
            Console.WriteLine($"     ▪ {prereq}");
        }

        // Cost Estimates
        Console.WriteLine("\n💰 COST ESTIMATES (USD):");
        Console.WriteLine($"   • Monthly Compute: ${recommendation.EstimatedCosts.MonthlyComputeCost:F2}");
        Console.WriteLine($"   • Monthly Storage: ${recommendation.EstimatedCosts.MonthlyStorageCost:F2}");
        Console.WriteLine($"   • Monthly Networking: ${recommendation.EstimatedCosts.MonthlyNetworkingCost:F2}");
        Console.WriteLine($"   • Total Monthly: ${recommendation.EstimatedCosts.TotalMonthlyCost:F2}");
        Console.WriteLine($"   • Annual Cost: ${recommendation.EstimatedCosts.AnnualCost:F2}");
        Console.WriteLine($"   • Migration Cost: ${recommendation.EstimatedCosts.MigrationCost:F2}");
        Console.WriteLine($"   • Optimization Tips: {recommendation.EstimatedCosts.CostOptimizationTips}");

        // Security
        if (recommendation.TargetState.Security.IdentityAndAccess.Any())
        {
            Console.WriteLine("\n🔒 SECURITY RECOMMENDATIONS:");
            Console.WriteLine("   Identity & Access:");
            foreach (var item in recommendation.TargetState.Security.IdentityAndAccess)
            {
                Console.WriteLine($"     ▪ {item}");
            }
            
            if (recommendation.TargetState.Security.NetworkSecurity.Any())
            {
                Console.WriteLine("   Network Security:");
                foreach (var item in recommendation.TargetState.Security.NetworkSecurity)
                {
                    Console.WriteLine($"     ▪ {item}");
                }
            }
            
            if (recommendation.TargetState.Security.DataProtection.Any())
            {
                Console.WriteLine("   Data Protection:");
                foreach (var item in recommendation.TargetState.Security.DataProtection)
                {
                    Console.WriteLine($"     ▪ {item}");
                }
            }
        }

        // Risks and Considerations
        Console.WriteLine("\n⚠️ RISKS AND CONSIDERATIONS:");
        foreach (var risk in recommendation.RisksAndConsiderations)
        {
            Console.WriteLine($"   • {risk}");
        }

        Console.WriteLine("\n═══════════════════════════════════════════════════════════════════════════════");
    }

    private static async Task SaveResultsAsync(AzureRecommendation recommendation)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"AzureMigrationAnalysis_{recommendation.ApplicationName.Replace(" ", "")}_{timestamp}.json";
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), filename);
            
            var json = JsonConvert.SerializeObject(recommendation, Formatting.Indented);
            await File.WriteAllTextAsync(filepath, json);
            
            Console.WriteLine($"📁 Analysis results saved to: {filepath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Could not save results to file: {ex.Message}");
        }
    }
}
