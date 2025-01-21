using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Attendance
    {
        public int ID{ get; set; }

        public Status Status{ get; set; }

        public string? Note {  get; set; }

       /// <summary>
       /// created and update might not be needed
       /// </summary>

        //[Required(ErrorMessage = "You must select the date")]
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }


        public int SingerID { get; set; }
        public Singer? Singer { get; set; }

    }
}
