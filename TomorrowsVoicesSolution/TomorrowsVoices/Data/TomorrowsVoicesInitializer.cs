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
                // 
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

                if (!context.Locations.Any())
                {
                    context.Locations.AddRange(
                    new Location
                    {
                        City = City.Toronto,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Tom" && d.LastName == "Ronton").ID

                    },
                    new Location
                    {
                        City = City.Saskatoon,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Sasha" && d.LastName == "Katherine").ID
                    },
                    new Location
                    {
                        City = City.Niagara,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Niam" && d.LastName == "Garrison").ID

                    },
                    new Location
                    {
                        City = City.Vancouver,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Vanda" && d.LastName == "Cooper").ID
                    },
                    new Location
                    {
                        City = City.Hamilton,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Ham" && d.LastName == "Elton").ID
                    },
                    new Location
                    {
                        City = City.Surrey,
                        DirectorID = context.Directors.FirstOrDefault(static d => d.FirstName == "Serg" && d.LastName == "Urls").ID
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
                        LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Toronto).ID
                    },
                      new Singer
                      {
                          FirstName = "Tyler",

                          LastName = "Klassen",
                          LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Toronto).ID
                      },
                        new Singer
                        {
                            FirstName = "James",

                            LastName = "Rodrigo",
                            LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Toronto).ID
                        },
                          new Singer
                          {
                              FirstName = "Theo",

                              LastName = "Baker",
                              LocationID = context.Locations.FirstOrDefault(static d => d.City == City.Toronto).ID
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
                        LocationID = context.Locations.FirstOrDefault(static l => l.City == City.Niagara).ID
                    });
                    context.SaveChanges();
                }

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
                                SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "Hall").ID,
                                SessionID = session.ID
                            },
                            new Attendance
                            {
                                Status = false,
                                SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "Logan").ID,
                                SessionID = session.ID
                            },
                            new Attendance
                            {
                                Status = true,
                                SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "Tyler").ID,
                                SessionID = session.ID
                            },
                            new Attendance
                            {
                                Status = false,
                                SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "Theo").ID,
                                SessionID = session.ID
                            },
                            new Attendance
                            {
                                Status = true,
                                SingerID = context.Singers.FirstOrDefault(static l => l.FirstName == "James").ID,
                                SessionID = session.ID
                            }
                        );
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}


