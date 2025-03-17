namespace TomorrowsVoices.ViewModels
{
    public class ScheduleVM
    {
        public int? ScheduleID { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public List<int> VolunteerIds { get; set; } = new List<int>();
    }
}
