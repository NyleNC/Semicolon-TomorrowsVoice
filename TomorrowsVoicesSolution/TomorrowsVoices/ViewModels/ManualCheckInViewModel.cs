namespace TomorrowsVoices.ViewModels
{
    public class ManualCheckInViewModel
    {
        public int AttendanceId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public DateTime ScheduleStart { get; set; }
        public DateTime ScheduleEnd { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;

        public int EventID { get; set; }
    }
}