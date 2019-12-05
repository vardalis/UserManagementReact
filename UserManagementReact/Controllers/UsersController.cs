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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UserManagementReact.Controllers
{
	[Authorize(Policy = "IsAdmin")]
	[ApiController]
	[Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
		private readonly ILogger<UsersController> _logger;
		private readonly IConfiguration _configuration;
		private readonly IUserManagementService _umService;
		private readonly IMapper _mapper;
		private readonly IHelperService _helperService;

		public UsersController(
			IConfiguration configuration,
			ILogger<UsersController> logger,
			IUserManagementService umService,
			IMapper mapper,
			IHelperService helperService)
		{
			_configuration = configuration;
			_logger = logger;
			_umService = umService;
			_mapper = mapper;
			_helperService = helperService;
		}

		// GET: api/users
		// [Authorize(Roles = "admin")]
		[HttpGet]
		public async Task<ActionResult<PageUsersModel>> Get(int? offset, int? limit, string sortOrder, string searchString)
		{
			PageUsersModel pageUsersModel = new PageUsersModel();

			pageUsersModel.TotalUsers = await _umService.GetAllUsersCountAsync(searchString);

			List<ApplicationUser> users = await _umService.GetUsersAsync(offset ?? 0, limit ?? 10, sortOrder, searchString);

			List<UserModel> userModels = new List<UserModel>();
			// Refactor to return role as part of the user (using a Dto) and avoid multiple
			// round-trips to the database
			foreach (ApplicationUser user in users)
			{
				UserModel userModel = _mapper.Map<UserModel>(user);
				string userRole = await _umService.GetUserRoleAsync(user.Id, false);
				userModel.Role = userRole;
				pageUsersModel.Users.Add(userModel);
			}
			return pageUsersModel;
		}

		// GET: api/users/5
		// [HttpGet("{id}"), Name = "Get"]
		[HttpGet("{id}")]
		public async Task<ActionResult<UserModel>> Get(string id)
		{
			var user = await _umService.FindUserAsync(id);

			if (user == null)
				return NotFound();

			UserModel userModel = _mapper.Map<UserModel>(user);
			string userRole = await _umService.GetUserRoleAsync(user.Id, true);
			userModel.Role = userRole;

			return userModel;
		}

		// POST: api/users
		[HttpPost]
		public async Task<ActionResult<UserModel>> Post(UserModel userModel)
		{
			if (!ModelState.IsValid) // Is automatically done by the [ApiController] controller attribute
				return BadRequest(ModelState);

			if (await _umService.IsEmailInUseAsync(userModel.Email))
			{
				ModelState.AddModelError("Email", "Email already in use.");
				return BadRequest(ModelState);
			}

			ApplicationUser user = _mapper.Map<ApplicationUser>(userModel);
			user.EmailConfirmed = true;
			user.UserName = userModel.Email;
			IdentityResult result = await _umService.AddUserAsync(user, userModel.Password, userModel.Role);

			return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
		}

		// PUT: api/users/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, UpdateUserModel userModel)
		{
			if (id != userModel.Id || !ModelState.IsValid)
				return BadRequest();

			ApplicationUser user = await _umService.FindUserAsync(userModel.Id);

			if (user == null)
				return NotFound();

			if (await _umService.IsEmailInUseAsync(userModel.Email, userModel.Id))
			{
				ModelState.AddModelError("Email", "Email already in use");
				return BadRequest(ModelState);
			}

			_mapper.Map(userModel, user);

			try
			{
				await _umService.UpdateUserAsync(user, userModel.Role, userModel.RowVersion);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				var databaseUser = _helperService.RetrieveEntity(ex);
				if (databaseUser == null)
				{
					ModelState.AddModelError("", "User deleted by another user.");
				}
				else
				{
					ModelState.AddModelError("", "User modified by another user.");
				}

				return BadRequest(ModelState);
			}

			return NoContent();
		}

		// DELETE: api/users/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
			var result = await _umService.DeleteUserAsync(id);

			if (result.Succeeded)
				return NoContent();

			return NotFound();
		}

		// GET: api/users/Roles
		[HttpGet("roles")]
		public ActionResult<List<RoleHelpers.RolePair>> GetRoles()
		{
			return RoleHelpers.Roles;
		}
		// POST: api/5/change-password
		[HttpPost("{id}/change-password")]
		public async Task<IActionResult> ChangeUserPassword(string id, PasswordChangeModel model)
		{
			ApplicationUser user = await _umService.FindUserAsync(id);

			if (user == null)
				return NotFound();

			var result = await _umService.ChangePasswordAsync(user, model.Password);

			if (result.Succeeded)
				return Ok();

			return BadRequest();
		}
	}
}
