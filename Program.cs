//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TheBlogProject.Services;

//namespace TheBlogProject
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            //CreateHostBuilder(args).Build().Run();
//            var host = CreateHostBuilder(args).Build();

//            //Pull out my registered DataService
//            var dataService = host.Services.CreateScope().ServiceProvider.GetRequiredService<DataService>();

//            await dataService.ManageDataAsync();

//            host.Run();


//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                });
//    }
//}

using TheBlogProject.Data;
using TheBlogProject.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddDefaultUI()
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

//Register my custom DataService class
builder.Services.AddScoped<DataService>();

builder.Services.AddScoped<BlogSearchService>();

builder.Services.AddScoped<ConnectionService>();

//Register a preconfigured instance of the MailSettings class
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IBlogEmailSender, EmailService>();

//Register Image Service
builder.Services.AddScoped<IImageService, BasicImageService>();

//Register Slug Service
builder.Services.AddScoped<ISlugService, BasicSlugService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
            name: "SlugRoute",
            pattern: "BlogPosts/UrlFriendly/{slug}",
            defaults: new { controller = "Posts", action = "Details" });
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();

