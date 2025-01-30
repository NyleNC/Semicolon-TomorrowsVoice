using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Location
    {
       public int ID { get; set; }
   
       public City City { get; set; }


        public int? DirectorID { get; set; }


        public Director? Director { get; set; }

        public ICollection<Singer> Singer { get; set; } = new HashSet<Singer>();

        public ICollection<Session> Session { get; set; } = new HashSet<Session>();

    }
}
