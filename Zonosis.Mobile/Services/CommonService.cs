using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.Services
{
    public class CommonService
    {
        public string? Token { get; private set; }

        public void SetToken(string? token) => Token = token;
    }
}
