using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Event
    {
        public int ID { get; set; }


        [Display(Name = "Title")]
        [Required(ErrorMessage = "You cannot leave the Event Title blank.")]
        [StringLength(30, ErrorMessage = "Event Title cannot be more than 30 characters long.")]
        public string  Name { get; set; }

        [Display(Name = "Location")]
        [StringLength(200, ErrorMessage = "Location cannot be more than 200 characters long.")]
        [Required(ErrorMessage = "Event Location is required and cannot be blank.")]
        public string Location { get; set; }

        [StringLength(2000, ErrorMessage = "Limit of 2000 characters for notes.")]
        public string? Notes { get; set; }

        [Display(Name = "Event Start")]
        public DateTime Start { get; set; }

        [Display(Name = "Event End")]
        public DateTime End { get; set; }

        [Display(Name = "City")]
        public int VolLocationID { get; set; }

        public VolLocation? VolLocation { get; set; }

        public ICollection<VolSchedule> VolSchedules { get; set; } = new HashSet<VolSchedule>();

        [Display(Name = "Is Archived")]
        public bool IsArchived { get; set; }
    }
}
