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
                        City=City.Toronto,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Gregory" && d.LastName == "House").ID

                    },
                    new Location
                    {
                        City = City.Niagara,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Doogie" && d.LastName == "Houser").ID

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
                        FirstName = "Logan",
                        LastName = "Xavier",
                        LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Saskatoon).ID

                    });
                    context.SaveChanges();
                }

              
            }
        }
    }
}
