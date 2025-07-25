using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class SapConnectionSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ServiceLayerUrl { get; set; }
        public string ServerDatabase { get; set; }
        public string Database { get; set; }
        public string UserDatabase { get; set; }
        public string PasswordDatabase { get; set; }
    }
}
