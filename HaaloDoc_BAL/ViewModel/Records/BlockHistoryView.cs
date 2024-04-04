using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Records
{
    public class BlockHistoryView
    {
        public int blockRequestId { get; set; }

        public int requestId { get; set; }

        public string patientName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime createdDate { get; set; }

        public string Notes { get; set; } = string.Empty;

        public bool? isActive { get; set; }
    }
}
