using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class AdminProfileView
    {
        // Account Information
        public int adminId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public short? Status { get; set; }
        public string statusString
        {
            get
            {
                switch (Status)
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
        public string role { get; set;}

        // Administrator Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string Phone { get; set; }
        public List<int> AdminRegions { get; set; }

        // Mailing & Billing Information
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public string Zip { get; set; }
        public string billingPhone { get; set; }

    }

}
