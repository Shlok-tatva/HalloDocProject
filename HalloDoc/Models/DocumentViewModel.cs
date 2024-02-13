namespace HalloDoc.Models
{
    public class DocumentViewModel
    {
        public int Requestid { get; set; }

        public DateTime uploadDate { get; set; }

        public string fileName { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? UploadImage { get; set; }
    }
}
