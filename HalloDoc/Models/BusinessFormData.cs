using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    public class BusinessFormData : ConciergeFormData
    {
        [Required(ErrorMessage = "Please enter your first name")]
        public string BusinessFirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string BusinessLastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string BusinessPhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string BusinessEmail { get; set; }

        [Required(ErrorMessage = "Please enter the business/property name")]
        public string BusinessOrPropertyName { get; set; }

        public string? CaseNumber { get; set; }
    }
}
