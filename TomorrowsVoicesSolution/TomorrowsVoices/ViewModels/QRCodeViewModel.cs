namespace TomorrowsVoices.ViewModels
{
    public class VolunteerStatus
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }

    public class QRCodeViewModel
    {
        public string QRCode { get; set; } = string.Empty;
        public string QRCodeImage { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public DateTime ScheduleStart { get; set; }
        public DateTime ScheduleEnd { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        
        public int EventID { get; set; }
        public int TotalVolunteers { get; set; }
        public int CheckedInCount { get; set; }
        public int CheckedOutCount { get; set; }
        public List<VolunteerStatus> Volunteers { get; set; } = new List<VolunteerStatus>();
    }
}