using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Records
{
    public class PatientRequestes
    {
        public int requestId { get; set; }
        public string patientName { get; set; }
        public string createdDate { get; set; }
        public string? conformationNumber { get; set; }
        public string? providerName { get; set; }
    }
}
