using Newtonsoft.Json;

namespace AzureMigrationAgent.Models;

public class ApplicationData
{
    [JsonProperty("application_ID")]
    public string ApplicationId { get; set; } = string.Empty;

    [JsonProperty("application_Name")]
    public string ApplicationName { get; set; } = string.Empty;

    [JsonProperty("application_Acronym")]
    public string ApplicationAcronym { get; set; } = string.Empty;

    [JsonProperty("assignment")]
    public string Assignment { get; set; } = string.Empty;

    [JsonProperty("target_Disposition")]
    public string TargetDisposition { get; set; } = string.Empty;

    [JsonProperty("criticality")]
    public string Criticality { get; set; } = string.Empty;

    [JsonProperty("host_Platform")]
    public string HostPlatform { get; set; } = string.Empty;

    [JsonProperty("languages")]
    public string Languages { get; set; } = string.Empty;

    [JsonProperty("region")]
    public string Region { get; set; } = string.Empty;

    [JsonProperty("in_Use_Server_Associations")]
    public string InUseServerAssociations { get; set; } = string.Empty;

    [JsonProperty("in_Use_Prod_Servers")]
    public string InUseProdServers { get; set; } = string.Empty;

    [JsonProperty("in_Use_Non_Prod_Servers")]
    public string InUseNonProdServers { get; set; } = string.Empty;

    [JsonProperty("application_Hosting_Model")]
    public string ApplicationHostingModel { get; set; } = string.Empty;

    [JsonProperty("mALServers")]
    public List<MalServer> MalServers { get; set; } = new();

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("business_Impact")]
    public string BusinessImpact { get; set; } = string.Empty;
}

public class MalServer
{
    [JsonProperty("server_Name")]
    public string ServerName { get; set; } = string.Empty;

    [JsonProperty("environment_Association")]
    public string EnvironmentAssociation { get; set; } = string.Empty;

    [JsonProperty("server_Status")]
    public string ServerStatus { get; set; } = string.Empty;

    [JsonProperty("operating_System")]
    public string OperatingSystem { get; set; } = string.Empty;

    [JsonProperty("cpUs_Physical")]
    public string CpUsPhysical { get; set; } = string.Empty;

    [JsonProperty("cpU_Description")]
    public string CpuDescription { get; set; } = string.Empty;

    [JsonProperty("cpUs_Cores")]
    public string CpUsCores { get; set; } = string.Empty;

    [JsonProperty("cpUs_HW_threads")]
    public string CpUsHwThreads { get; set; } = string.Empty;

    [JsonProperty("cpU_Speed_MHz")]
    public string CpuSpeedMhz { get; set; } = string.Empty;

    [JsonProperty("memory_Size_MB")]
    public string MemorySizeMb { get; set; } = string.Empty;

    [JsonProperty("disk_Size_GB")]
    public string DiskSizeGb { get; set; } = string.Empty;

    [JsonProperty("tcP_IP_Address")]
    public string TcpIpAddress { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("location_Code")]
    public string LocationCode { get; set; } = string.Empty;
}
