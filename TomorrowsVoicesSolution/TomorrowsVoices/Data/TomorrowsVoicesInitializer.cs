using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Data
{
    public static class TomorrowsVoicesInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider, bool DeleteDatabase, bool UseMigrations, bool SeedSampleData)
        {
            using (var context = new TomorrowsVoicesContext(
                serviceProvider.GetRequiredService<DbContextOptions<TomorrowsVoicesContext>>()))
            {
                // Refresh the database as per the parameter options
                #region Prepare the Database
                try
                {
                    // If DeleteDatabase is true or the database is not accessible, delete and recreate
                    if (DeleteDatabase || !context.Database.CanConnect())
                    {
                        context.Database.EnsureDeleted(); // Delete the existing database

                        if (UseMigrations)
                        {
                            context.Database.Migrate(); // Apply all migrations
                        }
                        else
                        {
                            context.Database.EnsureCreated(); // Create the database using the model
                        }
                    }
                    else if (UseMigrations)
                    {
                        context.Database.Migrate(); // Apply migrations if the database is already created
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                // Seed Data if the SeedSampleData flag is true
                if (SeedSampleData)
                {
                    SeedDirectors(context);
                    SeedLocations(context);
                    SeedSingers(context);
                    SeedSessions(context);
                    SeedAttendances(context);
                    SeedVolLocation(context);
                    SeedVolunteers(context);
                    SeedEvents(context);
                }
            }
        }

        private static void SeedDirectors(TomorrowsVoicesContext context)
        {
            if (!context.Directors.Any())
            {
                context.Directors.AddRange(
                    new Director
                    {
                        FirstName = "Tom",
                        LastName = "Ronton",
                        Email = "tronton@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Niam",
                        LastName = "Garrison",
                        Email = "ngarrison@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Sasha",
                        LastName = "Katherine",
                        Email = "skatherine@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Vanda",
                        LastName = "Cooper",
                        Email = "vcooper@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Ham",
                        LastName = "Elton",
                        Email = "helton@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Serg",
                        LastName = "Urls",
                        Email = "surls@tv.com"
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedLocations(TomorrowsVoicesContext context)
        {
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    new Location
                    {
                        City = "Toronto",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Tom" && d.LastName == "Ronton").ID
                    },
                    new Location
                    {
                        City = "Saskatoon",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Sasha" && d.LastName == "Katherine").ID
                    },
                    new Location
                    {
                        City = "Niagara",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Niam" && d.LastName == "Garrison").ID
                    },
                    new Location
                    {
                        City ="Vancouver",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Vanda" && d.LastName == "Cooper").ID
                    },
                    new Location
                    {
                        City = "Hamilton",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Ham" && d.LastName == "Elton").ID
                    },
                    new Location
                    {
                        City = "Surrey",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Serg" && d.LastName == "Urls").ID
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedSingers(TomorrowsVoicesContext context)
        {
            if (!context.Singers.Any())
            {
                context.Singers.AddRange(

                    // Niagara
                    new Singer
                    {
                        FirstName = "Bruce",
                        LastName = "House",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Niagara").ID,
                        EmergencyContactName = "John House",
                        EmergencyContactNumber = "1234567890"
                    },
                    new Singer
                    {
                        FirstName = "Torffin",
                        LastName = "Snow",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Niagara").ID,
                        EmergencyContactName = "Jane Snow",
                        EmergencyContactNumber = "0987654321"
                    },
                    new Singer
                    {
                        FirstName = "Hall",
                        LastName = "Houser",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Niagara").ID,
                        EmergencyContactName = "Mary Houser",
                        EmergencyContactNumber = "1122334455"
                    },
                    new Singer
                    {
                        FirstName = "Radin",
                        LastName = "Shahravan",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Niagara").ID,
                        EmergencyContactName = "Ali Shahravan",
                        EmergencyContactNumber = "2233445566"
                    }

                    // Toronto
                    ,
                    new Singer
                    {
                        FirstName = "Tyler",
                        LastName = "Klassen",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                        EmergencyContactName = "Sarah Klassen",
                        EmergencyContactNumber = "3344556677"
                    },
                    new Singer
                    {
                        FirstName = "James",
                        LastName = "Rodrigo",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                        EmergencyContactName = "Maria Rodrigo",
                        EmergencyContactNumber = "4455667788"
                    },
                    new Singer
                    {
                        FirstName = "Theo",
                        LastName = "Baker",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                        EmergencyContactName = "Tom Baker",
                        EmergencyContactNumber = "5566778899"
                    },

                    // Saskatoon
                    new Singer
                    {
                        FirstName = "Logan",
                        LastName = "Xavier",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                        EmergencyContactName = "Laura Xavier",
                        EmergencyContactNumber = "6677889900"
                    }


                );
                context.SaveChanges();
            }
        }

        private static void SeedSessions(TomorrowsVoicesContext context)
        {
            if (!context.Sessions.Any())
            {
                context.Sessions.AddRange(
                    new Session
                    {
                        Date = DateTime.Parse("2024/12/29"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Niagara").ID,
              
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedAttendances(TomorrowsVoicesContext context)
        {
            if (!context.Attendances.Any())
            {
        
                context.Attendances.AddRange(
                    new Attendance
                    {
                        Status = true,
                        SingerID =  context.Singers.FirstOrDefault(s => s.FirstName == "Radin").ID,
                        SessionID = context.Sessions.FirstOrDefault(s => s.Location.City == "Niagara").ID
                    },
                    new Attendance
                    {
                        Status = true,
                        SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Hall").ID,
                        SessionID = context.Sessions.FirstOrDefault(s => s.Location.City == "Niagara").ID
                    },
                    new Attendance
                    {
                        Status = false,
                        SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Torffin").ID,
                        SessionID = context.Sessions.FirstOrDefault(s => s.Location.City == "Niagara").ID
                    },
                    new Attendance
                    {
                        Status = true,
                        SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Bruce").ID,
                        SessionID = context.Sessions.FirstOrDefault(s => s.Location.City == "Niagara").ID
                    }

                );
                context.SaveChanges();
            }
            
        }


        // Volunteer Management Seed Data

        private static void SeedVolLocation(TomorrowsVoicesContext context)
        {
            if (!context.VolLocations.Any())
            {
                context.VolLocations.AddRange(
                    new VolLocation { City = "Toronto" },
                    new VolLocation { City = "Niagara Falls" }
                );
                context.SaveChanges();
            }
        }


        private static void SeedVolunteers(TomorrowsVoicesContext context)
        {
            if (!context.Volunteers.Any())
            {
                context.Volunteers.AddRange(
                    new Volunteer { FirstName = "John", LastName = "Doe", Phone = "1234567890", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Jane", LastName = "Smith", Phone = "2345678901", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Alice", LastName = "Johnson", Phone = "3456789012", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Bob", LastName = "Brown", Phone = "4567890123", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Charlie", LastName = "Davis", Phone = "5678901234", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "David", LastName = "Miller", Phone = "6789012345", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Eve", LastName = "Wilson", Phone = "7890123456", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Frank", LastName = "Moore", Phone = "8901234567", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Grace", LastName = "Taylor", Phone = "9012345678", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Hank", LastName = "Anderson", Phone = "0123456789", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Ivy", LastName = "Thomas", Phone = "1234567890", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Jack", LastName = "Jackson", Phone = "2345678901", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Karen", LastName = "White", Phone = "3456789012", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Leo", LastName = "Harris", Phone = "4567890123", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Mia", LastName = "Martin", Phone = "5678901234", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Nina", LastName = "Thompson", Phone = "6789012345", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Oscar", LastName = "Garcia", Phone = "7890123456", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Paul", LastName = "Martinez", Phone = "8901234567", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Quinn", LastName = "Robinson", Phone = "9012345678", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Rose", LastName = "Clark", Phone = "0123456789", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Sam", LastName = "Rodriguez", Phone = "1234567890", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Tina", LastName = "Lewis", Phone = "2345678901", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Uma", LastName = "Lee", Phone = "3456789012", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Vince", LastName = "Walker", Phone = "4567890123", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Wendy", LastName = "Hall", Phone = "5678901234", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 }
                );
                context.SaveChanges();
            }
        }


        private static void SeedEvents(TomorrowsVoicesContext context)
        {
            if (!context.Events.Any())
            {
                context.Events.AddRange(
                    new Event { Name = "Community Cleanup", Description = "A community cleanup event.", StartTime = DateTime.Parse("2024-02-20 09:00"), EndTime = DateTime.Parse("2024-02-20 12:00"), VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto").ID },
                    new Event { Name = "Food Drive", Description = "A food drive for the local food bank.", StartTime = DateTime.Parse("2024-03-10 10:00"), EndTime = DateTime.Parse("2024-03-10 14:00"), VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto").ID },
                    new Event { Name = "Blood Donation Camp", Description = "A blood donation camp.", StartTime = DateTime.Parse("2024-04-05 08:00"), EndTime = DateTime.Parse("2024-04-05 13:00"), VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto").ID }
                );
                context.SaveChanges();
            }
        }
    }
}
