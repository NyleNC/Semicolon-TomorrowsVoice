using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Data
{
    public static class TomorrowsVoicesInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
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
                        City = "St Catharines",
                        Director = context.Directors.FirstOrDefault(l => l.ID == 0)

                    },
                    new Location
                    {
                        City = "Toronto",
                        Director = context.Directors.FirstOrDefault(l => l.ID == 1)



                    },
                    new Location
                    {
                        City = "Saskatoon",

                        Director = context.Directors.FirstOrDefault(l => l.ID == 2)

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
                        Location = context.Locations.FirstOrDefault(l => l.ID == 0)
                    },
                    new Singer
                    {
                        FirstName = "Hall",

                        LastName = "Houser",
                        Location = context.Locations.FirstOrDefault(l => l.ID == 1)
                    },
                    new Singer
                    {
                        FirstName = "Logan",
                        LastName = "Xavier",
                        Location = context.Locations.FirstOrDefault(l => l.ID == 2)

                    });
                    context.SaveChanges();
                }

              
            }
        }
    }
}
