using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Director
    {
        public int ID { get; set; }

        [Display(Name = "Director")]
        public string DirectorFullName
        {
            get
            {
                return FirstName + " " + LastName;

            }
        }
        [Display(Name = "Emergency Contact Number")]
        public string FormattedContactNumber => "(" + dirPhoneNumber.Substring(0, 3) + ") "
+ dirPhoneNumber.Substring(3, 3) + "-" + dirPhoneNumber[6..];

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string? FirstName { get; set; } = "";

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string? LastName { get; set; } = "";

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]

        [Required(ErrorMessage = "You cannot leave the email blank.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Pick a city from the dropdown or add what you want")]

        public ICollection<DirectorLocation> DirectorLocations { get; set; } = new HashSet<DirectorLocation>();

        [Display(Name = "Director Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no space).")]
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]

        public string? dirPhoneNumber { get; set; }


        public bool IsArchived { get; set; }
    }
}
