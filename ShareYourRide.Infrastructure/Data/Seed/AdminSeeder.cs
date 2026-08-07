using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            var adminSection = configuration.GetSection("AdminSeed");
            var email = adminSection["Email"];
            var password = adminSection["Password"];
            var phoneNumber = adminSection["PhoneNumber"];

            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) return;

            var appUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(appUser, password);
            if (!result.Succeeded) return;

            await userManager.AddToRoleAsync(appUser, nameof(RoleType.Admin));

            var domainUser = new User
            {
                ApplicationUserId = appUser.Id,
                FirstName = "Admin",
                LastName = "Admin",
                FinCode = "0000000",
                BirthDate = new DateTime(2000, 1, 1),
                Status = UserStatus.Approved
            };

            await unitOfWork.Users.AddAsync(domainUser);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
