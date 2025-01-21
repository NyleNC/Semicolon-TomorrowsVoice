using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Note
    {
        public int ID { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "You must select the date")]
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int attendanceID { get; set; }

        public Attendance

    }
}
