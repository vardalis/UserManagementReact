using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UserManagementReact.Entities
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		[MaxLength(100)]
		public string FirstName { get; set; }
		[MaxLength(100)]
		public string LastName { get; set; }
		public bool ApplicationEditingAllowed { get; set; } = true;
		public bool Approved { get; set; } = false;

		[Timestamp]
		public byte[] RowVersion { get; set; }

		[NotMapped]
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
	}
}
