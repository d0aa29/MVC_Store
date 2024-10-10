using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repository;
using Store.DataAccess.DbIntializer;
using Store.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Store.Utility;
using Stripe;
using Microsoft.CodeAnalysis.Options;

namespace MvcStore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Configure the DbContext with SQL Server
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// Configure Stripe settings
			builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

			// Configure Identity services
			builder.Services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			builder.Services.AddAuthentication().AddFacebook(option =>
			{
				option.AppId = "490877137028185";
				option.AppSecret = "35037d583154dd92f411fa7c1c248e3f";

			});
			// Configure application cookies for login/logout
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/Login";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});

			// Configure session services
			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(100);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			builder.Services.AddScoped<IDbIntializer, DbIntializer>();
			// Add Razor Pages services
			builder.Services.AddRazorPages();

			// Register application services

			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IEmailSender, EmailSender>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			// Configure Stripe API key
			StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

			app.UseRouting();

			//Use session middleware before authentication

			app.UseSession();

			app.UseAuthentication();
			app.UseAuthorization();
            SeedDatabase();
            app.MapRazorPages();
			app.MapControllerRoute(
				name: "default",
				pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

			app.Run();

			void SeedDatabase()
			{
				using (var scope = app.Services.CreateScope())
				{
					var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
					dbInitializer.Intialize();
				}
			}
		}
	}
}
