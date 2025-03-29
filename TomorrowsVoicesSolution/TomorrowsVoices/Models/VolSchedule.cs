using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class VolSchedule
    {
        public int ID { get; set; }

        [Display(Name = "Shift Date")]
        [DataType(DataType.Date)]
        public DateTime ShiftDate { get; set; }

        [Display(Name = "Shift Start")]
        [DataType(DataType.Time)]
        public DateTime ScheduledStart { get; set; }

        [Display(Name = "Shift End")]
        [DataType(DataType.Time)]
        public DateTime ScheduledEnd { get; set; }

        [Display(Name = "Spots Available")]
        public int Capacity { get; set; }


        [Display(Name = "Event")]
        public int EventID { get; set; }

        public Event? Event { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();


    }
}
