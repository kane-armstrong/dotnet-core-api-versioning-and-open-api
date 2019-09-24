using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NSwag
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
            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.RegisterMiddleware = true;
            }).AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
            }).AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1";
                document.ApiGroupNames = new[] {"1"};
                document.IgnoreObsoleteProperties = true;
                document.SerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            }).AddOpenApiDocument(document =>
            {
                document.DocumentName = "v2";
                document.ApiGroupNames = new[] {"2"};
                document.IgnoreObsoleteProperties = true;
                document.SerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}