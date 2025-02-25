using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class VolAttendance
    {
        public int ID { get; set; }

        [Display(Name = "Scheduled Start Time")]
        public DateTime ScheduledStartTime { get; set; }

        [Display(Name = "Scheduled End Time")]
        public DateTime ScheduledEndTime { get; set; }

        [Display(Name = "Actual Start Time")]
        public DateTime? ActualStartTime { get; set; }

        [Display(Name = "Actual End Time")]
        public DateTime? ActualEndTime { get; set; }

        [Required(ErrorMessage = "You must select the status of the volunteer.")]
        public bool Status { get; set; }

        [StringLength(2000, ErrorMessage = "Limit of 2000 characters for notes.")]
        public string? Notes { get; set; }

        public int? VolunteerID { get; set; }

        public Volunteer? Volunteer { get; set; }

        public int EventID { get; set; }   

        public Event? Event { get; set; }
        public bool IsArchived { get; set; }
    }
}
