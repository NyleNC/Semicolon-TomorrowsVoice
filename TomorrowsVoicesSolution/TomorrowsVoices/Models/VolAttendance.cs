using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace TomorrowsVoices.Models
{
    public class VolAttendance
    {
        public int ID { get; set; }

        [Display(Name = "Actual Start")]
        public DateTime? ActualStart { get; set; }

        [Display(Name = "Actual End")]
        public DateTime? ActualEnd { get; set; }

        [Required(ErrorMessage = "You must select the status of the volunteer.")]
        public bool Status { get; set; } = true;

        [Display(Name = "Volunteer")]
        public int VolunteerID { get; set; }

        public Volunteer? Volunteer { get; set; }

        [Display(Name = "Schedule")]
        public int VolScheduleID { get; set; }   

        public VolSchedule? VolSchedule { get; set; }

        [Display(Name = "Is Archived")]
        public bool IsArchived { get; set; }
    }
}
