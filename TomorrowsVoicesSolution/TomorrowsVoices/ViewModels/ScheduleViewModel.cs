using TomorrowsVoices.Models;

namespace TomorrowsVoices.ViewModels
{
    public class ScheduleViewModel
    {
        public List<Schedule> MorningShifts { get; set; } = new List<Schedule>();
        public List<Schedule> AfternoonShifts { get; set; } = new List<Schedule>();
        public List<Schedule> EveningShifts { get; set; } = new List<Schedule>();

      
        // Dictionary to store total hours per volunteer
        public Dictionary<string, double> VolunteerTotalHours { get; set; } = new Dictionary<string, double>();
    }
}
