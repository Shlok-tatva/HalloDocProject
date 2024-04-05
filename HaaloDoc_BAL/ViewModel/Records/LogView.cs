using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Records
{
    public class LogView
    {

        public string recipientName { get; set; }
        public string Action { get; set; }
        public string roleName { get; set; }
        public string? emailId { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime sentDate { get; set; }
        public bool isSent { get; set; }
        public int sentTries { get; set; }
        public string? confirmationNumber { get; set; }

    }
}
