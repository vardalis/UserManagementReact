using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UserManagementReact.Entities
{
	public class Gender : LookupValue
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; }
	}
}
