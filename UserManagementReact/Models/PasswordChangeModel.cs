using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementReact.Models
{
	public class PasswordChangeModel
	{
		[StringLength(100, MinimumLength = 1)]
		public string Password { get; set; }

		[Compare("Password")]
		public string ConfirmPassword { get; set; }
	}
}
