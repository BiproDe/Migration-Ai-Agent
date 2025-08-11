# 🚀 Azure Migration AI Agent

An intelligent C# console application powered by **Microsoft Semantic Kernel** and **Azure OpenAI (GPT-4.1)** that analyzes on-premises infrastructure and provides comprehensive Azure migration recommendations.

## 🎯 What It Does

- **Analyzes** on-premises application infrastructure from JSON data
- **Recommends** optimal Azure services and SKUs 
- **Estimates** migration costs and complexity
- **Provides** security and compliance guidance
- **Generates** structured reports for migration planning

## 🏗️ Architecture

```
� Azure Migration AI Agent
├── 🧠 AI Analysis Engine (Semantic Kernel + GPT-4.1)
├── 📊 Data Processing (JSON → C# Models)  
├── 🎯 Migration Recommendations
├── 💰 Cost Estimation
└── 📄 Report Generation
```

## 🛠️ Tech Stack

- **C# (.NET 8)** - Main application framework
- **Microsoft Semantic Kernel** - AI orchestration and prompt engineering
- **Azure OpenAI (GPT-4.1)** - Advanced AI model for analysis
- **Newtonsoft.Json** - JSON processing
- **Dependency Injection** - Clean architecture patterns

## 📋 Prerequisites

- **.NET 8 SDK** or later
- **Azure OpenAI Service** with GPT-4.1 deployment OR **OpenAI API** access
- **Visual Studio 2022** or **VS Code** (recommended)

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/BiproDe/Migration-Ai-Agent.git
cd Migration-Ai-Agent
```

### 2. Configure API Keys
```bash
# Copy the example configuration
copy appsettings.example.json appsettings.json

# Edit appsettings.json with your API keys
# For Azure OpenAI:
{
  "Azure": {
    "OpenAI": {
      "Endpoint": "https://your-resource.cognitiveservices.azure.com/",
      "ApiKey": "your-api-key",
      "DeploymentName": "gpt-4.1"
    }
  }
}

# OR for OpenAI:
{
  "OpenAI": {
    "ApiKey": "sk-your-openai-key",
    "Model": "gpt-4.1"
  }
}
```

### 3. Prepare Your Data
Place your infrastructure JSON file as `appdata.json` in the project root:

```json
{
  "application_Name": "Your Application Name",
  "mALServers": [
    {
      "server_Name": "SERVER01",
      "cpUs_Cores": "8",
      "memory_Size_MB": "32768",
      "operating_System": "Windows Server 2019"
    }
  ]
}
```

### 4. Build and Run
```bash
# Restore packages
dotnet restore

# Build the project  
dotnet build

# Run the analysis
dotnet run
```

## 📊 Sample Output

The agent generates comprehensive analysis including:

```json
{
  "ApplicationName": "BILLING ASSISTANCE CENTER",
  "AnalysisDate": "2025-08-11T14:36:53",
  "CurrentState": {
    "TotalServers": 2,
    "TotalCores": 12,
    "TotalMemoryGB": 48,
    "OperatingSystems": ["Windows Server 2016", "Windows Server 2019"]
  },
  "AzureRecommendations": {
    "ComputeServices": [
      {
        "ServiceType": "Azure Virtual Machines",
        "RecommendedSKU": "Standard_D4s_v5",
        "MonthlyCostUSD": 280.32
      }
    ],
    "TotalEstimatedMonthlyCost": 1245.67,
    "MigrationComplexity": "Medium",
    "EstimatedMigrationTimeWeeks": 8
  }
}
```

## 📁 Project Structure

```
📦 AzureMigrationAgent/
├── 📄 Program.cs                    # Main orchestrator
├── 📄 appsettings.json              # Configuration (not in repo)
├── 📄 appsettings.example.json      # Configuration template
│
├── 📁 Models/                       # Data structures
│   ├── 📄 ApplicationData.cs        # Input data model
│   └── 📄 AzureRecommendation.cs    # Output recommendation model
│
├── 📁 Services/                     # Business logic
│   └── 📄 AzureMigrationAgent.cs    # Core AI agent service
│
├── 📁 Knowledge/                    # RAG knowledge base
│   └── 📄 azure-migration-guide.md  # Domain expertise for future RAG
│
└── 📁 Documentation/                # Technical docs
    ├── 📄 ARCHITECTURE.md           # Detailed architecture guide
    ├── 📄 DATA-FLOW.md             # Data flow documentation
    └── 📄 RAG-ENHANCEMENT.md       # Future RAG implementation
```

## 🔧 Configuration Options

### Azure OpenAI vs OpenAI
- **Azure OpenAI**: Enterprise-ready, data residency, better compliance
- **OpenAI**: Direct API access, potentially faster updates

### Model Recommendations
- **GPT-4.1**: Highest intelligence, best for complex infrastructure analysis
- **GPT-4**: Good balance of performance and cost
- **GPT-3.5-Turbo**: Cost-effective for simpler analyses

## 🚀 Advanced Features

### 1. Custom Prompting
Modify `CreateAnalysisPrompt()` in `AzureMigrationAgent.cs` to customize AI analysis:

```csharp
private string CreateAnalysisPrompt(ApplicationData data)
{
    return $@"
        You are an expert Azure Migration Architect...
        Custom instructions here...
    ";
}
```

### 2. Extending Data Models
Add new properties to capture more infrastructure details:

```csharp
public class MalServer 
{
    public string ServerName { get; set; }
    public string DatabaseVersion { get; set; }  // New field
    public List<string> InstalledSoftware { get; set; }  // New field
}
```

### 3. Future RAG Integration
The `Knowledge/` folder is prepared for Retrieval-Augmented Generation:
- Add domain-specific migration guides
- Include Azure best practices
- Custom pricing information

## 📈 Roadmap

- [ ] **RAG Pipeline**: Use knowledge base for context-aware recommendations
- [ ] **Batch Processing**: Analyze multiple applications simultaneously  
- [ ] **Web Interface**: REST API and web dashboard
- [ ] **Database Integration**: Store analysis history and trends
- [ ] **Azure Resource Manager**: Direct deployment of recommended infrastructure
- [ ] **Cost Optimization**: Continuous monitoring and right-sizing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⚠️ Security

- **Never commit** `appsettings.json` with real API keys
- Use **Azure Key Vault** for production deployments
- Review generated reports before sharing (may contain sensitive infrastructure details)

## 📞 Support

- Create an [Issue](https://github.com/BiproDe/Migration-Ai-Agent/issues) for bug reports
- Start a [Discussion](https://github.com/BiproDe/Migration-Ai-Agent/discussions) for questions
- Check [Documentation/](./Documentation/) for detailed technical guides

---

## 🎉 Getting Started Example

```bash
# Quick 3-step setup:
git clone https://github.com/BiproDe/Migration-Ai-Agent.git
cd Migration-Ai-Agent
cp appsettings.example.json appsettings.json
# Edit appsettings.json with your API key
dotnet run
```

**Ready to migrate to Azure with AI-powered insights!** 🚀

---

<sub>Built with ❤️ using Microsoft Semantic Kernel and Azure OpenAI</sub>
