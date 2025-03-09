using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Schedule
    {
        public int ID { get; set; }

   
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }

        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Display(Name = "Scheduled Start Time")]
        public TimeOnly ScheduledStartTime { get; set; }

        [Display(Name = "Scheduled End Time")]
        public TimeOnly ScheduledEndTime { get; set; }


        [Display(Name = "Actual Start Time")]
        public TimeOnly? ActualStartTime { get; set; }

        [Display(Name = "Actual End Time")]
        public TimeOnly? ActualEndTime { get; set; }

        [Required(ErrorMessage = "You must select the status of the volunteer.")]
        public bool Status { get; set; }


        public int volunteerID { get; set; }  // Foreign Key
        public Volunteer? Volunteer { get; set; }
        public int eventID { get; set; }
        public Event? Event { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPresent { get; set; }

        public ICollection<VolAttendance> VolAttendance { get; set; } = new HashSet<VolAttendance>();
    }
}
