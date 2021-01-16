using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplication1
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
            services.AddSingleton<IDbService, MockDbService>();
            services.AddTransient<newIDbService, SqlServerDbService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LoggingMiddleware>();

            app.Use(async(context, next))=>{
                if (!context.Response.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized();
                    await context.Response.WriteAsync("Nie podales loginu i hasla");
                }
                else
                {
                    //Sprawdzamy czy w bazie danych istnieje SqlConnection
                    string sindex = context.Request.Headers["Index"].ToString();
                }
                await next();
                    });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
