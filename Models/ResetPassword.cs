using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCoreApp.Models
{
    public class ResetPassword
    {
        public string email { get; set; }
        public string password { get; set; }
        public string confirmpassword { get; set; }
        public string Token { get; set; }
    }
}
