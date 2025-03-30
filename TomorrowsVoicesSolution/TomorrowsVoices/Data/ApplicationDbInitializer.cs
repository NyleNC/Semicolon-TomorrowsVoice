using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace TomorrowsVoices.Data
{
    public class ApplicationDbInitializer
    {
        public static async void Initialize(IServiceProvider serviceProvider,
        bool UseMigrations = true, bool SeedSampleData = true)
        {
            if (UseMigrations)
            {
                using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    try
                    {
                        // Create the database if it does not exist and apply the Migration  
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }

                // After seeding users  
                using (var context = new TomorrowsVoicesContext(
                    serviceProvider.GetRequiredService<DbContextOptions<TomorrowsVoicesContext>>()))
                {
                    if (SeedSampleData)
                    {
                        //Create Roles  
                        using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
                        {
                            try
                            {
                                string[] roleNames = { "Admin", "Director", "Volunteer" };

                                IdentityResult roleResult;
                                foreach (var roleName in roleNames)
                                {
                                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                                    if (!roleExist)
                                    {
                                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.GetBaseException().Message);
                            }
                        }

                        //Create Users
                        using (var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>())
                        {
                            try
                            {
                                string defaultPassword = "Password.1";

                                if (await userManager.FindByEmailAsync("admin@outlook.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "admin@outlook.com",
                                        Email = "admin@outlook.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Admin");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("director@outlook.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "director@outlook.com",
                                        Email = "director@outlook.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Director");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("averdecchia@tv.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "averdecchia@tv.com",
                                        Email = "averdecchia@tv.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Director");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("volunteer@outlook.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "volunteer@outlook.com",
                                        Email = "volunteer@outlook.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("john.doe@gmail.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "john.doe@gmail.com",
                                        Email = "john.doe@gmail.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("jane.smith@outlook.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "jane.smith@outlook.com",
                                        Email = "jane.smith@outlook.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("alice.johnson@yahoo.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "alice.johnson@yahoo.com",
                                        Email = "alice.johnson@yahoo.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("bbrown@example.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "bbrown@example.com",
                                        Email = "bbrown@example.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                                if (await userManager.FindByEmailAsync("charlie.davis@gmail.com") == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = "charlie.davis@gmail.com",
                                        Email = "charlie.davis@gmail.com",
                                        EmailConfirmed = true
                                    };

                                    IdentityResult result = await userManager.CreateAsync(user, defaultPassword);

                                    if (result.Succeeded)
                                    {
                                        await userManager.AddToRoleAsync(user, "Volunteer");
                                    }
                                }
                            }

                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.GetBaseException().Message);
                            }
                        }

                        //// Enhance volunteer data  
                        //EnhanceVolunteerData(context);
                    }
                }
            }
        }

        //private static void EnhanceVolunteerData(TomorrowsVoicesContext context)
        //{
        //    var volunteers = context.Volunteers.ToList();

        //    if (volunteers.Any())
        //    {
        //        var john = volunteers.FirstOrDefault(v => v.FirstName == "John" && v.LastName == "Doe");
        //        var jane = volunteers.FirstOrDefault(v => v.FirstName == "Jane" && v.LastName == "Smith");
        //        var alice = volunteers.FirstOrDefault(v => v.FirstName == "Alice" && v.LastName == "Johnson");
        //        var bob = volunteers.FirstOrDefault(v => v.FirstName == "Bob" && v.LastName == "Brown");
        //        var charlie = volunteers.FirstOrDefault(v => v.FirstName == "Charlie" && v.LastName == "Davis");

        //        if (john != null)
        //            john.Email = "john.doe@gmail.com";
        //        if (jane != null)
        //            jane.Email = "jsmith@example.com";
        //        if (alice != null)
        //            alice.Email = "ajohnson@example.com";
        //        if (bob != null)
        //            bob.Email = "bbrown@example.com";
        //        if (charlie != null)
        //            charlie.Email = "cdavis@example.com";

        //        context.SaveChanges();

        //        Console.WriteLine("User login information for volunteer portal:");
        //        Console.WriteLine($"John Doe - Username: john.doe@gmail.com - Password: Pa55w@rd");
        //        Console.WriteLine($"Jane Smith - Username: jsmith@example.com - Password: Pa55w@rd");
        //        Console.WriteLine($"Alice Johnson - Username: ajohnson@example.com - Password: Pa55w@rd");
        //        Console.WriteLine($"Bob Brown - Username: bbrown@example.com - Password: Pa55w@rd");
        //        Console.WriteLine($"Charlie Davis - Username: cdavis@example.com - Password: Pa55w@rd");
        //    }
        
    }
}
