namespace TomorrowsVoices.Models
{
    public class VolAttendance
    {
        public int ID { get; set; }

        public DateTime ScheduledStartTime { get; set; }

        public DateTime ScheduledEndTime { get; set; }


        public DateTime? ActualStartTime { get; set; }


        public DateTime? ActualEndTime { get; set; }


        public bool Status { get; set; }
        

        public string? Notes { get; set; }


        public int? VolunteerID { get; set; }

        public Volunteer? Volunteer { get; set; }

        public int EventID { get; set; }   

        public Event Event { get; set; }   
    }
}
