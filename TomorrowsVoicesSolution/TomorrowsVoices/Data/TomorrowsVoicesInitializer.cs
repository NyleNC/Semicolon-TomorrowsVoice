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
                        City = City.Toronto,
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Tom" && d.LastName == "Ronton").ID
                    },
                    new Location
                    {
                        City = City.Saskatoon,
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Sasha" && d.LastName == "Katherine").ID
                    },
                    new Location
                    {
                        City = City.Niagara,
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Niam" && d.LastName == "Garrison").ID
                    },
                    new Location
                    {
                        City = City.Vancouver,
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Vanda" && d.LastName == "Cooper").ID
                    },
                    new Location
                    {
                        City = City.Hamilton,
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Ham" && d.LastName == "Elton").ID
                    },
                    new Location
                    {
                        City = City.Surrey,
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
                    new Singer
                    {
                        FirstName = "Bruce",
                        LastName = "House",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Toronto).ID
                    },
                    new Singer
                    {
                        FirstName = "Tyler",
                        LastName = "Klassen",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Toronto).ID
                    },
                    new Singer
                    {
                        FirstName = "James",
                        LastName = "Rodrigo",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Toronto).ID
                    },
                    new Singer
                    {
                        FirstName = "Theo",
                        LastName = "Baker",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Toronto).ID
                    },
                    new Singer
                    {
                        FirstName = "Hall",
                        LastName = "Houser",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Niagara).ID
                    },
                    new Singer
                    {
                        FirstName = "Radin",
                        LastName = "Shahravan",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Niagara).ID
                    },
                    new Singer
                    {
                        FirstName = "Logan",
                        LastName = "Xavier",
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Saskatoon).ID
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
                        LocationID = context.Locations.FirstOrDefault(l => l.City == City.Niagara).ID
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedAttendances(TomorrowsVoicesContext context)
        {
            if (!context.Attendances.Any())
            {
                var radin = context.Singers.FirstOrDefault(s => s.FirstName == "Radin");
                var session = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2024/12/29"));

                if (radin != null && session != null)
                {
                    context.Attendances.AddRange(
                        new Attendance
                        {
                            Status = true,
                            SingerID = radin.ID,
                            SessionID = session.ID
                        },
                        new Attendance
                        {
                            Status = true,
                            SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Hall").ID,
                            SessionID = session.ID
                        },
                        new Attendance
                        {
                            Status = false,
                            SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Logan").ID,
                            SessionID = session.ID
                        },
                        new Attendance
                        {
                            Status = true,
                            SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Tyler").ID,
                            SessionID = session.ID
                        },
                        new Attendance
                        {
                            Status = false,
                            SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "Theo").ID,
                            SessionID = session.ID
                        },
                        new Attendance
                        {
                            Status = true,
                            SingerID = context.Singers.FirstOrDefault(s => s.FirstName == "James").ID,
                            SessionID = session.ID
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
