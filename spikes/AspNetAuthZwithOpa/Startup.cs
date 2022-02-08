using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetAuthZwithOpa.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetAuthZwithOpa
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
            services.AddRazorPages();

            services.AddMemoryCache();
            services.AddSingleton<IPoliciesStore, ImmutablePoliciesFileStore>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GuardedByOpa", policy =>
                    policy.Requirements.Add(new OpaPolicyRequirement("example")));
            });

            services.AddSingleton<IAuthorizationHandler, OpaPolicyHandler>();

            services.AddSingleton<SampleFactory, SampleFactory>(); // AddSingleton useful when it does cache stuff
            services.AddTransient<SamplePolicy, SamplePolicy>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
