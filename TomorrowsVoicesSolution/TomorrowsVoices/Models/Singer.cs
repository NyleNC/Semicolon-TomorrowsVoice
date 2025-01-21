using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Singer
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";

        //might not be needed
        //[Required(ErrorMessage = "You must select the date")]
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt{ get;set; }

        public int LocationID { get; set; }
        public Location? Location { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
    }
}
