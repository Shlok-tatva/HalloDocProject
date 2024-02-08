using System;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    public class PatientFormData
    {
        [Required(ErrorMessage = "Please enter brief details of symptoms")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Please enter Password")]
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        //[RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid zip code")]
        public string ZipCode { get; set; }

        public string RoomOrSuite { get; set; }

        public string? FileName { get; set; }
    }

}