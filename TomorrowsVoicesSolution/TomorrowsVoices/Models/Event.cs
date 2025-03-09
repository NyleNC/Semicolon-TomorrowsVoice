using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Event
    {
        public int ID { get; set; }


        [Display(Name = "Title")]
        [Required(ErrorMessage = "You cannot leave the Event Title blank.")]
        [StringLength(30, ErrorMessage = "Event Title cannot be more than 30 characters long.")]
        public string? Name { get; set; }

        [Display(Name = "Address")]
        [StringLength(1000, ErrorMessage = "Address cannot be more than 1000 characters long.")]
        public string? Address { get; set; }

        [StringLength(2000, ErrorMessage = "Limit of 2000 characters for notes.")]
        public string? Notes { get; set; }

        [Display(Name = "Date")]
        public DateOnly Date { get; set; }


        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Display(Name = "End Time")]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "Location")]
        public int VolLocationID { get; set; }


        public ICollection<Schedule>? Schedules { get; set; }
        public VolLocation? VolLocation { get; set; }

        public ICollection<VolAttendance> VolAttendance { get; set; } = new HashSet<VolAttendance>();
        public bool IsArchived { get; set; }
    }
}
