using Microsoft.Data.SqlClient;

namespace TaskManager.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
