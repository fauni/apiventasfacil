using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ConnectionStringService : IConnectionStringService
    {
        public string BuildSqlServerConnectionString(string server, string db, string user, string password)
        {
            return $"Server={server};Database={db};User Id={user};Password={password};TrustServerCertificate=True;MultipleActiveResultSets=True;";
        }
    }
}
