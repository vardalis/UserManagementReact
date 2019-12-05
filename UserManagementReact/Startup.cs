using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagementReact.Services;
using UserManagementReact.Entities;
using System;
using System.ComponentModel.Design;
using UserManagementReact.Helpers;
using IdentityServer4.Services;
using AutoMapper;
using IdentityModel;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using UserManagementReact.Shared;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;

namespace UserManagementReact
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));

			services.AddDefaultIdentity<ApplicationUser>().AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddIdentityServer()
				.AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
				.AddProfileService<ProfileService>();

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 1;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 10;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = true;
			});

			services.AddAuthentication()
				.AddIdentityServerJwt();

			services.AddAuthorization(options =>
			{
				options.AddPolicy("IsAdmin",
					policy =>
					{
						// Even though we are using JwtClaimTypes in the ProfileService of the IdentityServer
						// the actual user claims are converted to those in ClaimTypes so check for them here
						policy.RequireClaim(ClaimTypes.Role, "administrator");
					});
			});

			services.AddControllersWithViews();
			services.AddRazorPages();

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/build";
			});

			services.AddAutoMapper(typeof(Startup));

			services.AddSingleton<IStringLocalizer>((ctx) =>
			{
				IStringLocalizerFactory factory = ctx.GetService<IStringLocalizerFactory>();
				return factory.Create(typeof(SharedResources));
			});
			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.Configure<RequestLocalizationOptions>(options =>
			{
				// State what the default culture for your application is. This will be used if no specific culture
				// can be determined for a given request.
				options.DefaultRequestCulture = new RequestCulture(culture: Cultures.DefaultCulture, uiCulture: Cultures.DefaultCulture);

				// You must explicitly state which cultures your application supports.
				// These are the cultures the app supports for formatting numbers, dates, etc.
				options.SupportedCultures = Cultures.SupportedCultures;

				// These are the cultures the app supports for UI strings, i.e. we have localized resources for.
				options.SupportedUICultures = Cultures.SupportedCultures;

				// You can change which providers are configured to determine the culture for requests, or even add a custom
				// provider with your own logic. The providers will be asked in order to provide a culture for each request,
				// and the first to provide a non-null result that is in the configured supported cultures list will be used.
				// By default, the following built-in providers are configured:
				// - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
				// - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
				// - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header

				// Remove AcceptLanguageHeaderRequestCultureProvider to eliminate warning in the log files
				options.RequestCultureProviders = new List<IRequestCultureProvider>
				{
					new QueryStringRequestCultureProvider(),
					new CookieRequestCultureProvider()
				};

				//options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
				//{
				//  // My custom request culture logic
				//  return new ProviderCultureResult("en");
				//}));
			});

			services.AddScoped<IUserManagementService, UserManagementService>();
			services.AddScoped<IHelperService, HelperService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseRequestLocalization();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseIdentityServer();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				//endpoints.Map("Identity/Account/Manage/ChangePassword", async context =>
				//{
				//	context.Response.StatusCode = 300;
				//	await context.Response.CompleteAsync();
				//});
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseReactDevelopmentServer(npmScript: "start");
				}
			});
		}
	}
}
