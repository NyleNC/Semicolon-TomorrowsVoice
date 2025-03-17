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
        //Db sets for all the choir administration classes
        public DbSet<Director> Directors { get; set; }
        public DbSet<Attendance>Attendances { get; set; }
         public DbSet<Location> Locations { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Singer> Singers { get; set; }


        //Db sets for all the Volunteer Management classes
        public DbSet<Event> Events  { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<VolAttendance> VolAttendances { get; set; }
        public DbSet<VolLocation> VolLocations { get; set; }

        public DbSet<VolSchedule> VolSchedules { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //one to one relationship between location and director
            modelBuilder.Entity<Location>()
               .HasOne(l => l.Director)
               .WithOne(d => d.Location)
               .HasForeignKey<Location>(l => l.DirectorID)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

            // PREVENT CASCADE Delete FROM VOLUNTEER LOCATION TO VOLUNTEER
            modelBuilder.Entity<VolLocation>()
              .HasMany(l => l.Volunteers)
              .WithOne(d => d.VolLocation)
              .HasForeignKey(d => d.VolLocationID)
              .OnDelete(DeleteBehavior.Restrict);


            // PREVENT CASCADE Delete FROM VOLUNTEER LOCATION TO EVENT
            modelBuilder.Entity<VolLocation>()
              .HasMany(l => l.Events)
              .WithOne(d => d.VolLocation)
              .HasForeignKey(d => d.VolLocationID)
              .OnDelete(DeleteBehavior.Restrict);



            // PREVENT CASCADE Delete FROM Event TO Attendance
            modelBuilder.Entity<Event>()
             .HasMany(l => l.VolSchedules)
             .WithOne(d => d.Event)
             .HasForeignKey(d => d.EventID)
             .OnDelete(DeleteBehavior.Restrict);


            // PREVENT CASCADE Delete FROM Volunteer TO Attendance
            modelBuilder.Entity<Volunteer>()
             .HasMany(l => l.VolAttendances)
             .WithOne(d => d.Volunteer)
             .HasForeignKey(d => d.VolunteerID)
             .OnDelete(DeleteBehavior.Restrict);


            // One-to-Many: Location -> Singers
            modelBuilder.Entity<Singer>()
                .HasOne(s => s.Location)
                .WithMany(l => l.Singer)
                .HasForeignKey(s => s.LocationID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Singer)
                .WithMany(s => s.Attendance)
                .HasForeignKey(a => a.SingerID)
                  .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(Session => Session.Location)
                .WithMany(Location => Location.Session)
                .HasForeignKey(Session => Session.LocationID)
             .OnDelete(DeleteBehavior.Restrict);

            // is unique for email
            modelBuilder.Entity<Director>()

                .HasIndex(d => d.Email)
                .IsUnique();
        }
    }
}
