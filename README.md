# 🚀 Azure Migration AI Agent

An comprehensive **AI-powered migration analysis platform** with both **web-based chat interface** and **core library** components. Built with **Microsoft Semantic Kernel** and **Azure OpenAI (GPT-4.1)** to analyze on-premises infrastructure and provide intelligent Azure migration recommendations.

## 🎯 What It Does

- **🔍 Analyzes** on-premises application infrastructure from JSON data
- **🎯 Recommends** optimal Azure services and SKUs with detailed justifications
- **💰 Estimates** migration costs and complexity with breakdown analysis
- **🔒 Provides** security and compliance guidance tailored to your infrastructure
- **📊 Generates** comprehensive reports with PDF/JSON export capabilities
- **💬 Interactive Chat** for follow-up questions and detailed exploration
- **🌐 Professional Web Interface** for enterprise-ready user experience

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

### Core Library (AzureMigrationAgent)
- **C# (.NET 8)** - Core business logic and AI services
- **Microsoft Semantic Kernel** - AI orchestration and prompt engineering
- **Azure OpenAI (GPT-4.1)** - Advanced AI model for intelligent analysis
- **Newtonsoft.Json** - JSON processing and data serialization

### Web Application (AzureMigrationAgent.Web)
- **ASP.NET Core (.NET 9)** - Modern web framework
- **Razor Pages** - Server-side rendered UI
- **Bootstrap 5 & FontAwesome** - Professional responsive design
- **HTML5/CSS3/JavaScript** - Interactive frontend features
- **jsPDF** - Client-side PDF report generation
- **Session Management** - Persistent chat context

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
Update the web application's configuration with your API keys:

```bash
# Navigate to web project
cd AzureMigrationAgent.Web

# Edit appsettings.json with your API keys
# For Azure OpenAI (Recommended):
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
    "ModelId": "gpt-4o"
  }
}
```

### 3. Build and Run the Web Application
```bash
# Restore packages and build
dotnet restore
dotnet build

# Start the web application
dotnet run

# Access the application
# Open browser to: http://localhost:5104
# Or secure: https://localhost:7121
```

### 4. Upload and Analyze
1. **Open** your browser to `http://localhost:5104`
2. **Upload** your infrastructure JSON file using the drag-and-drop interface
3. **Interact** with the AI agent through the chat interface
4. **Export** your analysis as PDF or JSON report

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
📦 Azure Migration AI Platform/
├── 🌐 AzureMigrationAgent.Web/          # ASP.NET Core Web Application
│   ├── 📄 Program.cs                    # Web app entry point & DI setup
│   ├── ⚙️ appsettings.json             # Configuration (API keys, etc.)
│   │
│   ├── 📁 Controllers/                  # RESTful API endpoints
│   │   └── 📄 ChatController.cs         # Chat API, file upload, exports
│   │
│   ├── � Pages/                        # Razor Pages UI
│   │   └── 📄 Index.cshtml              # Main chat interface
│   │
│   └── 📁 wwwroot/                      # Static web assets
│       ├── 📁 css/
│       │   └── 📄 chat.css              # Professional UI styling
│       ├── 📁 js/                       # JavaScript functionality
│       └── 📁 images/                   # UI assets
│
├── 📚 AzureMigrationAgent/              # Core Library (Referenced by Web)
│   ├── 📁 Models/                       # Data structures & DTOs
│   │   ├── 📄 ApplicationData.cs        # Input data model
│   │   └── 📄 AzureRecommendation.cs    # AI analysis output model
│   │
│   └── 📁 Services/                     # Business logic & AI services
│       └── 📄 AzureMigrationAgent.cs    # Core AI analysis engine
│
├── 📁 Knowledge/                        # RAG knowledge base (Future)
│   └── 📄 azure-migration-guide.md     # Domain expertise for RAG
│
├── 📁 Documentation/                    # Technical documentation
│   ├── 📄 ARCHITECTURE.md              # Detailed architecture guide
│   ├── 📄 DATA-FLOW.md                # Data flow documentation
│   └── 📄 RAG-ENHANCEMENT.md          # Future RAG implementation
│
├── 📄 appdata.json                      # Sample application data
├── 📄 appsettings.example.json          # Configuration template
└── 📄 AzureMigrationAgent.sln           # Visual Studio solution
```

### 🏗️ Architecture Layers

1. **🌐 Presentation Layer** (`AzureMigrationAgent.Web`)
   - Interactive chat interface
   - File upload and drag-and-drop
   - Real-time AI responses
   - PDF/JSON export capabilities

2. **🧠 Business Logic Layer** (`AzureMigrationAgent`)
   - AI analysis engine
   - Migration recommendations
   - Cost calculations
   - Security assessments

3. **📊 Data Layer**
   - JSON parsing and validation
   - Session management
   - Export data formatting

## 🌐 Web Interface Features

### 💬 Interactive Chat Experience
- **Professional Design**: Modern, full-screen chat interface with sidebar navigation
- **Drag & Drop Upload**: Easy JSON file upload with visual feedback
- **Real-time Analysis**: Live AI processing with typing indicators
- **Session Persistence**: Maintains context across browser sessions
- **Follow-up Questions**: Ask detailed questions about any aspect of the analysis

### 🎯 Quick Action Buttons
- **🔒 Security Analysis**: Get detailed security recommendations and compliance guidance
- **💰 Cost Breakdown**: Drill down into cost estimates and optimization opportunities
- **📅 Migration Timeline**: View detailed migration phases and milestones
- **⚠️ Risk Assessment**: Understand potential risks and mitigation strategies

### 📊 Export Capabilities
- **📄 PDF Reports**: Professional, formatted reports with comprehensive analysis
- **📋 JSON Data**: Raw analysis data for integration with other tools
- **📈 Executive Summary**: High-level overview for stakeholder presentations

### 🎨 User Experience
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Bootstrap 5 UI**: Modern, professional appearance with accessibility features
- **Font Awesome Icons**: Intuitive visual elements for better usability
- **Loading States**: Clear feedback during AI processing
- **Error Handling**: Graceful error handling with helpful messages

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

### ✅ Completed
- [x] **Core AI Analysis Engine**: Intelligent infrastructure analysis with GPT-4.1
- [x] **Web Interface**: Professional chat-based interface with full-screen design
- [x] **Interactive Chat**: Follow-up questions and contextual conversations
- [x] **Export Functionality**: PDF and JSON report generation
- [x] **Session Management**: Persistent chat context and analysis state
- [x] **Professional UI**: Bootstrap 5 responsive design with modern UX

### 🚧 In Progress
- [ ] **Enhanced Data Models**: Support for more infrastructure components
- [ ] **Advanced Security Analysis**: Detailed compliance and security assessments
- [ ] **Cost Optimization Engine**: AI-powered cost optimization recommendations

### 🔮 Future Features
- [ ] **RAG Pipeline**: Use knowledge base for context-aware recommendations
- [ ] **Batch Processing**: Analyze multiple applications simultaneously  
- [ ] **Database Integration**: Store analysis history and trends
- [ ] **Azure Resource Manager**: Direct deployment of recommended infrastructure
- [ ] **Multi-Cloud Support**: AWS and Google Cloud migration analysis
- [ ] **API Integration**: REST API for enterprise integrations
- [ ] **Advanced Analytics**: Migration success tracking and optimization insights

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
# Quick 4-step setup for web interface:
git clone https://github.com/BiproDe/Migration-Ai-Agent.git
cd Migration-Ai-Agent/AzureMigrationAgent.Web
# Edit appsettings.json with your Azure OpenAI API key
dotnet run
# Open browser to: http://localhost:5104
```

**Ready to analyze your infrastructure with AI-powered migration insights!** 🚀

### 📱 Access URLs:
- **HTTP**: http://localhost:5104
- **HTTPS**: https://localhost:7121 (may show certificate warning - safe to ignore for local development)

### 🎯 Quick Demo:
1. **Upload** the included `appdata.json` sample file
2. **Chat** with the AI about your migration strategy
3. **Ask** questions like "What about security considerations?" or "Break down the costs"
4. **Export** your analysis as a professional PDF report

---

<sub>Built with ❤️ using Microsoft Semantic Kernel and Azure OpenAI</sub>
