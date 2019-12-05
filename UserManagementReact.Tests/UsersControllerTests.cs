using System;
using Xunit;
using Moq;
using UserManagementReact.Services;
using Microsoft.Extensions.Logging;
using UserManagementReact.Controllers;
using AutoMapper;
using UserManagementReact.Helpers;
using System.Threading.Tasks;
using UserManagementReact.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using UserManagementReact.Tests.Helpers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UserManagementReact.Models;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace UserManagementReact.Tests
{
	public class UsersControllerTests
		: IClassFixture<CustomWebApplicationFactory<Startup>>
	{
		private readonly CustomWebApplicationFactory<Startup> _factory;
		private const string ACCEPTABLE_PASSWORD = "accEPTable123!@#Pass";
		public UsersControllerTests(CustomWebApplicationFactory<Startup> factory)
		{
			_factory = factory;
		}
		[Theory]
		[InlineData("/api/users")]
		public async Task Get_AuthorizedRequest_ReturnsSuccess(string url)
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();

			// Act
			var response = await client.GetAsync(url);

			// Assert
			response.EnsureSuccessStatusCode(); // Status Code 200-299
			Assert.Equal("application/json; charset=utf-8",
				response.Content.Headers.ContentType.ToString());
		}
		[Theory]
		[InlineData("/api/users")]
		[InlineData("/api/users/1")]
		[InlineData("/api/users/roles")]
		public async Task Get_UnauthorizedRequest_ReturnsUnauthorized(string url)
		{
			// Arrange
			var client = _factory.CreateClient();

			// Act
			var response = await client.GetAsync(url);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}
		[Fact]
		public async Task Get_UserExists_ReturnsUser()
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();

			var testUsers = TestHelpers.GetTestUsers();

			string id;
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				id = (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == testUsers[0].Email))?.Id;
			}

			// Act
			var response = await client.GetAsync("/api/users/" + id);
			string contentString = await response.Content.ReadAsStringAsync();
			UserModel actualUser = (UserModel) JsonConvert.DeserializeObject(contentString, typeof(UserModel));

			// Assert
			Assert.Equal(testUsers[0].Email, actualUser.Email);
		}
		[Fact]
		public async Task Post_ModelMissingInfo_ReturnsBadRequest()
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();

			var user = new UserModel
			{
				FirstName = "aFirstName",
			};

			string jsonString = JsonConvert.SerializeObject(user);

			// Act
			var response = await client.PostAsync("/api/users", new StringContent(jsonString, Encoding.UTF8, "application/json"));

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact]
		public async Task Post_ModelOK_AddsUser()
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();

			const string EMAIL = "different@email.cEmail";

			var user = new UserModel
			{
				FirstName = "DifferentFirstName",
				LastName = "DifferentLastName",
				Email = EMAIL,
				Password = ACCEPTABLE_PASSWORD,
				ConfirmPassword = ACCEPTABLE_PASSWORD,
				Approved = true,
				Role = "supervisor"
			};

			string jsonString = JsonConvert.SerializeObject(user);

			// Act
			var response = await client.PostAsync("/api/users", new StringContent(jsonString, Encoding.UTF8, "application/json"));

			// Assert
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				var actualUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == EMAIL);
				Assert.Equal(EMAIL, actualUser.Email);
			}
		}
		[Fact]
		public async Task Put_ModelOK_UpdatesUser()
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();
			const int userIndexToUse = 1;
			var testUsers = TestHelpers.GetTestUsers();

			string id;
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				id = (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == testUsers[userIndexToUse].Email))?.Id;
			}

			// Act
			var getResponse = await client.GetAsync("/api/users/" + id);
			string contentString = await getResponse.Content.ReadAsStringAsync();
			UserModel retrievedUser = (UserModel)JsonConvert.DeserializeObject(contentString, typeof(UserModel));

			const string EMAIL = "different@email.cEmail";
			var user = new UpdateUserModel
			{
				Id = retrievedUser.Id,
				// RowVersion =  retrievedUser.RowVersion, // Does not work for in memory db
				RowVersion = null,
				FirstName = "DifferentFirstName",
				LastName = "DifferentLastName",
				Email = EMAIL,
				Approved = true,
				Role = "supervisor"
			};

			string jsonString = JsonConvert.SerializeObject(user);

			// Act
			var response = await client.PutAsync("/api/users/" + retrievedUser.Id, new StringContent(jsonString, Encoding.UTF8, "application/json"));

			// Assert
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				var actualUser = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == EMAIL);
				Assert.Equal(EMAIL, actualUser.Email);
			}
		}
		[Fact]
		public async Task Delete_UserExists_DeletesUser()
		{
			// Arrange
			var client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.AddAuthentication("Test")
						.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
							"Test", options => { });
				});
			})
			.CreateClient();

			const int userIndexToUse = 2;
			var testUsers = TestHelpers.GetTestUsers();

			string id;
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				id = (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == testUsers[userIndexToUse].Email))?.Id;
			}

			// Act
			var response = await client.DeleteAsync("/api/users/" + id);

			// Assert
			using (var scope = _factory.Server.Host.Services.CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				var actualUser = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
				Assert.Null(actualUser);
			}
		}
	}
}
