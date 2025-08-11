# Azure Migration Knowledge Base

## Azure Service Mapping for Common Scenarios

### .NET Applications
- **ASP.NET Core**: Azure App Service (Premium tier for production)
- **ASP.NET Framework**: Azure App Service or Azure Virtual Machines
- **Windows Services**: Azure Container Instances or Service Fabric
- **Desktop Apps**: Azure Virtual Desktop

### Database Migrations
- **SQL Server**: Azure SQL Database or SQL Managed Instance
- **Oracle**: Azure Database for PostgreSQL or SQL Database
- **MySQL**: Azure Database for MySQL
- **MongoDB**: Azure Cosmos DB

### Infrastructure Patterns
- **Load Balancers**: Azure Application Gateway or Load Balancer
- **File Shares**: Azure Files or Azure NetApp Files
- **Backup Systems**: Azure Backup
- **Monitoring**: Azure Monitor + Application Insights

## Regional Recommendations

### US Regions by Location
- **West Coast**: West US, West US 2, West US 3
- **Central**: Central US, North Central US, South Central US
- **East Coast**: East US, East US 2

### Compliance Considerations
- **Government**: Azure Government regions
- **Healthcare**: Regions with HIPAA compliance
- **Financial**: Regions with SOC/PCI compliance

## Cost Optimization Strategies

### Compute Savings
- **Azure Hybrid Benefit**: 40% savings on Windows VMs
- **Reserved Instances**: Up to 72% savings for predictable workloads
- **Spot Instances**: Up to 90% savings for flexible workloads
- **Auto-scaling**: Automatic scale based on demand

### Storage Optimization
- **Storage Tiers**: Hot, Cool, Archive for different access patterns
- **Managed Disks**: Premium SSD for performance, Standard for cost
- **Content Delivery**: Azure CDN for static content

## Migration Complexity Factors

### Low Complexity (1-3 months)
- Cloud-native applications (.NET Core, containerized)
- Stateless applications
- Applications with modern architecture
- Limited dependencies

### Medium Complexity (3-6 months)
- Legacy .NET Framework applications
- Applications with database dependencies
- Multiple environment tiers
- Some custom integrations

### High Complexity (6+ months)
- Monolithic legacy applications
- Complex database schemas
- Extensive third-party integrations
- Compliance requirements
- Custom hardware dependencies

## Security Best Practices

### Identity and Access
- **Azure AD Integration**: Single sign-on and MFA
- **Managed Identities**: Eliminate password management
- **RBAC**: Role-based access control
- **Conditional Access**: Location and device-based policies

### Network Security
- **Virtual Networks**: Isolated network environments
- **Network Security Groups**: Subnet-level firewall rules
- **Application Gateway WAF**: Web application firewall
- **Azure Firewall**: Centralized network security

### Data Protection
- **Encryption at Rest**: All Azure storage encrypted by default
- **Encryption in Transit**: TLS/SSL for all communications
- **Key Vault**: Centralized secret and certificate management
- **Backup and Recovery**: Automated backup solutions

## Performance Considerations

### Compute Sizing Guidelines
- **2-4 vCPUs**: Small applications, dev/test environments
- **4-8 vCPUs**: Medium applications, production workloads
- **8+ vCPUs**: High-performance applications, databases

### Memory Recommendations
- **4-8 GB**: Basic web applications
- **8-16 GB**: Standard business applications  
- **16-32 GB**: Database servers, high-memory workloads
- **32+ GB**: Big data, in-memory databases

### Storage Performance
- **Standard HDD**: Backup, archival data
- **Standard SSD**: General purpose workloads
- **Premium SSD**: High IOPS, low latency requirements
- **Ultra Disk**: Mission-critical, high-performance databases
