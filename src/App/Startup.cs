using App.Data;
using App.Data.DataRepository;
using App.Data.IDataRepository;
using App.Services;
using App.Services.IServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IBranchRepo, BranchRepo>();
            services.AddScoped<ICommitRepo, CommitRepo>();
            services.AddScoped<IDeveloperRepo, DeveloperRepo>();
            services.AddScoped<IFileRepo, FileRepo>();
            services.AddScoped<IFileDetailRepo, FileDetailRepo>();
            services.AddScoped<ILanguageRepo, LanguageRepo>();
            services.AddScoped<IMetricRepo, MetricsRepo>();
            services.AddScoped<IRepositoryRepo, RepositoryRepo>();

            services.AddScoped<IBranchService, BranchService>();

            services.AddControllersWithViews();
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

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
