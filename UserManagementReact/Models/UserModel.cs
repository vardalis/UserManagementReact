using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementReact.Models
{
	public class UserModel
	{
		public byte[] RowVersion { get; set; }

		public string Id { get; set; }

		[StringLength(50)]
		public string FirstName { get; set; }

		[StringLength(50)]
		public string LastName { get; set; }

		[Required()]
		[EmailAddress()]
		public string Email { get; set; }

		[Required()]
		[StringLength(100, MinimumLength = 1)]
		public string Password { get; set; }

		[Compare("Password")]
		public string ConfirmPassword { get; set; }

		[Required]
		public string Role { get; set; }

		[Required]
		public bool Approved { get; set; }
	}

	public class UpdateUserModel
	{
		public byte[] RowVersion { get; set; }

		public string Id { get; set; }

		[StringLength(50)]
		public string FirstName { get; set; }

		[StringLength(50)]
		public string LastName { get; set; }

		[Required()]
		[EmailAddress()]
		public string Email { get; set; }

		[Required]
		public string Role { get; set; }

		[Required]
		public bool Approved { get; set; }
	}
}
