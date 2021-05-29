using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventBLib.DataContext;
using EventB.Services;
using EventBLib.Models;
using EventB.Services.MarketKibnetApiServices;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using EventB.Services.MessageServices;
using EventB.Services.EventServices;
using EventB.Hubs;
using EventB.Services.Logger;
using EventB.Services.FriendService;
using EventB.Services.ImageService;

namespace EventB
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment _hostEnvironment)
        {
            Configuration = configuration;
            hostEnvironment = _hostEnvironment;
        }
        public IWebHostEnvironment hostEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("EB1");
            services.AddDbContext<Context>(options=> {
                options.UseMySql(connection);
            });
            //services.AddScoped<IDataProvider, DbData>();

            services.AddIdentity<User, IdentityRole>(
                opt =>
                {
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireDigit = false;
                    
                }
                ).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;

                services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(7);
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";

                    options.SlidingExpiration = true;
                });
            });
            services.AddTransient<ILogger>(sp => new Logger($"{hostEnvironment.WebRootPath}/{Configuration.GetSection("LogPath").Value}"));
            services.AddTransient<ITegSplitter, TegSplitter>();
            services.AddTransient<IUserFindService, UserFindService>();
            services.AddTransient<IMarketKibnetApiServices, MarketKibnetApiServices>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IFriendService, FriendService>();

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IEventSelectorService, EventSelectorService>();

            services.AddSignalR().AddHubOptions<MessagesHub>(options => {
                options.EnableDetailedErrors = true;
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'self'");
                context.Response.Headers.Add("X-Frame-Options", "Deny");
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Events}/{action=Start}/{id?}");

                endpoints.MapHub<MessagesHub>("/chatroom");
            });

        }
    }
}
