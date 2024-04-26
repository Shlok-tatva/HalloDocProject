using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class BusinessFormData : PatientFormData
    {
        [Required(ErrorMessage = "Please enter your first name")]
        public string BusinessFirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string BusinessLastName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string BusinessPhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string BusinessEmail { get; set; }

        [Required(ErrorMessage = "Please enter the business/property name")]
        public string BusinessOrPropertyName { get; set; }

        public string? CaseNumber { get; set; }
    }
}
