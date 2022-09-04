using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.Blazor;
namespace BlazorPoPupDemo_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        [System.Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddAuthentication("Cookies").AddCookie(opt =>
            {
                opt.Cookie.Name = "TryingoutGoogleOAuth";
                opt.LoginPath = "/auth/signin-google";
            }).AddGoogle(opt =>
            {
                opt.ClientId = Configuration["Google:Id"];
                opt.ClientSecret = Configuration["Google:secret"];
                opt.Scope.Add("profile");
            });
            services.AddBlazoredModal();
            services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
