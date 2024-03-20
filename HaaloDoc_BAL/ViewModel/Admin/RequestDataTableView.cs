using System.ComponentModel.DataAnnotations;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class RequestDataTableView
    {
        [Required]
        public int requestId { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        public string PatientEmail { get; set; }

        [Required]
        public string RequesterEmail { get; set; }

        [Required]
        public string PatientPhoneNumber { get; set; }

        [Required]
        public int status { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        public string PhysicianName {  get; set; }

        [Required]
        public string RequesterName { get; set; }

        [Required]
        public string RequestedDate { get; set;}

        [Required]
        public string RequesterPhoneNumber { get; set; }

        public int? regionId { get; set; }

        [Required]
        public string Address { get; set; }

        public string Notes { get; set; }

        public List<MenuOptionEnum> MenuOptions { get; set; }

        [Required]
        public int RequestTyepid { get; set; }
    }
}

public enum MenuOptionEnum
{
    assignCase,
    cancelCase,
    viewCase,
    viewNotes,
    BlockPatient,
    viewUpload,
    Transfer,
    clearCase,
    sendAgreement,
    orders,
    Encounter,
    closeCase,
    // Add other menu options as needed
}