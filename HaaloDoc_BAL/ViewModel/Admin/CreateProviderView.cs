using System;
using System.ComponentModel.DataAnnotations;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class CreateProviderView
    {
        public CreateProviderView()
        {
            regionOfservice = new int[0]; // Initialize as an empty array
        }

        public int? ProviderId { get; set; }
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public short? Status { get; set; }

        public string? statusString
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

        public List<Role?> allRoles { get; set; }

        public int? roleid { get; set; }

        public string? photo { get; set; }
        public string? signature { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(5, ErrorMessage = "Password must be 5 characters long", MinimumLength = 5)]
        public string password { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string phoneNumber { get; set; }
        public string? NPInumber { get; set; }
        public string? medicalLicence { get; set; }
        public string? Adminnotes { get; set; }
        public bool isAggrementDoc { get; set; }
        public bool isbackgroundDoc { get; set; }
        public bool istrainginDoc { get; set; }
        public bool isnondisclosuredoc { get; set; }
        public bool islicensedoc { get; set; }
        [Required(ErrorMessage = "Addreess is required")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string city { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid Zip Code")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Region is required")]
        public int regionId { get; set; }
        public string? regionName { get; set; } // only for scheduling

        public int? onCallStatus { get; set; } // only for provider-on-calll status

        public int[] regionOfservice { get; set; }

        [Required(ErrorMessage = "Business Name is required")]
        public string businessName { get; set; }

        [Required(ErrorMessage = "Business Website is required")]
        public string businessWebsite { get; set; }
        public string? Altphone { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public string? Syncemailaddress { get; set; }

        public IFormFile? PhotoFile { get; set; }
        public IFormFile? SignatureFile { get; set; }
        public IFormFile? Agreementdoc { get; set; }
        public IFormFile? NonDisclosuredoc { get; set; }
        public IFormFile? Trainingdoc { get; set; }
        public IFormFile? BackGrounddoc { get; set; }
        public IFormFile? Licensedoc { get; set; }
    }
}
