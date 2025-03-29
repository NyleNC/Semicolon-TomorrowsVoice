
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.ViewModels
{
    public class ScheduleVM
    {
        public int? ScheduleID { get; set; }


 

        [DataType(DataType.Time)]
        public DateTime ScheduledStart { get; set; }

        [DataType(DataType.Time)]
        public DateTime ScheduledEnd { get; set; }

        [Display(Name = "Capacity")]
        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50")]
        public int Capacity { get; set; } = 1; 
    }
}
