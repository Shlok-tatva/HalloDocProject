using System;
using System.Collections.Generic;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class ShiftView
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPending { get; set; }
        public int ProviderId { get; set; } // Provider ID
        public string ProviderName { get; set; } // Provider Name
        public string ImagePath { get; set; } // Provider Image Path
    }

    public class CalendarViewModel
    {
        public List<ShiftView> Shifts { get; set; }
    }
}
