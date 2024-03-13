using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Feature.Configuration.Infra
{


    public  class SqlServerConnection : ISqlConnection
    {
        private  readonly ILogger<SqlServerConnection> _logger;

        public SqlServerConnection(ILogger<SqlServerConnection> logger)
        {
            _logger = logger;   
        }

        public void Connect(string conn)
        {
            using (SqlConnection db = new SqlConnection(conn))
            {
                try
                {
                    db.Open();
                    Console.WriteLine("Conexão com o banco de dados SQL Server estabelecida com sucesso.");
                    _logger.LogInformation("Conexão com o banco de dados SQL Server estabelecida com sucesso.");
                }
                catch (SqlException e)
                {
                     Console.WriteLine("Ocorreu um erro ao se conectar ao banco de dados SQL Server:");
                    _logger.LogInformation("Ocorreu um erro ao se conectar ao banco de dados SQL Server: " + e.Message);
                    Console.WriteLine(e.Message);
                    db.Dispose();
                }
            }
        }
    }
}
