using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.Models
{
    public class Location
    {
        public int ID { get; set; }

        public string? City { get; set; }

        public ICollection<DirectorLocation> DirectorLocations { get; set; } = new HashSet<DirectorLocation>();

        public ICollection<Singer> Singer { get; set; } = new HashSet<Singer>();

        public ICollection<Session> Session { get; set; } = new HashSet<Session>();

    }
}
