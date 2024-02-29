using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class CommonFunctionRepository : ICommonFunctionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestwisefileRepository _requestwisefilerepo;

        public CommonFunctionRepository(ApplicationDbContext context, IRequestwisefileRepository _requestwisefilerepo)
        {
            _context = context;
            _requestwisefilerepo = _requestwisefilerepo;

        }
    

        public void HandleFileUpload(IFormFile UploadFile, int requestId , int? adminId)
        {
            var requestwisefile = new Requestwisefile();
            string FilePath = "wwwroot\\Upload\\" + requestId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
            string UploadImage = "~" + FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + UploadFile.FileName;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadFile.CopyTo(stream);
            }

            requestwisefile.Requestid = requestId;
            requestwisefile.Filename = UploadImage;
            requestwisefile.Createddate = DateTime.Now;
            requestwisefile.Adminid = adminId;
            _context.Requestwisefiles.Add(requestwisefile);
            _context.SaveChanges();
        }

    }
}
