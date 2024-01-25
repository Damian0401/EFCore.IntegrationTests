## Integration Tests

Base template for integration tests of EF Core and SQL Server using TestContainers.

### Stack
- .NET 8
- Entity Framework Core
- xUnit
- TestContainers

### Requirements
- .NET 8
- Docker
- (optional) MsSqlServer image: `mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04`

### How to run
Using dotnet CLI execute the following command:
```bash
dotnet test
```
Required docker images will be downloaded (if not present) and the tests will be executed.