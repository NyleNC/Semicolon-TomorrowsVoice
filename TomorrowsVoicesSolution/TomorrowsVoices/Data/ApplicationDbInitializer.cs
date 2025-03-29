using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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
                        //Create the database if it does not exist and apply the Migration
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }
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
                            string defaultPassword = "Pa55w@rd";

                            if (userManager.FindByEmailAsync("admin@outlook.com").Result == null)
                            {
                                IdentityUser user = new IdentityUser
                                {
                                    UserName = "admin@outlook.com",
                                    Email = "admin@outlook.com",
                                    EmailConfirmed = true
                                };

                                IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                                if (result.Succeeded)
                                {
                                    userManager.AddToRoleAsync(user, "Admin").Wait();
                                }
                            }
                            if (userManager.FindByEmailAsync("director@outlook.com").Result == null)
                            {
                                IdentityUser user = new IdentityUser
                                {
                                    UserName = "director@outlook.com",
                                    Email = "director@outlook.com",
                                    EmailConfirmed = true
                                };

                                IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                                if (result.Succeeded)
                                {
                                    userManager.AddToRoleAsync(user, "Director").Wait();
                                }
                            }
                            if (userManager.FindByEmailAsync("volunteer@outlook.com").Result == null)
                            {
                                IdentityUser user = new IdentityUser
                                {
                                    UserName = "volunteer@outlook.com",
                                    Email = "volunteer@outlook.com",
                                    EmailConfirmed = true
                                };

                                IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                                if (result.Succeeded)
                                {
                                    userManager.AddToRoleAsync(user, "Volunteer").Wait();
                                }
                            }
                            if (userManager.FindByEmailAsync("john.doe@gmail.com").Result == null)
                            {
                                IdentityUser user = new IdentityUser
                                {
                                    UserName = "john.doe@gmail.com",
                                    Email = "john.doe@gmail.com",
                                    EmailConfirmed = true
                                };

                                IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                                if (result.Succeeded)
                                {
                                    userManager.AddToRoleAsync(user, "Volunteer").Wait();
                                }
                            }
                            if (userManager.FindByEmailAsync("averdecchia@tv.com").Result == null)
                            {
                                IdentityUser user = new IdentityUser
                                {
                                    UserName = "averdecchia@tv.com",
                                    Email = "averdecchia@tv.com",
                                    EmailConfirmed = true
                                };

                                IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                                if (result.Succeeded)
                                {
                                    userManager.AddToRoleAsync(user, "Director").Wait();
                                }

                            }
                        }

                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetBaseException().Message);
                        }
                    }
                }
            }
        }
    }
}
