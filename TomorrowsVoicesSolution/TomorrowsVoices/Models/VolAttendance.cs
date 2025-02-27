using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class VolAttendance
    {
        public int ID { get; set; }

        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Display(Name = "Scheduled Start Time")]
        public TimeOnly ScheduledStartTime { get; set; }

        [Display(Name = "Scheduled End Time")]
        public TimeOnly ScheduledEndTime { get; set; }

        [Required(ErrorMessage = "You must select the status of the volunteer.")]
        public bool Status { get; set; }


        [Display(Name = "Actual Start Time")]
        public TimeOnly? ActualStartTime { get; set; }

        [Display(Name = "Actual End Time")]
        public TimeOnly? ActualEndTime { get; set; }

        public int? VolunteerID { get; set; }

        public Volunteer? Volunteer { get; set; }

        public int EventID { get; set; }   

        public Event? Event { get; set; }
        public bool IsArchived { get; set; }
    }
}
