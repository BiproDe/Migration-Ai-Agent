using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using AzureMigrationAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add session support for maintaining chat context
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Semantic Kernel
var kernelBuilder = Kernel.CreateBuilder();

// Check for Azure OpenAI configuration
var azureOpenAIEndpoint = builder.Configuration["Azure:OpenAI:Endpoint"];
var azureOpenAIApiKey = builder.Configuration["Azure:OpenAI:ApiKey"];
var azureDeploymentName = builder.Configuration["Azure:OpenAI:DeploymentName"];

// Check for OpenAI configuration
var openAIApiKey = builder.Configuration["OpenAI:ApiKey"];
var openAIModel = builder.Configuration["OpenAI:Model"];

if (!string.IsNullOrEmpty(azureOpenAIEndpoint) && !string.IsNullOrEmpty(azureOpenAIApiKey))
{
    // Use Azure OpenAI
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: azureDeploymentName ?? "gpt-4",
        endpoint: azureOpenAIEndpoint,
        apiKey: azureOpenAIApiKey);
}
else if (!string.IsNullOrEmpty(openAIApiKey))
{
    // Use OpenAI
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: openAIModel ?? "gpt-4",
        apiKey: openAIApiKey);
}
else
{
    throw new InvalidOperationException("No AI service configuration found. Please configure either Azure OpenAI or OpenAI in appsettings.json");
}

var kernel = kernelBuilder.Build();
builder.Services.AddSingleton(kernel);

// Register the migration agent
builder.Services.AddScoped<AzureMigrationAgent.Services.AzureMigrationAgent>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseSession(); // Enable session middleware

app.UseAuthorization();

// Map API controllers
app.MapControllers();
app.MapRazorPages();

app.Run();
