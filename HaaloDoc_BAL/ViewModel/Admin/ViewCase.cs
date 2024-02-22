
using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Models
{
    public class ViewCase
    {
        [Required]
        public int requestId { get; set; }

        [Required]
        public string symptom { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        [Required]
        public int requesttypeId { get; set; }

        [Required]
        public string dateofBirth { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string phoneNumber { get; set; }

        public int? Region { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string requesterfirstName { get; set; }

        [Required]
        public string requesterlastName { get; set; }

        [Required]
        public string requesterEmail { get; set;}

        [Required]
        public string requesterPhoneNumber { get; set; }

    }
}
