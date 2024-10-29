using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Shared
{
    public static class AppConstants
    {
        public const string BaseApiUrl = "https://zonosisapi.azurewebsites.net";

        public const string HubPattern = "/hubs/pet-hub";
        public const string HubFullUrl = BaseApiUrl + HubPattern;
    }
}
