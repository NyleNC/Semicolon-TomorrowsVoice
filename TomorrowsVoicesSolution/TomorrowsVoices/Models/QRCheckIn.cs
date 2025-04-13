using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class QRCheckIn
    {
        public int ID { get; set; }

        [Required]
        public int EventID { get; set; }
        public Event? Event { get; set; }

        [Required]
        public int ScheduleID { get; set; }
        public VolSchedule? Schedule { get; set; }

        [Required]
        public string QRCode { get; set; } = string.Empty;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; } = true;
    }
}