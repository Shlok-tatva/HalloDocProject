namespace HalloDoc.Models
{
    public class DashboardViewModel
    {
        public DateTime requestDate { get; set; }

        public int requestStatus { get; set; }

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
                        return "Cancelled";
                    case 4:
                        return "Reserving";
                    case 5:
                        return "MDEnRoute";
                    case 6:
                        return "MDOnSite";
                    case 7:
                        return "FollowUp";
                    case 8:
                        return "Closed";
                    case 9:
                        return "Locked";
                    case 10:
                        return "Declined";
                    case 11:
                        return "Consult";
                    case 12:
                        return "Clear";
                    case 13:
                        return "CancelledByProvider";
                    case 14:
                        return "CCUploadedByClient";
                    case 15:
                        return "CCApprovedByAdmin";
                    default:
                        return "Unknown";
                }
            }
        }

        public string documentPath { get; set; }
    }
}



