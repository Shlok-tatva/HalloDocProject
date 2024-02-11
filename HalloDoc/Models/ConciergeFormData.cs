using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
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

        // Concierge Location
        [Required(ErrorMessage = "Please enter the street")]
        public string ConciergeStreet { get; set; }

        [Required(ErrorMessage = "Please enter the city")]
        public string ConciergeCity { get; set; }

        [Required(ErrorMessage = "Please enter the state")]
        public string ConciergeState { get; set; }

        [Required(ErrorMessage = "Please enter the ZIP code")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid ZIP code")]
        public string ConciergeZipCode { get; set; }
    }
}
