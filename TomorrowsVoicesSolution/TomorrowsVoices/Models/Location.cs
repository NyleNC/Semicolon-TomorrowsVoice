namespace TomorrowsVoices.Models
{
    public class Location
    {
       public int ID { get; set; }
        

        public string? City{ get; set; }

        public int? DirectorID { get; set; }
        public Director? Director { get; set; }

    }
}
