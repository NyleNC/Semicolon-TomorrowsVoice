namespace TomorrowsVoices.Models
{
    public class Event
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int VolLocationID { get; set; }

        public VolLocation VolLocation { get; set; }

        public ICollection<VolAttendance> VolAttendances { get; set; } = new HashSet<VolAttendance>();
    }
}
