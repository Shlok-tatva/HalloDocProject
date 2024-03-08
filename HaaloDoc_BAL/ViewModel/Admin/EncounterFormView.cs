using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class EncounterFormView
    {
        public int requestId { get; set; }
        public int? adminId { get; set; }
        public int? physicianId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string location { get; set; }
        public string dateOfBirth { get; set; }
        public string dateOfRequest { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string? historyOfPresentIllnessOrInjury { get; set; } = "";
        public string? medicalHistory { get; set; } = "";
        public string? medications { get; set; } = "";
        public string? allergies { get; set; }
        public string? temp { get; set; } = "";
        public string? hr { get; set; } = "";
        public string? rr { get; set; } = "";    
        public string? bloodPressureSystolic { get; set; } = "";
        public string? bloodPressureDiastolic { get; set; } = "";
        public string? o2 { get; set; } = "";
        public string? pain { get; set; } = "";
        public string? heent { get; set; } = ""; 
        public string? cv { get; set; } = "";
        public string? chest { get; set; } = "";
        public string? abd { get; set; } = "";
        public string? extremities { get; set; } = "";   
        public string? skin { get; set; } = "";
        public string? neuro { get; set; } = "";
        public string? other { get; set; } = "";
        public string? diagnosis { get; set; } = "";
        public string? treatmentPlan { get; set; } = "";
        public string? medicalDispensed { get; set; } = "";
        public string? procedures { get; set; } = "";
        public string? followup { get; set; } = "";
        public int isFinalize { get; set; }
    }
}
