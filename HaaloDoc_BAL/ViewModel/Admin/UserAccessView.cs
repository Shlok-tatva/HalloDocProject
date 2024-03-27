using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class UserAccessView
    {
        public int? adminId { get; set; }
        public int? physicianId { get; set; }
        public string aspnetUserId { get; set; }
        public int accountTypeId { get; set; }
        public string accountType { get; set; }
        public string accountPOC { get; set; }
        public string phoneNumber { get; set; }
        public int statusId { get; set; }
        public string statusString
        {
            get
            {
                switch (statusId)
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
        public int openRequest { get; set; }
    }
}
