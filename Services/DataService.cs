using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, 
                           RoleManager<IdentityRole> roleManager, 
                           UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //Task: Create the DB from the Migrations
            await _dbContext.Database.MigrateAsync();
            //Task: Seed a few Roles into the system
            await SeedRolesAsync();
            //Task: Seed a few Users into the system
            await SeedUserAsync();
        }


        private async Task SeedRolesAsync()
        {
            //If there are already Roles in the system, do nothing
            if (_dbContext.Roles.Any()) return;

            //Otherwise we want to create a few Roles
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                // I need to use the Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUserAsync()
        {
            //If there are already Users in the system, do nothing.
            if (_dbContext.Users.Any()) return;


            //Step 1: Create a new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "ebrucey@outlook.com",
                UserName = "ebrucey@outlook.com",
                FirstName = "Eric",
                LastName = "Bruce",
                DisplayName = "ebruce",
                PhoneNumber = "(555) 555-1212",
                EmailConfirmed = true
            };

            //Step 2: Use the UserManager to create a new user that is defined by the adminUser variable
            await _userManager.CreateAsync(adminUser, "Abc123!");

            //Step 3: Add this new user to the Administrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            //Step 1 repeat: Create the Moderator user
            var modUser = new BlogUser()
            {
                Email = "Jeremy.Austin@email.com",
                UserName = "Jeremy.Austin@email.com",
                FirstName = "Jeremy",
                LastName = "Austin",
                DisplayName = "jaustin",
                PhoneNumber = "(555) 555-1212",
                EmailConfirmed = true
            };



            //Step 2 repeat: Use the UserManager to create a new user that is defined by the adminUser variable
            await _userManager.CreateAsync(modUser, "Aa;sldfjkbc123!");

            //Step 3 repeat: Add this new user to the Moderator role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }

    }
}
