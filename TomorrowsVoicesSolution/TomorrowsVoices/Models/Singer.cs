using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Singer
    {
        public int ID { get; set; }

        #region Summary Properties

        [Display(Name = "Singer")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }


     
        #endregion

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";

        [Display(Name = "City")]
        public int LocationID { get; set; }
        public Location? Location { get; set; }

        public ICollection<Attendance> Attendance { get; set; } = new HashSet<Attendance>();
    }
}
