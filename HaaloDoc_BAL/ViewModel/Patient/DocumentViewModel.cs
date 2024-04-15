using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.ViewModel.Patient
{
    public class DocumentViewModel
    {
        public int Requestid { get; set; }

        public DateTime uploadDate { get; set; }

        public string fileName { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? UploadImage { get; set; }
        
        public string? confirmationNumber { get; set; }
    }
}
