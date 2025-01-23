using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Data
{
    public static class TomorrowsVoicesInitializer
    {
        public static async void Initialize(IServiceProvider serviceProvider,
            bool UseMigrations = true, bool SeedSampleData = true)
        {
            #region Prepare the Database
            if (UseMigrations)
            {
                using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    try
                    {
                        //Create the database if it does not exist and apply the Migration
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }
            }
            #endregion


            //#region Seed Sample Data
            










            using (var context = new TomorrowsVoicesContext(
                serviceProvider.GetRequiredService<DbContextOptions<TomorrowsVoicesContext>>()))
            {
                //Seed Data
                // Look for any Doctors.  Since we can't have patients without Doctors.
                if (!context.Directors.Any())
                {
                    context.Directors.AddRange(
                    new Director
                    {
                        FirstName = "Gregory",
                        LastName = "House",
                        Email = "sX@xvarier.com"
                    },
                       new Director
                       {
                           FirstName = "Dave",
                           LastName = "Kendell",
                           Email = "dkendell@tv.com"
                       },
                    new Director
                    {
                        FirstName = "Doogie",
                        LastName = "Houser",
                        Email = "esX@xvarier.com"
                    },
                    new Director
                    {
                        FirstName = "Charles",
                        LastName = "Xavier",
                        Email="charlesX@xvarier.com"

                    });
                    context.SaveChanges();
                }

                if (!context.Locations.Any())
                {
                    context.Locations.AddRange(
                    new Location
                    {
                        City=City.Toronto,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Gregory" && d.LastName == "House").ID

                    },
                    new Location
                    {
                        City = City.Niagara,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Dave" && d.LastName == "Kendell").ID

                    },
                    new Location
                    {
                        City = City.Saskatoon,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Charles" && d.LastName == "Xavier").ID
                    }
                    );
                    context.SaveChanges();
                }
                if (!context.Singers.Any())
                {
                    context.Singers.AddRange(
                    new Singer
                    {
                        FirstName = "Bruce",

                        LastName = "House",
                        LocationID = context.Locations.FirstOrDefault(static d => d.City==City.Toronto).ID
                    },
                    new Singer
                    {
                        FirstName = "Hall",

                        LastName = "Houser",
                        LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Niagara).ID
                    },
                    new Singer
                    {
                        FirstName = "Radin",

                        LastName = "Shahravan",
                        LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Niagara).ID
                    },
                    new Singer
                    {
                        FirstName = "Logan",
                        LastName = "Xavier",
                        LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Saskatoon).ID

                    });
                    context.SaveChanges();
                }

                if (!context.Sessions.Any())
                {
                    context.Sessions.AddRange(
                    new Session
                    {
                        Date = DateTime.Parse("2024/12/29"),
                        Status = true,
                        LocationID = context.Locations.FirstOrDefault(static l => l.City == City.Niagara).ID
                    });
                    context.SaveChanges();
                }


                //if (!context.Attendances.Any())
                //{
                //    context.Attendances.AddRange(
                //    new Attendance
                //    {
                //        Status = true,
                //        //SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "Radin").ID,
                //        SessionID = context.Sessions.FirstOrDefault(static s => s.Date == DateTime.Parse("2024/12/29")).ID
                //    });
                //    context.SaveChanges();
                //}



            }
        }
    }
}
