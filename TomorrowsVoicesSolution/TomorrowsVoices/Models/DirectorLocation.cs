namespace TomorrowsVoices.Models
{
    public class DirectorLocation
    {
        public int DirectorID { get; set; }
        public Director? Director { get; set; }

        public int LocationID { get; set; }
        public Location? Location { get; set; }
    }
}

