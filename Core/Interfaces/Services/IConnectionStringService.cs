using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IConnectionStringService
    {
        string BuildSqlServerConnectionString(string server, string db, string user, string password);
    }
}
