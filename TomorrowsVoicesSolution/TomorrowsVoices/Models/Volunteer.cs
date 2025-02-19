using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Volunteer
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string? LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }


        public int VolLocationID { get; set; }

        public VolLocation? VolLocation { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();

    }
}
