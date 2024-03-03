using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class CreateProviderView
    {
        public CreateProviderView()
        {
            regionOfservice = new int[0]; // Initialize as an empty array
        }

        public string firstName {  get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public IFormFile? photo {  get; set; }
        public bool isAggrementDoc { get; set; }
        public bool isbackgroundDoc { get; set; }
        public bool istrainginDoc {  get; set; }
        public bool isnondisclosuredoc { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int regionId { get; set; }
        public int[] regionOfservice { get; set; }
        public string? status { get; set; }
        public string businessName {  get; set; }
        public string businessWebsite { get; set; }

    }
}
