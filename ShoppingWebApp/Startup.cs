using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoppingWebApp.Infrastructure;

namespace ShoppingWebApp
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
            //enable sessions
            services.AddMemoryCache();
            services.AddSession(options=> 
            {
                ////clear the session in specified time
                //options.IdleTimeout = TimeSpan.FromSeconds(2);
            });

            services.AddControllersWithViews();

            //register db context
            services.AddDbContext<ShoppingWebAppContext>(options=> options.UseSqlServer
                (Configuration.GetConnectionString("ShoppingWebAppContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            //use session
            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // define routing from moast specific to least specific
                // map the new route to Pages controller and Page action
                // will work without /pages/action
                endpoints.MapControllerRoute(
                    name:"pages",
                    pattern:"{slug?}",
                    defaults: new { controller="Pages", action="Page"}
                    );

                endpoints.MapControllerRoute(
                    name: "products",
                    pattern: "products/{categorySlug}",
                    defaults: new { controller = "Products", action = "ProductsByCategory" }
                    );

                //map the areas folder
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
