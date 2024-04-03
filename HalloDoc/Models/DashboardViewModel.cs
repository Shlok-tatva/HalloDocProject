namespace HalloDoc.Models
{
    public class DashboardViewModel
    {
        public int Requestid { get; set; }

        public DateTime requestDate { get; set; }

        public int requestStatus { get; set; }


        public string documentPath { get; set; }

        public int DocumentCount { get; set; }

        public string statusString
        {
            get
            {
                switch (requestStatus)
                {
                    case 1:
                        return "Unassigned";
                    case 2:
                        return "Accepted";
                    case 3:
                        return "Cancel by Admin";
                    case 4:
                        return "MDEnRoute";
                    case 5:
                        return "MDONSite";
                    case 6:
                        return "Conclude";
                    case 7:
                        return "Cancelled By Patient";
                    case 8:
                        return "Closed";
                    case 9:
                        return "UnPaid";
                    case 10:
                        return "Requeste Cleared";
                    case 11:
                        return "Blocked Request";
                    default:
                        return "Unknown";
                }
            }
        }
    }
}




