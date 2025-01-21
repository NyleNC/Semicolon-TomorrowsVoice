using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Data
{
    public class TomorrowsVoicesContext : DbContext
    {
        public TomorrowsVoicesContext(DbContextOptions<TomorrowsVoicesContext> options)
            : base(options) 
        {

        }
        //Db sets for all the classes
        DbSet<Director> Directors { get; set; }
        DbSet<Attendance>Attendances { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Note> Notes { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Singer> Singers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // is unique for email
            modelBuilder.Entity<Director>().HasIndex(d=>d.Email).IsUnique();


        }
    }
}
