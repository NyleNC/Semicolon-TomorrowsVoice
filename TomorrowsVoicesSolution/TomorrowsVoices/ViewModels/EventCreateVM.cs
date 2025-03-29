using TomorrowsVoices.Models;

namespace TomorrowsVoices.ViewModels
{
    public class EventCreateVM
    {
        public Event Event { get; set; }
        public ScheduleVM NewSchedule { get; set; }
        public List<ScheduleVM> ExistingSchedules { get; set; } = new List<ScheduleVM>();
        public List<MultiDateEventVM> MultiDateEvents { get; set; } = new List<MultiDateEventVM>();
    }
}
