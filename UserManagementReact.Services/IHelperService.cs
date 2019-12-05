using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementReact.Services
{
	public interface IHelperService
	{
		Task<PropertyValues> RetrieveEntity(DbUpdateConcurrencyException ex);
	}
}
