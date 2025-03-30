using TomorrowsVoices.Models;

namespace TomorrowsVoices.ViewModels
{
    public class EventEditVM
    {
        public Event Event { get; set; }
        public ScheduleVM NewSchedule { get; set; }
        public List<ScheduleVM> ExistingSchedules { get; set; } = new List<ScheduleVM>();
        public List<MultiDateEventVM> MultiDateEvents { get; set; } = new List<MultiDateEventVM>();
    }
}