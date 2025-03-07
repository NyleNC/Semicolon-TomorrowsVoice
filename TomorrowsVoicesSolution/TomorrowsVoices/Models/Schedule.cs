namespace TomorrowsVoices.Models
{
    public class Schedule
    {
        public int ID { get; set; }

   
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }

        public int volunteerID { get; set; }  // Foreign Key
        public Volunteer? Volunteer { get; set; }
        public int eventID { get; set; }
        public Event? Event { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPresent { get; set; }
    }
}
