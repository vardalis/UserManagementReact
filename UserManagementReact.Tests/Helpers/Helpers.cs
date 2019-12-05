using UserManagementReact.Entities;
using UserManagementReact.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagementReact.Tests.Helpers
{
	class TestHelpers
	{
		public static UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
		{
			var userStore = new UserStore<ApplicationUser>(context);
			var passwordHasher = new PasswordHasher<ApplicationUser>();
			var userValidators = new List<UserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
			var passwordValidators = new List<PasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() };
			var userLogger = (new LoggerFactory()).CreateLogger<UserManager<ApplicationUser>>();

			return new UserManager<ApplicationUser>(userStore, null, passwordHasher, userValidators, passwordValidators,
				null, null, null, userLogger);
		}
		public static RoleManager<IdentityRole> CreateRoleManager(ApplicationDbContext context)
		{
			var roleStore = new RoleStore<IdentityRole>(context);
			var roleValidators = new List<RoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };
			var roleLogger = (new LoggerFactory()).CreateLogger<RoleManager<IdentityRole>>();

			return new RoleManager<IdentityRole>(roleStore, roleValidators, null, null, roleLogger);
		}
		public static List<ApplicationUser> GetTestUsers()
		{
			List<ApplicationUser> returnList = new List<ApplicationUser>();

			returnList.Add(new ApplicationUser
			{
				FirstName = "aFirstName",
				LastName = "bLastName",
				Email = "c@b.cEmail",
				UserName = "d@b.cUserName",
				Approved = true
			});

			returnList.Add(new ApplicationUser
			{
				FirstName = "bFirstName",
				LastName = "cLastName",
				Email = "d@b.cEmail",
				UserName = "a@b.cUserName",
				Approved = false
			});

			returnList.Add(new ApplicationUser
			{
				FirstName = "cFirstName",
				LastName = "dLastName",
				Email = "a@b.cEmail",
				UserName = "b@b.cUserName",
				Approved = false
			});

			returnList.Add(new ApplicationUser
			{
				FirstName = "dFirstName",
				LastName = "aLastName",
				Email = "b@b.cEmail",
				UserName = "c@b.cUserName",
				Approved = false
			});

			return returnList;
		}
	}
}
