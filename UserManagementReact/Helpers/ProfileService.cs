using UserManagementReact.Services;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UserManagementReact.Helpers
{
	public class ProfileService : IProfileService
    {
        private readonly IUserManagementService _umService;

		public ProfileService(IUserManagementService umService)
		{
			_umService = umService;
		}

		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			if (context == null)
				return;

			var claims = context.Subject.Claims
				.Where(claim => claim.Type == JwtClaimTypes.Email || claim.Type == JwtClaimTypes.Role).ToList();

			//foreach (var claim in claims)
			//{
			//	if (claim.Type == "role")
			//		context.IssuedClaims.Add(new Claim(ClaimTypes.Role, claim.Value));
			//}

			context.IssuedClaims.AddRange(claims);
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			var user = await _umService.GetUserAsync(context.Subject);
			context.IsActive = (user != null);
		}
	}
}
