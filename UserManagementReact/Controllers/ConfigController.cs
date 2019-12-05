using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserManagementReact.Entities;
using UserManagementReact.Models;
using UserManagementReact.Services;
using UserManagementReact.Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UserManagementReact.Controllers
{
	[Route("Config")]
    public class ConfigController : ControllerBase
    {
		private readonly ILogger<UsersController> _logger;
		private readonly IConfiguration _configuration;

		public ConfigController(
			ILogger<UsersController> logger,
			IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}
	}
}
