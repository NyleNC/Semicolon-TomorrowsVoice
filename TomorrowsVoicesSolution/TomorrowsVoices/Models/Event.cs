using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Event
    {
        public int ID { get; set; }


        [Display(Name = "Title")]
        [Required(ErrorMessage = "You cannot leave the Event Title blank.")]
        [StringLength(30, ErrorMessage = "Event Title cannot be more than 30 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot be more than 1000 characters long.")]
        public string? Description { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        public int VolLocationID { get; set; }

        public VolLocation VolLocation { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();
    }
}
