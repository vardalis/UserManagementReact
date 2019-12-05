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
using UserManagementReact.Tests.Helpers;

namespace UserManagementReact.Tests
{
	public class OldUsersControllerTests
	{
		// [Fact]
		public async Task Get_ManyUsers_ReturnsUsers()
		{
			// Arrange
			var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
			var mockLogger = new Mock<ILogger<UsersController>>();
			var mockUmService = new Mock<IUserManagementService>();
			var mockHelperService = new Mock<IHelperService>();
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new AutomapperProfiles.UserProfile());
			});
			var mapper = mapperConfig.CreateMapper();

			var testUsers = TestHelpers.GetTestUsers();

			const string SORT_ORDER = "FName";
			const string SEARCH_STRING = "a";

			mockUmService.Setup(umService => umService.GetAllUsersCountAsync(SEARCH_STRING))
				.ReturnsAsync(testUsers.Count)
				.Verifiable();

			mockUmService.Setup(umService => umService.GetUsersAsync(0, testUsers.Count, SORT_ORDER, SEARCH_STRING))
				.ReturnsAsync(testUsers)
				.Verifiable();

			mockUmService.Setup(umService => umService.GetUserRoleAsync(It.IsAny<string>(), It.IsAny<bool>()))
				.ReturnsAsync("employee")
				.Verifiable();

			var controller = new UsersController(mockConfiguration.Object, mockLogger.Object,
				mockUmService.Object, mapper, mockHelperService.Object);

			// Act
			var result = await controller.Get(0, 10, SORT_ORDER, SEARCH_STRING);

			// Assert

		}
	}
}
