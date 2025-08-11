# Azure Migration Agent - Architecture Deep Dive

## 📁 Project Structure Explained

```
📦 AzureMigrationAgent/
├── 📄 Program.cs                    # Main entry point - orchestrates everything
├── 📄 appsettings.json              # Configuration (API keys, endpoints)
├── 📄 AzureMigrationAgent.csproj    # Project file (dependencies, .NET version)
├── 📄 nuget.config                  # NuGet package sources configuration
│
├── 📁 Models/                       # Data structures
│   ├── 📄 ApplicationData.cs        # Input data model (maps to your JSON)
│   └── 📄 AzureRecommendation.cs    # Output data model (structured results)
│
├── 📁 Services/                     # Business logic
│   └── 📄 AzureMigrationAgent.cs    # Core AI agent service
│
├── 📁 Knowledge/                    # Future RAG knowledge base
│   └── 📄 azure-migration-guide.md  # Domain expertise for RAG
│
├── 📁 bin/Debug/net8.0/             # Compiled application
└── 📁 obj/                          # Build artifacts
```

## 🔄 Data Flow Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────────┐
│   INPUT JSON    │───▶│   PROGRAM.CS     │───▶│  SEMANTIC KERNEL    │
│   (appdata.json)│    │  (Orchestrator)  │    │   (AI Service)      │
└─────────────────┘    └──────────────────┘    └─────────────────────┘
                              │                           │
                              ▼                           ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────────┐
│  JSON OUTPUT    │◀───│  MODELS & DATA   │◀───│    GPT-4.1 API     │
│  (Results file) │    │   PROCESSING     │    │  (Azure OpenAI)     │
└─────────────────┘    └──────────────────┘    └─────────────────────┘
```

## 🎯 Detailed Component Breakdown

### 1. **Program.cs** - The Orchestrator 🎭
**Purpose**: Main application entry point, coordinates everything

**What it does**:
- Loads configuration from `appsettings.json`
- Initializes Semantic Kernel with your Azure OpenAI credentials  
- Reads your `appdata.json` file
- Creates the AI agent
- Calls the analysis service
- Formats and displays results
- Saves results to JSON file

**Key Code Flow**:
```csharp
Main() → Load Config → Initialize Kernel → Load JSON → Analyze → Display Results
```

### 2. **appsettings.json** - Configuration Hub ⚙️
**Purpose**: Stores all configuration settings

**What's in it**:
```json
{
  "Azure": {
    "OpenAI": {
      "Endpoint": "https://gpt-4-1-migration-resource.cognitiveservices.azure.com/",
      "ApiKey": "your-secret-key",
      "DeploymentName": "gpt-4.1"
    }
  }
}
```

**Why this matters**: 
- Keeps secrets separate from code
- Easy to change without recompiling
- Different configs for dev/test/prod

### 3. **Models/** - Data Structures 📊

#### **ApplicationData.cs** - Input Data Model
**Purpose**: Maps your JSON input to C# objects

**What it does**:
```csharp
// Maps this JSON:
{
  "application_Name": "BILLING ASSISTANCE CENTER",
  "mALServers": [...]
}

// To this C# object:
public class ApplicationData {
    public string ApplicationName { get; set; }
    public List<MalServer> MalServers { get; set; }
}
```

#### **AzureRecommendation.cs** - Output Data Model  
**Purpose**: Structures the AI recommendations

**Contains**:
- Current state analysis
- Target Azure architecture
- Cost estimates
- Migration complexity
- Security recommendations

### 4. **Services/AzureMigrationAgent.cs** - The AI Brain 🧠

**This is where the magic happens!**

#### **Key Methods**:

1. **`AnalyzeApplicationAsync()`** - Main analysis method
2. **`CreateAnalysisPrompt()`** - Builds the AI prompt
3. **`ParseRecommendationAsync()`** - Processes AI response
4. **`PopulateRecommendationFromResponseAsync()`** - Structures output

#### **AI Prompt Engineering**:
The service creates sophisticated prompts like:
```csharp
"Please analyze this on-premises application and provide comprehensive 
Azure migration recommendations including:
1. Current state architecture
2. Target Azure services with specific SKUs
3. Cost estimates
4. Migration complexity..."
```

## 🔄 Step-by-Step Execution Flow

### **Step 1: Application Startup**
```
Program.Main() 
├── Load appsettings.json
├── Create Semantic Kernel with Azure OpenAI
└── Initialize AzureMigrationAgent service
```

### **Step 2: Data Loading**
```
AnalyzeApplicationDataAsync()
├── Find appdata.json file
├── Read JSON content  
├── Deserialize to ApplicationData object
└── Validate data
```

### **Step 3: AI Analysis** ⭐
```
AzureMigrationAgent.AnalyzeApplicationAsync()
├── CreateAnalysisPrompt() - Build detailed prompt
├── Add system message (AI persona/expertise)
├── Call GPT-4.1 via Semantic Kernel
├── Get AI response
└── ParseRecommendationAsync() - Structure results
```

### **Step 4: Response Processing**
```
PopulateRecommendationFromResponseAsync()
├── Extract current state info
├── Map servers to Azure services
├── Calculate cost estimates
├── Assess migration complexity
└── Generate security recommendations
```

### **Step 5: Output Generation**
```
DisplayResults() & SaveResultsAsync()
├── Format console output with colors/sections
├── Create JSON file with timestamp
└── Save structured recommendations
```

## 🧠 How the AI Prompting Works

### **Input Transformation**:
Your JSON data gets transformed into a structured prompt:

```csharp
// Your data:
"server_Name": "USDDCWVFUSION02"
"cpUs_Cores": "4"  
"memory_Size_MB": "12288"

// Becomes this prompt:
"Server: USDDCWVFUSION02 has 4 cores and 12GB RAM. 
Please recommend appropriate Azure VM SKU..."
```

### **AI System Instructions**:
The AI is given expert persona:
```csharp
"You are an expert Azure Migration Architect with deep knowledge of:
1. On-premises infrastructure assessment
2. Azure services and SKU recommendations..."
```

### **Response Processing**:
AI provides recommendations which get structured into:
- Service recommendations
- Cost calculations  
- Risk assessments
- Migration timelines

## 🎯 Key Technologies Used

1. **Microsoft Semantic Kernel** - AI orchestration framework
2. **Azure OpenAI (GPT-4.1)** - The actual AI model
3. **Newtonsoft.Json** - JSON serialization/deserialization
4. **.NET 8** - Runtime platform
5. **Dependency Injection** - Service management

## 🔧 Configuration Points

### **Easy to Customize**:
1. **Change AI Model**: Update `DeploymentName` in appsettings.json
2. **Add New Prompts**: Modify `CreateAnalysisPrompt()` method
3. **Extend Data Model**: Add properties to `ApplicationData.cs`
4. **Enhance Output**: Modify `AzureRecommendation.cs`

## 📊 What Happens When You Run It

1. **Console shows**: "Initializing Semantic Kernel..."
2. **Loads your data**: "Loaded application: BILLING ASSISTANCE CENTER"  
3. **AI analysis**: "Analyzing current infrastructure..." (calls GPT-4.1)
4. **Displays results**: Structured output with recommendations
5. **Saves file**: "Analysis results saved to: AzureMigrationAnalysis_*.json"

## 🚀 Next Enhancement Points

1. **Database Integration**: Replace JSON file with SQL database
2. **RAG Pipeline**: Use the Knowledge/ folder for context
3. **Batch Processing**: Analyze multiple apps at once
4. **Web API**: Expose as REST service
5. **Azure Deployment**: Use recommended infrastructure

**This architecture is scalable and production-ready!** 🎉
