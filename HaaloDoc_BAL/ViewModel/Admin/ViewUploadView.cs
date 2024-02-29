

using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class ViewUploadView
    {
        public int Requestid { get; set; }

        public int fileId { get; set; }

        public DateTime uploadDate { get; set; }

        public string fileName { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? UploadImage { get; set; }
    }
}
