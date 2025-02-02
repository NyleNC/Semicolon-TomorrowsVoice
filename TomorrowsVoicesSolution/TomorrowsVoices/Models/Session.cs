using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Session : IValidatableObject
    {
        public int ID { get; set; }

 

        [Required(ErrorMessage = "You must select the session date")]
        [DataType(DataType.Date)]
        public DateTime? Date{ get; set; }

        [MaxLength(2000, ErrorMessage = "Limit of 2000 characters for notes.")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = "";


        [Display(Name = "City")]
        public int? LocationID { get; set; }

        public Location? Location { get; set; }
      
        public ICollection<Attendance> Attendance { get; set; } = new HashSet<Attendance>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date.HasValue && Date.Value > DateTime.Today)
            {
                yield return new ValidationResult("Session date cannot be in the future.", new[] { nameof(Date) });
            }
        }
    }
}
