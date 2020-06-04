using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using TVShowCast.Infrastructure.Repository;
using TVShowCast.Infrastructure.Scheduler;

namespace TVShowCast.Presentation
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
                       

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddTransient<ShowsService>();
            services.AddTransient<TVMazeScapper>();
            services.AddTransient<TVMazeRepository>();
            services.AddHttpClient<TVMazeScapper>();
            services.AddSingleton<TvMazeScrapperJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(TvMazeScrapperJob),
                cronExpression: "0 22 * * * ?")); 
            
            services.AddHostedService<QuartzHostedService>();

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
