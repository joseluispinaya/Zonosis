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

        public event EventHandler? LoginStatusChanged;

        public void ToggleLoginStatus() => LoginStatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
