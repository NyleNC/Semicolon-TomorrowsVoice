using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Director
    {
        public int ID { get; set; }

        [Display(Name ="Director")]
        public string DirectorFullName
        {
            get
            {
                return FirstName +" "+LastName;
               
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = ""; 

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        //might not be needed
        ////[Required(ErrorMessage = "You must select the date")]
        ////public DateTime? CreatedAt { get; set; }
        ////public DateTime? UpdatedAt{ get; private set; }

        public Location? Location { get; set; }
        public ICollection<Singer> Singers { get; set; } = new HashSet<Singer>();

    }
}
