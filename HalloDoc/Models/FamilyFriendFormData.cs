using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    public class FamilyFriendFormData : PatientFormData
    {
        [Required (ErrorMessage = "Please enter first name")]
        public string f_firstName { get; set; }

        [Required (ErrorMessage = "Please enter last name")]
        public string f_lastName { get; set; }

        [Required (ErrorMessage = "Please enter email")]
        public string f_Email { get; set; }

        [Required (ErrorMessage = "Please enter Phone Number")]
        public string f_PhoneNumber { get; set; }

        [Required (ErrorMessage = "Please enter relation")]
        public string relationWithPatinet { get; set; }

    }
}
