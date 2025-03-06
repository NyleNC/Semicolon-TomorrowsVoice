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

        [Display(Name = "Location")]
        public int? LocationID { get; set; }
        public Location? Location { get; set; }

        [Display(Name = "Is Archived")]
        public bool IsArchived { get; set; }
    }
}
