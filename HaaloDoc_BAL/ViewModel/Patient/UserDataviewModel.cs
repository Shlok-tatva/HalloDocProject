using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class UserDataviewModel
    {

        public int userid { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter date of birth")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter State")]
        public string State { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Zip code must be exactly 6 digits")]
        public string ZipCode { get; set; }
    }
}
