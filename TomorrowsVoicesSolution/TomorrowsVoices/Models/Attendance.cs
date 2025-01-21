using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Attendance
    {
        public int ID{ get; set; }

        public bool Status{ get; set; }

        

        //[Required(ErrorMessage = "You must select the date")]
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }


        public int SingerID { get; set; }
        public Singer? Singer { get; set; }

        public int SessionID { get;set; }

        public Session? Session { get; set; }

        public ICollection<Note> Notes { get; set; } = new HashSet<Note>();

    }
}
