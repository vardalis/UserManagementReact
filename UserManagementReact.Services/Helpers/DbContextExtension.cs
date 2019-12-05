using UserManagementReact.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UserManagementReact.Services.Helpers
{
	public static class DbContextExtension
	{
		public static bool AllMigrationsApplied(this DbContext context)
		{
			var applied = context.GetService<IHistoryRepository>()
				.GetAppliedMigrations()
				.Select(m => m.MigrationId);

			var total = context.GetService<IMigrationsAssembly>()
				.Migrations
				.Select(m => m.Key);

			return !total.Except(applied).Any();
		}

		public static void EnsureSeeded(this ApplicationDbContext context)
		{
			var genders = JsonConvert.DeserializeObject<List<Gender>>(File.ReadAllText(".." + Path.DirectorySeparatorChar + "UserManagementReact.Services" + Path.DirectorySeparatorChar + "Seeding" + Path.DirectorySeparatorChar + "Genders.json"));
			genders.ForEach(gender => context.Entry(gender).State = context.Set<Gender>().Any(e => e.Id == gender.Id) ? EntityState.Modified : EntityState.Added);

			context.SaveChangesAsync().Wait();
		}
	}
}
