using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Session 
    {
        public int ID { get; set; }

 

        [Required(ErrorMessage = "You must select the session date")]
        [DataType(DataType.Date)]
        public DateTime? Date{ get; set; }

        [Display(Name = "City")]
        public int? LocationID { get; set; }

        public Location? Location { get; set; }
      
        public ICollection<Attendance> Attendance { get; set; } = new HashSet<Attendance>();
    }
}
