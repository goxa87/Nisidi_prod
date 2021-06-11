using Admin.AdminDbContext;
using Admin.AdminDbContext.Models;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("EB1");
            services.AddDbContext<Context>(options => {
                options.UseMySql(connection);
            });

            string adminConnection = Configuration.GetConnectionString("Admin");
            services.AddDbContext<AdminContext>(options => {
                options.UseMySql(adminConnection);
            });

            services.AddIdentity<AdminUser, IdentityRole>(
               opt =>
               {
                   opt.Password.RequireUppercase = false;
                   opt.Password.RequireLowercase = false;
                   opt.Password.RequireNonAlphanumeric = false;
                   opt.Password.RequireDigit = false;

               }
               ).AddEntityFrameworkStores<AdminContext>().AddDefaultTokenProviders();

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
                    options.Cookie.Expiration = TimeSpan.FromHours(5);
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";

                    options.SlidingExpiration = true;
                });
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
