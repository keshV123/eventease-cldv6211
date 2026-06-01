/*
 * EventEase - CLOUD Assignment Part 2
 * 
 * This project was developed using ASP.NET Core MVC scaffolding,
 * Microsoft documentation, Azure Blob Storage documentation,
 * and AI-assisted guidance for implementation refinement,
 * validation logic, and UI improvements.
 * 
 * Technologies used:
 * - ASP.NET Core MVC
 * - Entity Framework Core
 * - Azure Blob Storage / Azurite
 * - SQL Server LocalDB
 * 
 * Author: ST10474344
 */
using EventEase.Services;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;

namespace EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<BlobService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
