using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class DateOfBirthInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime dateOfBirth = (DateTime)value;
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult("Date of birth cannot be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class PatientFormData
    {
        [Required(ErrorMessage = "Please enter brief details of symptoms")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter date of birth")]
        [DateOfBirthInPast(ErrorMessage = "Date of birth cannot be in the future.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter State")]
        public int regionId { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid zip code")]
        public string ZipCode { get; set; }

        public string? RoomOrSuite { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? RelationWithPatinet { get; set; }

        public string? UploadImage { get; set; }

        public string? adminNotes { get; set; }

        public string? providerNotes { get; set; }
    }

}