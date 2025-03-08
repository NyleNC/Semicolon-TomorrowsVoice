using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Volunteer
    {
        #region Summary Properties
        [Display(Name = "Volunteer")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Emergency Contact Number")]
        public string FormattedPhone => "(" + Phone.Substring(0, 3) + ") "
        + Phone.Substring(3, 3) + "-" + Phone[6..];


        //For lookup values.
        [Display(Name = "Events Attended")]
        public int EventsAttended => VolAttendances.Count(va => va.Status);

        [Display(Name = "Total Hours")]
        public double TotalHours => VolAttendances
            .Where(va => va.ActualStartTime.HasValue && va.ActualEndTime.HasValue)
            .Sum(va => (va.ActualEndTime.Value - va.ActualStartTime.Value).TotalHours);

        #endregion

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
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        public string? Phone { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You cannot leave the email blank.")]
        public string? Email { get; set; } = null;


        public int VolLocationID { get; set; }

        [Display(Name = "Location")]
        public VolLocation? VolLocation { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();
        public bool IsArchived { get; set; }

    }
}

//Now for my volunteer i want to come up with a look up value Volunteer likr for example (How many events showed up, 
//How many hours spent in events, while capturing some model properties: fullname, age, location, email) 
//Something like this or may be something helpful to have an idea of what and how volunteer is doing for each Events on one page.
