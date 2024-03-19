using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
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
        public string? NPInumber { get; set; }
        public string? medicalLicence { get; set; }
        public string? Photo { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public string? Adminnotes { get; set; }
        public bool isAggrementDoc { get; set; }
        public bool isbackgroundDoc { get; set; }
        public bool istrainginDoc {  get; set; }
        public bool isnondisclosuredoc { get; set; }
        public bool islicensedoc { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Zip {get; set;}
        public int regionId { get; set; }
        public int[] regionOfservice { get; set; }
        public string? status { get; set; }
        public string businessName {  get; set; }
        public string businessWebsite { get; set; }

        public string? Altphone { get; set; }

        public string? Createdby { get; set; } = null!;

        public DateTime? Createddate { get; set; }

        public string? Modifiedby { get; set; }

        public DateTime? Modifieddate { get; set; }

        public string? Signature { get; set; }
        public IFormFile? SignatureFile { get; set; }

        public BitArray? Iscredentialdoc { get; set; }

        public BitArray? Istokengenerate { get; set; }

        public string? Syncemailaddress { get; set; }
        public IFormFile? Agreementdoc { get; set; }
        public IFormFile? NonDisclosuredoc { get; set; }
        public IFormFile? Trainingdoc { get; set; }
        public IFormFile? BackGrounddoc { get; set; }
        public IFormFile? Licensedoc { get; set; }

    }
}
