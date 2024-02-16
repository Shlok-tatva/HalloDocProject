using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    public class CreateAccountViewModel
    {
        [Required]
        public int requestId {  get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage ="Please Enter The Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage ="Please Enter the Confirm Password")]
        public string? ConfirmPassword { get; set;}


    }
}
