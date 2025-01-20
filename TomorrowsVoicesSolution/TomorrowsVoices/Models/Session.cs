using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Session
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You must select the date")]
        public bool? Status { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "You must select a session date")]
        public DateTime? Date{ get; set; }

        public int LocationID { get; set; }

        public Location? Location { get; set; }
      
    }
}
