using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class ProviderInfoAdmin
    {
        public int providerId { get; set; }
        public bool stopNotification {get;set;}
        public string providerName { get; set; }
        public string providerRole { get; set; }
        public int? onCallStatus { get;set;}
        public int? providerStatus { get;set;}

        public string statusString
        {
            get
            {
                switch (providerStatus)
                {
                    case 1:
                        return "Active";
                    case 2:
                        return "Pending";
                    case 3:
                        return "Inactive";
                    default:
                        return "None";
                }
            }
        }
        public string providerEmail { get;set;}
        public string providerPhone { get;set;}
    }
}
