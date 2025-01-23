using Microsoft.EntityFrameworkCore;
using System.Numerics;
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
        public DbSet<Director> Directors { get; set; }
        public DbSet<Attendance>Attendances { get; set; }
         public DbSet<Location> Locations { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Singer> Singers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Location>()
               .HasOne(l => l.Director)
               .WithOne(d => d.Location)
               .HasForeignKey<Location>(l => l.DirectorID)
               .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Location -> Singers
            modelBuilder.Entity<Singer>()
                .HasOne(s => s.Location)
                .WithMany(l => l.Singers)
                .HasForeignKey(s => s.LocationID)
                .OnDelete(DeleteBehavior.Restrict);

         
      
            // is unique for email
            modelBuilder.Entity<Director>()
                .HasIndex(d=>d.Email)
                .IsUnique();



        }
    }
}
