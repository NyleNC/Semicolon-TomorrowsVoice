using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
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
                    // If DeleteDatabase is true or the database is not accessible, Delete and recreate
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
                    //SeedVolAttendances(context);
                    SeedSchedules(context);
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
                        FirstName = "Anais",
                        LastName = "Kelsey-Verdecchia",
                        Email = "averdecchia@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Brian",
                        LastName = "Paul",
                        Email = "bpaul@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Mendelt",
                        LastName = "Hoekstra",
                        Email = "menhoekstra@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Monique",
                        LastName = "Hoekstra",
                        Email = "monhoekstra@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Melissa",
                        LastName = "Dutch",
                        Email = "mdutch@tv.com"
                    },
                    new Director
                    {
                        FirstName = "Frances",
                        LastName = "Olson",
                        Email = "folson@tv.com"
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
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Anais" && d.LastName == "Kelsey-Verdecchia").ID
                    },
                    new Location
                    {
                        City = "Saskatoon",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Brian" && d.LastName == "Paul").ID
                    },
                    new Location
                    {
                        City = "St. Catharines",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Mendelt" && d.LastName == "Hoekstra").ID
                    },
                    new Location
                    {
                        City ="Vancouver",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Monique" && d.LastName == "Hoekstra").ID
                    },
                    new Location
                    {
                        City = "Hamilton",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Melissa" && d.LastName == "Dutch").ID
                    },
                    new Location
                    {
                        City = "Surrey",
                        DirectorID = context.Directors.FirstOrDefault(d => d.FirstName == "Frances" && d.LastName == "Olson").ID
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

            // Toronto
            new Singer
            {
                FirstName = "Michael",
                LastName = "Johnson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Sarah Johnson",
                EmergencyContactNumber = "6471234567"
            },
            new Singer
            {
                FirstName = "Emily",
                LastName = "Clark",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "David Clark",
                EmergencyContactNumber = "6479876543"
            },
            new Singer
            {
                FirstName = "Nathan",
                LastName = "Williams",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Laura Williams",
                EmergencyContactNumber = "4165557890"
            },
            new Singer
            {
                FirstName = "Sophia",
                LastName = "Martinez",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Carlos Martinez",
                EmergencyContactNumber = "4167778888"
            },
            new Singer
            {
                FirstName = "Daniel",
                LastName = "Brown",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Linda Brown",
                EmergencyContactNumber = "6472223333"
            },
            new Singer
            {
                FirstName = "Ava",
                LastName = "Taylor",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Robert Taylor",
                EmergencyContactNumber = "4169990000"
            },
            new Singer
            {
                FirstName = "Lucas",
                LastName = "Harris",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Emma Harris",
                EmergencyContactNumber = "6475556666"
            },
            new Singer
            {
                FirstName = "Olivia",
                LastName = "Moore",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Henry Moore",
                EmergencyContactNumber = "4163334444"
            },
            new Singer
            {
                FirstName = "Ethan",
                LastName = "White",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Sophia White",
                EmergencyContactNumber = "6477778888"
            },
            new Singer
            {
                FirstName = "Charlotte",
                LastName = "Lee",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                EmergencyContactName = "Daniel Lee",
                EmergencyContactNumber = "4161112222"
            },

            // Saskatoon
            new Singer
            {
                FirstName = "Benjamin",
                LastName = "Adams",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Megan Adams",
                EmergencyContactNumber = "3065551234"
            },
            new Singer
            {
                FirstName = "Emma",
                LastName = "Scott",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "James Scott",
                EmergencyContactNumber = "3064445678"
            },
            new Singer
            {
                FirstName = "Liam",
                LastName = "Cooper",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Natalie Cooper",
                EmergencyContactNumber = "3068889999"
            },
            new Singer
            {
                FirstName = "Isabella",
                LastName = "Fisher",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Ethan Fisher",
                EmergencyContactNumber = "3062223333"
            },
            new Singer
            {
                FirstName = "Mason",
                LastName = "Hall",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Ava Hall",
                EmergencyContactNumber = "3067770000"
            },
            new Singer
            {
                FirstName = "Grace",
                LastName = "Carter",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Logan Carter",
                EmergencyContactNumber = "3069991111"
            },
            new Singer
            {
                FirstName = "Jacob",
                LastName = "Reed",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Emily Reed",
                EmergencyContactNumber = "3065556666"
            },
            new Singer
            {
                FirstName = "Aiden",
                LastName = "Graham",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Sophia Graham",
                EmergencyContactNumber = "3064447777"
            },
            new Singer
            {
                FirstName = "Lily",
                LastName = "Parker",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Benjamin Parker",
                EmergencyContactNumber = "3063338888"
            },
            new Singer
            {
                FirstName = "Noah",
                LastName = "Ward",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                EmergencyContactName = "Isabella Ward",
                EmergencyContactNumber = "3061112222"
            },

            // Surrey
            new Singer
            {
                FirstName = "William",
                LastName = "Davis",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Hannah Davis",
                EmergencyContactNumber = "6045551234"
            },
            new Singer
            {
                FirstName = "Chloe",
                LastName = "Bennett",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Lucas Bennett",
                EmergencyContactNumber = "6044445678"
            },
            new Singer
            {
                FirstName = "Henry",
                LastName = "Wilson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Ava Wilson",
                EmergencyContactNumber = "6048889999"
            },
            new Singer
            {
                FirstName = "Ella",
                LastName = "Robinson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Ethan Robinson",
                EmergencyContactNumber = "6042223333"
            },
            new Singer
            {
                FirstName = "Sebastian",
                LastName = "Walker",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Mia Walker",
                EmergencyContactNumber = "6047770000"
            },
            new Singer
            {
                FirstName = "Madison",
                LastName = "King",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Alexander King",
                EmergencyContactNumber = "6049991111"
            },
            new Singer
            {
                FirstName = "David",
                LastName = "Evans",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Charlotte Evans",
                EmergencyContactNumber = "6045556666"
            },
            new Singer
            {
                FirstName = "Mila",
                LastName = "Cruz",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "James Cruz",
                EmergencyContactNumber = "6044447777"
            },
            new Singer
            {
                FirstName = "Samuel",
                LastName = "Mitchell",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Sophia Mitchell",
                EmergencyContactNumber = "6043338888"
            },
            new Singer
            {
                FirstName = "Zoe",
                LastName = "Nelson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                EmergencyContactName = "Benjamin Nelson",
                EmergencyContactNumber = "6041112222"
            },
            // Vancouver
            new Singer
            {
                FirstName = "James",
                LastName = "Anderson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Emily Anderson",
                EmergencyContactNumber = "6041234567"
            },
            new Singer
            {
                FirstName = "Sophia",
                LastName = "Baker",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Daniel Baker",
                EmergencyContactNumber = "6049876543"
            },
            new Singer
            {
                FirstName = "Ethan",
                LastName = "Wright",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Laura Wright",
                EmergencyContactNumber = "6045557890"
            },
            new Singer
            {
                FirstName = "Ava",
                LastName = "Hill",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Christopher Hill",
                EmergencyContactNumber = "6047778888"
            },
            new Singer
            {
                FirstName = "Benjamin",
                LastName = "Turner",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Olivia Turner",
                EmergencyContactNumber = "6042223333"
            },
            new Singer
            {
                FirstName = "Mia",
                LastName = "Adams",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Matthew Adams",
                EmergencyContactNumber = "6049990000"
            },
            new Singer
            {
                FirstName = "Noah",
                LastName = "Morris",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Hannah Morris",
                EmergencyContactNumber = "6045556666"
            },
            new Singer
            {
                FirstName = "Charlotte",
                LastName = "Phillips",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Jacob Phillips",
                EmergencyContactNumber = "6043334444"
            },
            new Singer
            {
                FirstName = "Liam",
                LastName = "Stewart",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "Sophia Stewart",
                EmergencyContactNumber = "6047778888"
            },
            new Singer
            {
                FirstName = "Isabella",
                LastName = "Mitchell",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                EmergencyContactName = "William Mitchell",
                EmergencyContactNumber = "6041112222"
            },

            // Hamilton
            new Singer
            {
                FirstName = "Mason",
                LastName = "Carter",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Ella Carter",
                EmergencyContactNumber = "9055551234"
            },
            new Singer
            {
                FirstName = "Emma",
                LastName = "Parker",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Benjamin Parker",
                EmergencyContactNumber = "9054445678"
            },
            new Singer
            {
                FirstName = "Jacob",
                LastName = "Cooper",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Grace Cooper",
                EmergencyContactNumber = "9058889999"
            },
            new Singer
            {
                FirstName = "Aiden",
                LastName = "Ward",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Isabella Ward",
                EmergencyContactNumber = "9052223333"
            },
            new Singer
            {
                FirstName = "Lily",
                LastName = "Foster",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Nathan Foster",
                EmergencyContactNumber = "9057770000"
            },
            new Singer
            {
                FirstName = "Lucas",
                LastName = "Reed",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Sophia Reed",
                EmergencyContactNumber = "9059991111"
            },
            new Singer
            {
                FirstName = "Olivia",
                LastName = "Gray",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Daniel Gray",
                EmergencyContactNumber = "9055556666"
            },
            new Singer
            {
                FirstName = "Ethan",
                LastName = "Bell",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Charlotte Bell",
                EmergencyContactNumber = "9054447777"
            },
            new Singer
            {
                FirstName = "Zoe",
                LastName = "Howard",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "James Howard",
                EmergencyContactNumber = "9053338888"
            },
            new Singer
            {
                FirstName = "William",
                LastName = "Murphy",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                EmergencyContactName = "Emma Murphy",
                EmergencyContactNumber = "9051112222"
            },

            // St. Catharines
            new Singer
            {
                FirstName = "Henry",
                LastName = "Collins",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Ava Collins",
                EmergencyContactNumber = "2895551234"
            },
            new Singer
            {
                FirstName = "Ella",
                LastName = "Ramirez",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Ethan Ramirez",
                EmergencyContactNumber = "2894445678"
            },
            new Singer
            {
                FirstName = "Sebastian",
                LastName = "Fisher",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Lily Fisher",
                EmergencyContactNumber = "2898889999"
            },
            new Singer
            {
                FirstName = "Madison",
                LastName = "Ross",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Jacob Ross",
                EmergencyContactNumber = "2892223333"
            },
            new Singer
            {
                FirstName = "David",
                LastName = "Evans",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Charlotte Evans",
                EmergencyContactNumber = "2897770000"
            },
            new Singer
            {
                FirstName = "Mila",
                LastName = "Hughes",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "James Hughes",
                EmergencyContactNumber = "2899991111"
            },
            new Singer
            {
                FirstName = "Samuel",
                LastName = "Bennett",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Sophia Bennett",
                EmergencyContactNumber = "2895556666"
            },
            new Singer
            {
                FirstName = "Zoe",
                LastName = "Cruz",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Benjamin Cruz",
                EmergencyContactNumber = "2894447777"
            },
            new Singer
            {
                FirstName = "Michael",
                LastName = "Nelson",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Emma Nelson",
                EmergencyContactNumber = "2893338888"
            },
            new Singer
            {
                FirstName = "Harper",
                LastName = "Scott",
                LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                EmergencyContactName = "Daniel Scott",
                EmergencyContactNumber = "2891112222"
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
                    // Toronto
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Toronto").ID,
                    }
                    ,
                    // Saskatoon
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Saskatoon").ID,
                    }
                    ,


                    // Surrey
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Surrey").ID,
                    }
                    ,

                    // Vancouver
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Vancouver").ID,
                    },
                    // Hamilton
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "Hamilton").ID,
                    },

                    // St. Catharines
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/14"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
                    },
                    new Session
                    {
                        Date = DateTime.Parse("2025/02/21"),
                        LocationID = context.Locations.FirstOrDefault(l => l.City == "St. Catharines").ID,
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


                // Attendances for Toronto singers
                new Attendance
                {
                    Status = false, // Michael Johnson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6471234567").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID

                },
                new Attendance
                {
                    Status = true, // Emily Clark
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6479876543").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Nathan Williams
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4165557890").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Sophia Martinez
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4167778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Daniel Brown
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6472223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Ava Taylor
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4169990000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Harris
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6475556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Olivia Moore
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4163334444").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = false, // Ethan White
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6477778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Charlotte Lee
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4161112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Toronto").ID
                },



                   new Attendance
                   {
                       Status = false, // Michael Johnson
                       SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6471234567").ID,
                       SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID

                   },
                new Attendance
                {
                    Status = false, // Emily Clark
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6479876543").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = false, // Nathan Williams
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4165557890").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Sophia Martinez
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4167778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = false, // Daniel Brown
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6472223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Ava Taylor
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4169990000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = false, // Lucas Harris
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6475556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Olivia Moore
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4163334444").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = true, // Ethan White
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6477778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                },
                new Attendance
                {
                    Status = false, // Charlotte Lee
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "4161112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Toronto").ID
                }
                ,

                // Attendances for Saskatoon singers
                new Attendance
                {
                    Status = false, // Benjamin Adams
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3065551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Emma Scott
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3064445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Liam Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3068889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Isabella Fisher
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3062223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Mason Hall
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3067770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Grace Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3069991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3065556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Graham
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3064447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Lily Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3063338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Noah Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3061112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Saskatoon").ID
                },


                new Attendance
                {
                    Status = false, // Benjamin Adams
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3065551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Emma Scott
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3064445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Liam Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3068889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Isabella Fisher
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3062223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Mason Hall
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3067770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Grace Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3069991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3065556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Graham
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3064447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = false, // Lily Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3063338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },
                new Attendance
                {
                    Status = true, // Noah Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "3061112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Saskatoon").ID
                },


                // Attendances for Surrey singers
                new Attendance
                {
                    Status = false, // William Davis
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Chloe Bennett
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6044445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // Henry Wilson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6048889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Ella Robinson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6042223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // Sebastian Walker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Madison King
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // David Evans
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Mila Cruz
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6044447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // Samuel Mitchell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6043338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Zoe Nelson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Surrey").ID
                },





                  new Attendance
                  {
                      Status = false, // William Davis
                      SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045551234").ID,
                      SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                  },
                new Attendance
                {
                    Status = true, // Chloe Bennett
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6044445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // Henry Wilson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6048889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Ella Robinson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6042223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // Sebastian Walker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Madison King
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = false, // David Evans
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Mila Cruz
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6044447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Samuel Mitchell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6043338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },
                new Attendance
                {
                    Status = true, // Zoe Nelson
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Surrey").ID
                },



                // Attendances for Hamilton singers
                new Attendance
                {
                    Status = false, // Mason Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9055551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9054445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9058889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9052223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = false, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9057770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9059991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = false, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9055556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9054447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = false, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9053338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = false, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9051112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Hamilton").ID
                },






                new Attendance
                {
                    Status = true, // Mason Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9055551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9054445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9058889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9052223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9057770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9059991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9055556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9054447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9053338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },
                new Attendance
                {
                    Status = true, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "9051112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Hamilton").ID
                },



                // Attendances for Vancouver
                new Attendance
                {
                    Status = false, // Mason Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041234567").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049876543").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045557890").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6042223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049990000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6043334444").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "Vancouver").ID
                },




                         new Attendance
                         {
                             Status = false, // Mason Carter
                             SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041234567").ID,
                             SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                         },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049876543").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045557890").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6042223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6049990000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6045556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6043334444").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6047778888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },
                new Attendance
                {
                    Status = false, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "6041112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "Vancouver").ID
                },


                // Attendances for St. Catharines singers
                   new Attendance
                   {
                       Status = false, // Mason Carter
                       SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2895551234").ID,
                       SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                   },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2894445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = false, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2898889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2892223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = false, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2897770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2899991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2895556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2894447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2893338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2891112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/14") && s.Location.City == "St. Catharines").ID
                },





                new Attendance
                {
                    Status = false, // Mason Carter
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2895551234").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Emma Parker
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2894445678").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Jacob Cooper
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2898889999").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Aiden Ward
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2892223333").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Lily Foster
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2897770000").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Lucas Reed
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2899991111").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Olivia Gray
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2895556666").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Ethan Bell
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2894447777").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // Zoe Howard
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2893338888").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
                },
                new Attendance
                {
                    Status = true, // William Murphy
                    SingerID = context.Singers.FirstOrDefault(s => s.EmergencyContactNumber == "2891112222").ID,
                    SessionID = context.Sessions.FirstOrDefault(s => s.Date == DateTime.Parse("2025/02/21") && s.Location.City == "St. Catharines").ID
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
                    new Volunteer { FirstName = "John", LastName = "Doe", Phone = "1234567890", Email = "john.doe@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Jane", LastName = "Smith", Phone = "2345678901", Email = "jane.smith@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Alice", LastName = "Johnson", Phone = "3456789012", Email = "alice.johnson@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Bob", LastName = "Brown", Phone = "4567890123", Email = "bob.brown@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Charlie", LastName = "Davis", Phone = "5678901234", Email = "charlie.davis@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "David", LastName = "Miller", Phone = "6789012345", Email = "david.miller@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Eve", LastName = "Wilson", Phone = "7890123456", Email = "eve.wilson@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Frank", LastName = "Moore", Phone = "8901234567", Email = "frank.moore@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Grace", LastName = "Taylor", Phone = "9012345678", Email = "grace.taylor@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Hank", LastName = "Anderson", Phone = "0123456789", Email = "hank.anderson@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Ivy", LastName = "Thomas", Phone = "1234567890", Email = "ivy.thomas@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Jack", LastName = "Jackson", Phone = "2345678901", Email = "jack.jackson@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Karen", LastName = "White", Phone = "3456789012", Email = "karen.white@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Leo", LastName = "Harris", Phone = "4567890123", Email = "leo.harris@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Mia", LastName = "Martin", Phone = "5678901234", Email = "mia.martin@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Nina", LastName = "Thompson", Phone = "6789012345", Email = "nina.thompson@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Oscar", LastName = "Garcia", Phone = "7890123456", Email = "oscar.garcia@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Paul", LastName = "Martinez", Phone = "8901234567", Email = "paul.martinez@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Quinn", LastName = "Robinson", Phone = "9012345678", Email = "quinn.robinson@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Rose", LastName = "Clark", Phone = "0123456789", Email = "rose.clark@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Sam", LastName = "Rodriguez", Phone = "1234567890", Email = "sam.rodriguez@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Tina", LastName = "Lewis", Phone = "2345678901", Email = "tina.lewis@outlook.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto")?.ID ?? 0 },
                    new Volunteer { FirstName = "Uma", LastName = "Lee", Phone = "3456789012", Email = "uma.lee@yahoo.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Vince", LastName = "Walker", Phone = "4567890123", Email = "vince.walker@tv.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 },
                    new Volunteer { FirstName = "Wendy", LastName = "Hall", Phone = "5678901234", Email = "wendy.hall@gmail.com", VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls")?.ID ?? 0 }
                );
                context.SaveChanges();
            }
        }


        private static void SeedEvents(TomorrowsVoicesContext context)
        {
            if (!context.Events.Any())
            {
                context.Events.AddRange(
                    new Event
                    {
                        Name = "Community Cleanup",
                        Description = "A community cleanup event.",
                        Date = DateOnly.Parse("2024-02-20"),
                        StartTime = TimeOnly.ParseExact("09:00 AM", "hh:mm tt", CultureInfo.InvariantCulture),
                        EndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto").ID,
                        Notes = "20 volunteers showed up and the event was successful."
                    },
                    new Event
                    {
                        Name = "Food Drive",
                        Description = "A food drive for the local food bank.",
                        Date = DateOnly.Parse("2024-03-10"),
                        StartTime = TimeOnly.ParseExact("10:00 AM", "hh:mm tt", CultureInfo.InvariantCulture),
                        EndTime = TimeOnly.ParseExact("02:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Toronto").ID,
                        Notes = "15 volunteers participated and collected 200 food items."
                    },
                    new Event
                    {
                        Name = "Blood Donation Camp",
                        Description = "A blood donation camp.",
                        Date = DateOnly.Parse("2024-04-05"),
                        StartTime = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture),
                        EndTime = TimeOnly.ParseExact("06:30 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        VolLocationID = context.VolLocations.FirstOrDefault(v => v.City == "Niagara Falls").ID,
                        Notes = "25 volunteers assisted and 50 units of blood were collected."
                    }
                );
                context.SaveChanges();
            }
        }
        private static void SeedSchedules(TomorrowsVoicesContext context)
        {
            if (!context.Schedules.Any())
            {
                context.Schedules.AddRange(
                    new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Jane").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Food Drive").ID,


                    }, new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("04:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "John").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Community Cleanup").ID,


                    }, new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("05:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("08:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Karen").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID,


                    }, new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Alice").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Food Drive").ID,


                    }, new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("04:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Leo").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Community Cleanup").ID,


                    }, new Schedule
                    {
                        ShiftStart = TimeOnly.ParseExact("05:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        ShiftEnd = TimeOnly.ParseExact("08:00 PM", "hh:mm tt", CultureInfo.InvariantCulture),
                        volunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Mia").ID,
                        eventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID,


                    });
                context.SaveChanges();
            }
        }
        //private static void SeedVolAttendances(TomorrowsVoicesContext context)
        //{
        //    if (!context.VolAttendances.Any())
        //    {
        //        context.VolAttendances.AddRange(
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-02-20", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("09:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("09:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "John" && v.LastName == "Doe").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Community Cleanup").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-02-20", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("09:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("09:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Jane" && v.LastName == "Smith").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Community Cleanup").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-02-20", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("09:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("12:00 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = false, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Alice" && v.LastName == "Johnson").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Community Cleanup").ID },




        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Bob" && v.LastName == "Brown").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("08:00 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("08:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Charlie" && v.LastName == "Davis").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },

        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "David" && v.LastName == "Miller").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Eve" && v.LastName == "Wilson").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("11:30 AM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = false, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Frank" && v.LastName == "Moore").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },

        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("06:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("06:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Grace" && v.LastName == "Taylor").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID },
        //            new VolAttendance { Date = DateOnly.ParseExact("2024-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture), ScheduledStartTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ScheduledEndTime = TimeOnly.ParseExact("06:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualStartTime = TimeOnly.ParseExact("03:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), ActualEndTime = TimeOnly.ParseExact("06:30 PM", "hh:mm tt", CultureInfo.InvariantCulture), Status = true, VolunteerID = context.Volunteers.FirstOrDefault(v => v.FirstName == "Hank" && v.LastName == "Anderson").ID, EventID = context.Events.FirstOrDefault(e => e.Name == "Blood Donation Camp").ID }
        //        );
        //        context.SaveChanges();
        //    }
        //}
    }
}
