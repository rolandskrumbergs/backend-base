using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BackendBase.Infrastructure.Data;
public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SqlConnection")
            ?? throw new ConfigurationException("SqlConnection is not configured");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
