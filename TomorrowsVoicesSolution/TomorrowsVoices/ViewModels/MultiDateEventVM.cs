using System;
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.ViewModels
{
    public class MultiDateEventVM
    {
        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string EndTime { get; set; }

        public List<ScheduleVM> Shifts { get; set; } = new List<ScheduleVM>();
    }
}
