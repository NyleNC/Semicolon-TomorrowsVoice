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
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Emergency Contact Number")]
        public string FormattedContactNumber => "(" + EmergencyContactNumber.Substring(0, 3) + ") "
        + EmergencyContactNumber.Substring(3, 3) + "-" + EmergencyContactNumber[6..];

        #endregion

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;


        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedOn { get; set; }


        // Fields for Emergency details
        [Display(Name = "Parent/Guardian Name")]
        [Required(ErrorMessage = "Guardian/Parents Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
        public string? EmergencyContactName { get; set; }

        [Display(Name = "Parent/Guardian Contact Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        //[MaxLength(10)]
        public string? EmergencyContactNumber { get; set; }




        [Display(Name = "City")]
        [Required(ErrorMessage = "You must select a location.")]
        public int LocationID { get; set; }
        public Location? Location { get; set; }

        public ICollection<Attendance> Attendance { get; set; } = new HashSet<Attendance>();
    }
}
