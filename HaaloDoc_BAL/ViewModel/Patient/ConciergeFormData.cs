using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class ConciergeFormData : PatientFormData
    {
        [Required(ErrorMessage = "Please enter your first name")]
        public string ConciergeFirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string ConciergeLastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string ConciergePhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ConciergeEmail { get; set; }

        [Required(ErrorMessage = "Please enter the hotel/property name")]
        public string HotelOrPropertyName { get; set; }
    }
}
