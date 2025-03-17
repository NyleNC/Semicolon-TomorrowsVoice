using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class VolSchedule
    {
        public int ID { get; set; }

        [Display(Name = "Scheduled Start")]
        public DateTime ScheduledStart { get; set; }

        [Display(Name = "Scheduled End")]
        public DateTime ScheduledEnd { get; set; }

        [Display(Name = "Event")]
        public int EventID { get; set; }

        public Event? Event { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();


    }
}
