# Azure Migration Agent - Setup Guide

## Quick Start

### 1. Get API Access

**Option A: OpenAI (Recommended for testing)**
1. Go to https://platform.openai.com/
2. Create account and get API key
3. Update `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-your-actual-key-here",
       "ModelId": "gpt-4o"
     }
   }
   ```

**Option B: Azure AI Foundry (Recommended for Enterprise)**
1. Go to Azure AI Foundry (ai.azure.com)
2. Create/Select AI Hub and Project
3. **Deploy gpt-4.1 model** (excellent intelligence for complex migration analysis)
   - Deployment name: `gpt-4.1` 
   - Resource: `gpt-4-1-migration-resource`
   - Capacity: 250K TPM (tokens per minute)
4. Get endpoint and key from deployment
5. Update `appsettings.json`:
   ```json
   {
     "Azure": {
       "OpenAI": {
         "Endpoint": "https://gpt-4-1-migration-resource.cognitiveservices.azure.com/",
         "ApiKey": "your-actual-key-here", 
         "DeploymentName": "gpt-4.1"
       }
     }
   }
   ```

**Model Recommendations (Intelligence Priority):**
- 🥇 **gpt-4.1**: Highest available intelligence, excellent reasoning (YOUR CURRENT SETUP ✅)
- 🥈 **o3-pro**: Alternative high intelligence model
- 🥉 **gpt-4o**: Solid choice, widely available
- 💰 **gpt-4o-mini**: Budget-friendly option

**Note**: Claude Sonnet is NOT available in Azure AI Foundry (Anthropic models only on AWS/direct API)

### 2. Run the Application

```powershell
cd AzureMigrationAgent
dotnet restore
dotnet build
dotnet run
```

### 3. Expected Output

The application will:
1. ✅ Load your `appdata.json` file
2. 🤖 Analyze it using AI
3. 📊 Generate comprehensive Azure migration recommendations
4. 💾 Save results to timestamped JSON file

## What You'll Get

### Current State Analysis
- Server inventory and specifications
- Technology stack assessment  
- Resource utilization patterns

### Azure Target Architecture
- **Specific service recommendations** (App Service, VMs, databases)
- **Exact SKU suggestions** (Standard_D4s_v5, Premium P2V3, etc.)
- **Resource sizing** based on current specs
- **Cost estimates** with monthly/annual projections

### Migration Roadmap
- Complexity assessment (Low/Medium/High)
- Timeline estimates
- Risk mitigation strategies
- Prerequisites checklist

## Sample Recommendations for Your BAC Application

Based on your current data, expect recommendations like:

- **Azure App Service Premium P2V3** for the C# application
- **Azure SQL Database** instead of on-premises databases  
- **West US 2** region (based on Denver/Colorado location)
- **Azure Application Gateway** for load balancing
- **Azure Monitor + Application Insights** for observability
- **Estimated costs**: ~$300-400/month for basic setup

## Testing Strategy

1. **Start with this single application** (BAC)
2. **Validate recommendations** against your expectations
3. **Refine the AI prompts** based on results
4. **Scale to database integration** once satisfied

## Next Steps After Testing

1. **Database Integration**: Connect to your application inventory database
2. **Batch Processing**: Analyze hundreds of applications at once
3. **RAG Pipeline**: Add domain-specific knowledge base
4. **Web Interface**: Build user-friendly front-end
5. **Azure Integration**: Auto-deploy recommended resources

## Troubleshooting

**Issue**: "API key not configured"
- **Solution**: Update appsettings.json with valid API key

**Issue**: "appdata.json not found"  
- **Solution**: Place file in project directory or provide full path

**Issue**: "Model not available"
- **Solution**: Verify model deployment in Azure OpenAI or use gpt-4 for OpenAI

## Cost Considerations

- **OpenAI API**: ~$0.01-0.03 per analysis (very cheap for testing)
- **Azure OpenAI**: Included in your Azure credits
- **Recommended**: Start with OpenAI for initial testing
