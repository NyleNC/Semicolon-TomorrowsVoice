using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class VolLocation
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave the City blank.")]
        [StringLength(250, ErrorMessage = "City cannot be more than 250 characters long.")]
        public string City { get; set; } = "";
    }
}