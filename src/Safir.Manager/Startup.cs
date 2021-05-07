using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Safir.Messaging.DependencyInjection;
using Serilog;

namespace Safir.Manager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();
            
            services.AddSafirMessaging(options => {
                options.ConnectionString = Configuration["Redis"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
                
                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync(
                        "Communication with gRPC endpoints must be made through a gRPC client");
                });
            });
        }
    }
}
