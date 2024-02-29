

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class ViewNotesView
    {
        public int requestId {get;set;}

        public string adminNote { get; set; }

        public string physicianNote { get; set; }

        public List<string> transferNotes { get;set;}

        public string adminCancelationNote { get; set; }

        public string physicianCancelationNote { get; set; }

    }
}
