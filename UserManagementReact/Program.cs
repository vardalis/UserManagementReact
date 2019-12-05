using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagementReact.Services;
using UserManagementReact.Entities;
using Microsoft.AspNetCore.Identity;
using static UserManagementReact.Services.Helpers.RoleHelpers;
using UserManagementReact.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace UserManagementReact
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// CreateWebHostBuilder(args).Build().Run();

			var hostBuilder = CreateWebHostBuilder(args);
			var host = hostBuilder.Build();

			InitializeDatabase(host);

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				//.ConfigureLogging(logging =>
				//{
				//	logging.ClearProviders();
				//	logging.AddConsole(); 
				//})
				.UseStartup<Startup>();

		private static void InitializeDatabase(IWebHost host)
		{
			using (var serviceScope = host.Services.CreateScope())
			{
				var services = serviceScope.ServiceProvider;

				if (!services.GetService<ApplicationDbContext>().AllMigrationsApplied())
				{
					services.GetService<ApplicationDbContext>().Database.Migrate();
				}

				// Seed database
				serviceScope.ServiceProvider.GetService<ApplicationDbContext>().EnsureSeeded();

				IUserManagementService umService = services.GetRequiredService<IUserManagementService>();
				var usersCount = umService.GetAllUsersCountAsync("").Result;
				if (usersCount == 0)
				{
					RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

					foreach (RolePair role in RoleHelpers.Roles)
					{
						if (!roleManager.RoleExistsAsync(role.Name).Result)
						{
							var idRole = new IdentityRole(role.Name);
							roleManager.CreateAsync(idRole).Wait();
						}
					}

					// Create admin user
					ApplicationUser adminUser = new ApplicationUser
					{
						UserName = "admin@domain.com",
						Email = "admin@domain.com",
						FirstName = "AdminFirst",
						LastName = "AdminLast",
						EmailConfirmed = true,
						Approved = true
					};

					umService.AddUserAsync(adminUser, "admin", "administrator").Wait(); // username = "admin", password = "admin"

					UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
					userManager.AddToRoleAsync(adminUser, "user").Wait();

					ApplicationUser supervisorUser = new ApplicationUser
					{
						UserName = "supervisor@domain.com",
						Email = "supervisor@domain.com",
						FirstName = "SupervisorFirst",
						LastName = "SupervisorLast",
						EmailConfirmed = true,
						Approved = true
					};

					umService.AddUserAsync(supervisorUser, "supervisor", "supervisor").Wait();

					ApplicationUser userUser = new ApplicationUser();

					// Create users
					for (int i = 1; i < 125; i++)
					{
						userUser = new ApplicationUser
						{
							UserName = "user" + i + "@domain.com",
							Email = "user" + i + "@domain.com",
							FirstName = "USER" + i + "FIRST",
							LastName = "USER" + i + "LAST",
							EmailConfirmed = true,
							Approved = true
						};
						umService.AddUserAsync(userUser, "user", "user").Wait();
					}
				}
			}
		}
	}
}
