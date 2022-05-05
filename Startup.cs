using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Triton.Operations
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
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddHttpClient();

            //services.AddAuthentication(IISDefaults.AuthenticationScheme);
            //services.AddAuthentication(IISServerDefaults.AuthenticationScheme);

            // services.AddTransient<IClaimsTransformation, Utils.ClaimsTransformer>();
            // services.AddAuthentication(HttpSysDefaults.AuthenticationScheme);
           
            services.AddScoped<IClaimsTransformation, Utils.ClaimsTransformer>();
           
            // Load up customer services
            //StartupService.AddService(services);

            //Local service
            services.AddScoped<Service.Utils.ILocalisationService, Service.Utils.LocalisationService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            //app.UseHttpsRedirection();
            app.UseStaticFiles();
                       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
