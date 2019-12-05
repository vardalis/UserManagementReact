using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UserManagementReact.Tests.Helpers
{
	class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}
		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var claims = new[] 
			{ 
				new Claim(ClaimTypes.Name, "Test user"),
				new Claim(ClaimTypes.Role, "administrator")
			};

			var identity = new ClaimsIdentity(claims, "Test");
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, "Test");

			var result = AuthenticateResult.Success(ticket);

			return Task.FromResult(result);
		}
	}
}
