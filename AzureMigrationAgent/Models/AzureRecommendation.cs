namespace AzureMigrationAgent.Models;

public class AzureRecommendation
{
    public string ApplicationName { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public CurrentStateArchitecture CurrentState { get; set; } = new();
    public TargetStateArchitecture TargetState { get; set; } = new();
    public List<string> KeyRecommendations { get; set; } = new();
    public MigrationComplexity Complexity { get; set; } = new();
    public EstimatedCosts EstimatedCosts { get; set; } = new();
    public List<string> RisksAndConsiderations { get; set; } = new();
}

public class CurrentStateArchitecture
{
    public int TotalServers { get; set; }
    public int ProductionServers { get; set; }
    public int NonProductionServers { get; set; }
    public List<ServerSpecs> ServerSpecifications { get; set; } = new();
    public string HostingModel { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = new();
    public string BusinessCriticality { get; set; } = string.Empty;
}

public class TargetStateArchitecture
{
    public List<AzureService> RecommendedServices { get; set; } = new();
    public string Region { get; set; } = string.Empty;
    public string AvailabilityZone { get; set; } = string.Empty;
    public SecurityRecommendations Security { get; set; } = new();
    public NetworkingRecommendations Networking { get; set; } = new();
}

public class ServerSpecs
{
    public string ServerName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public int Cores { get; set; }
    public int MemoryMB { get; set; }
    public int DiskGB { get; set; }
    public string OperatingSystem { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class AzureService
{
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int RecommendedCores { get; set; }
    public int RecommendedMemoryGB { get; set; }
    public int RecommendedStorageGB { get; set; }
    public string Tier { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public decimal EstimatedMonthlyCostUSD { get; set; }
}

public class SecurityRecommendations
{
    public List<string> IdentityAndAccess { get; set; } = new();
    public List<string> NetworkSecurity { get; set; } = new();
    public List<string> DataProtection { get; set; } = new();
    public List<string> Compliance { get; set; } = new();
}

public class NetworkingRecommendations
{
    public string VNetConfiguration { get; set; } = string.Empty;
    public List<string> SubnetDesign { get; set; } = new();
    public string LoadBalancing { get; set; } = string.Empty;
    public string DNSStrategy { get; set; } = string.Empty;
}

public class MigrationComplexity
{
    public string OverallComplexity { get; set; } = string.Empty;
    public List<string> ComplexityFactors { get; set; } = new();
    public string EstimatedTimeframe { get; set; } = string.Empty;
    public List<string> Prerequisites { get; set; } = new();
}

public class EstimatedCosts
{
    public decimal MonthlyComputeCost { get; set; }
    public decimal MonthlyStorageCost { get; set; }
    public decimal MonthlyNetworkingCost { get; set; }
    public decimal TotalMonthlyCost { get; set; }
    public decimal AnnualCost { get; set; }
    public decimal MigrationCost { get; set; }
    public string CostOptimizationTips { get; set; } = string.Empty;
}
