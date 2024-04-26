using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class FamilyFriendFormData : PatientFormData
    {
        [Required(ErrorMessage = "Please enter first name")]
        public string f_firstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string f_lastName { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string f_Email { get; set; }

        [Required(ErrorMessage = "Please enter Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string f_PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please specify your relation with the patient")]
        public string relationWithPatinet { get; set; }

    }
}
