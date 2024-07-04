
using Microsoft.AspNetCore.Identity;
using TicketEase.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace TicketEase.Common.Utilities
{
	public class Seeder
	{
		public static void SeedRolesAndSuperAdmin(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var managerManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

			// Seed roles
			if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
			{
				var role = new IdentityRole("SuperAdmin");
				roleManager.CreateAsync(role).Wait();
			}

			if (!roleManager.RoleExistsAsync("Manager").Result)
			{
				var role = new IdentityRole("Manager");
				roleManager.CreateAsync(role).Wait();

			}
			if (!roleManager.RoleExistsAsync("User").Result)
			{
				var role = new IdentityRole("User");
				roleManager.CreateAsync(role).Wait();
			}

			// Seed users with roles
			
			//if (managerManager.FindByNameAsync("Manager").Result == null)
			//{
			//	var man = new Manager
			//	{
			//		CompanyName = "Decagon Institute",
			//		BusinessEmail = "manager@ticketease.com",
			//		//Email = "manager@ticketease.com",
			//		//EmailConfirmed = true,
			//		//LockoutEnabled = false,
			//	};
			//	var result = managerManager.CreateAsync(man, "Password@123").Result;
			//	if (result.Succeeded)
			//	{
			//		managerManager.AddToRoleAsync(man, "Manager").Wait();
			//	}
			//}


		}
	}
}
