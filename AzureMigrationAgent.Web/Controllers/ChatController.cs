using Microsoft.AspNetCore.Mvc;
using AzureMigrationAgent.Services;
using AzureMigrationAgent.Models;
using Newtonsoft.Json;
using System.Text;

namespace AzureMigrationAgent.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly Services.AzureMigrationAgent _migrationAgent;
        private readonly ILogger<ChatController> _logger;

        public ChatController(Services.AzureMigrationAgent migrationAgent, ILogger<ChatController> logger)
        {
            _migrationAgent = migrationAgent;
            _logger = logger;
        }

        [HttpPost("upload-and-analyze")]
        public async Task<IActionResult> UploadAndAnalyze(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file uploaded" });
                }

                if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { error = "Please upload a JSON file" });
                }

                // Read the uploaded file
                string jsonContent;
                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    jsonContent = await reader.ReadToEndAsync();
                }

                // Parse the JSON into ApplicationData
                var applicationData = JsonConvert.DeserializeObject<ApplicationData>(jsonContent);
                if (applicationData == null)
                {
                    return BadRequest(new { error = "Invalid JSON format" });
                }

                _logger.LogInformation("Analyzing application: {ApplicationName}", applicationData.ApplicationName);

                // Perform the analysis using your existing agent
                var recommendation = await _migrationAgent.AnalyzeApplicationAsync(applicationData);

                // Store the analysis in session for follow-up questions
                HttpContext.Session.SetString("lastAnalysis", JsonConvert.SerializeObject(recommendation));
                HttpContext.Session.SetString("lastApplicationData", jsonContent);

                return Ok(new
                {
                    type = "analysis",
                    message = $"✅ **Analysis Complete for {applicationData.ApplicationName}**\n\n" +
                             GenerateAnalysisSummary(recommendation),
                    data = recommendation,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing uploaded file");
                return StatusCode(500, new { error = "An error occurred while analyzing the file", details = ex.Message });
            }
        }

        [HttpPost("ask-question")]
        public async Task<IActionResult> AskQuestion([FromBody] ChatMessage message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.Question))
                {
                    return BadRequest(new { error = "Question cannot be empty" });
                }

                // Get the previous analysis from session
                var lastAnalysisJson = HttpContext.Session.GetString("lastAnalysis");
                var lastApplicationDataJson = HttpContext.Session.GetString("lastApplicationData");

                if (string.IsNullOrEmpty(lastAnalysisJson))
                {
                    return BadRequest(new { error = "No previous analysis found. Please upload and analyze a JSON file first." });
                }

                var lastAnalysis = JsonConvert.DeserializeObject<AzureRecommendation>(lastAnalysisJson);
                var lastApplicationData = JsonConvert.DeserializeObject<ApplicationData>(lastApplicationDataJson);

                _logger.LogInformation("Processing follow-up question: {Question}", message.Question);

                // Create a context-aware prompt for the follow-up question
                var contextualPrompt = CreateFollowUpPrompt(message.Question, lastAnalysis, lastApplicationData);

                // Use the migration agent to answer the question
                var answer = await _migrationAgent.AnswerFollowUpQuestionAsync(contextualPrompt, lastAnalysis);

                return Ok(new
                {
                    type = "answer",
                    message = answer,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing question: {Question}", message.Question);
                return StatusCode(500, new { error = "An error occurred while processing your question", details = ex.Message });
            }
        }

        [HttpPost("clear-session")]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Session cleared. You can now analyze a new application." });
        }

        [HttpGet("session-status")]
        public IActionResult GetSessionStatus()
        {
            var hasAnalysis = !string.IsNullOrEmpty(HttpContext.Session.GetString("lastAnalysis"));
            var applicationName = "";

            if (hasAnalysis)
            {
                var lastApplicationDataJson = HttpContext.Session.GetString("lastApplicationData");
                if (!string.IsNullOrEmpty(lastApplicationDataJson))
                {
                    var appData = JsonConvert.DeserializeObject<ApplicationData>(lastApplicationDataJson);
                    applicationName = appData?.ApplicationName ?? "";
                }
            }

            return Ok(new
            {
                hasAnalysis,
                applicationName,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("export-data")]
        public IActionResult ExportData()
        {
            var lastAnalysisJson = HttpContext.Session.GetString("lastAnalysis");
            
            if (string.IsNullOrEmpty(lastAnalysisJson))
            {
                return BadRequest(new { error = "No analysis data available to export" });
            }

            var lastAnalysis = JsonConvert.DeserializeObject<AzureRecommendation>(lastAnalysisJson);
            
            return Ok(new
            {
                analysis = lastAnalysis,
                exportDate = DateTime.UtcNow,
                type = "Azure Migration Analysis"
            });
        }

        private string GenerateAnalysisSummary(AzureRecommendation recommendation)
        {
            var summary = new StringBuilder();
            
            summary.AppendLine($"**📊 Current State:**");
            summary.AppendLine($"• Servers: {recommendation.CurrentState?.TotalServers ?? 0}");
            
            var totalCores = recommendation.CurrentState?.ServerSpecifications?.Sum(s => s.Cores) ?? 0;
            var totalMemoryGB = recommendation.CurrentState?.ServerSpecifications?.Sum(s => s.MemoryMB / 1024) ?? 0;
            
            summary.AppendLine($"• Total Cores: {totalCores}");
            summary.AppendLine($"• Total Memory: {totalMemoryGB}GB");
            summary.AppendLine();

            summary.AppendLine($"**🎯 Azure Recommendations:**");
            summary.AppendLine($"• Migration Complexity: {recommendation.Complexity?.OverallComplexity ?? "Not specified"}");
            summary.AppendLine($"• Estimated Timeline: {recommendation.Complexity?.EstimatedTimeframe ?? "Not specified"}");
            summary.AppendLine($"• Monthly Cost Estimate: ${recommendation.EstimatedCosts?.TotalMonthlyCost ?? 0:F2}");
            summary.AppendLine();

            summary.AppendLine($"**💬 Ask me questions like:**");
            summary.AppendLine($"• \"What about security considerations?\"");
            summary.AppendLine($"• \"Can you break down the costs?\"");
            summary.AppendLine($"• \"What migration risks should I know about?\"");
            summary.AppendLine($"• \"Show me the detailed migration timeline\"");

            return summary.ToString();
        }

        private string CreateFollowUpPrompt(string question, AzureRecommendation analysis, ApplicationData appData)
        {
            var totalCores = analysis.CurrentState?.ServerSpecifications?.Sum(s => s.Cores) ?? 0;
            
            return $@"
Based on the previous analysis for {appData.ApplicationName}, please answer this follow-up question:

QUESTION: {question}

CONTEXT FROM PREVIOUS ANALYSIS:
- Application: {appData.ApplicationName}
- Servers: {analysis.CurrentState?.TotalServers ?? 0}
- Migration Complexity: {analysis.Complexity?.OverallComplexity ?? "Not specified"}
- Estimated Cost: ${analysis.EstimatedCosts?.TotalMonthlyCost ?? 0:F2}/month
- Timeline: {analysis.Complexity?.EstimatedTimeframe ?? "Not specified"}

Please provide a detailed, specific answer based on this application's context. If the question is about costs, provide breakdowns. If about timeline, provide phases. If about risks, be specific to this application's characteristics.
";
        }
    }

    public class ChatMessage
    {
        public string Question { get; set; } = string.Empty;
    }
}
