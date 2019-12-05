using UserManagementReact.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagementReact.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace UserManagementReact.Tests.Helpers
{
	public class CustomWebApplicationFactory<TStartup>
		: WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Remove the app's ApplicationDbContext registration.
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType ==
						typeof(DbContextOptions<ApplicationDbContext>));

				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

				// Add ApplicationDbContext using an in-memory database for testing.
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseInMemoryDatabase("InMemoryDbForTesting");
				});

				// Build the service provider.
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database
				// context (ApplicationDbContext).
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
					var logger = scopedServices
						.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

					// Ensure the database is created.
					dbContext.Database.EnsureCreated();

					try
					{
						// Seed the database with test data.
						var testUsers = TestHelpers.GetTestUsers();
						const string ACCEPTABLE_PASSWORD = "accEPTable123!@#Pass";
						const string ROLE_NAME = "administrator";
						using (var userManager = TestHelpers.CreateUserManager(dbContext))
						using (var roleManager = TestHelpers.CreateRoleManager(dbContext))
						{
							var role = new IdentityRole(ROLE_NAME);
							roleManager.CreateAsync(role);
							foreach (ApplicationUser user in testUsers)
							{
								userManager.CreateAsync(user, ACCEPTABLE_PASSWORD).Wait();
								userManager.AddToRoleAsync(user, ROLE_NAME).Wait();
							}
						}
						// Utilities.InitializeDbForTests(db);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "An error occurred seeding the " +
							"database with test messages. Error: {Message}", ex.Message);
					}
				}
			});
		}
	}
}
