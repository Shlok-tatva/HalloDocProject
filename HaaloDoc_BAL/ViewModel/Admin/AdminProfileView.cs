
using System.ComponentModel.DataAnnotations;

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

        public string role { get; set; }

        public int? roleId { get; set; }

        // Administrator Information
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Compare("Email", ErrorMessage = "Email and Confirm Email must match")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }

        // Mailing & Billing Information
        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State ID is required")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Zip code must be exactly 6 digits")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Billing phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string? billingPhone { get; set; }

        public List<int> AdminRegions { get; set; }

    }
}
