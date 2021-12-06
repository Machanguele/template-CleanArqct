using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Persistence
{
    public class DataContextSeed
    {
        public DataContextSeed()
        {
        }
       
        public static async Task SeedAsync(DataContext context, ILoggerFactory loggerFactory, 
            UserManager<AppUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    var rolesData = File.ReadAllText("../Persistence/SeedData/rolesSeed.json");
                    var roles = JsonSerializer.Deserialize<List<Role>>(rolesData);
                    
                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role.Name));
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<DataContext>();
                logger.LogError(ex, ex.Message);
            }
        }
    }
}