using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.RegisterMiddleware = true;
            }).AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
            });
            
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1";
                document.ApiGroupNames = new[] {"1"};
                document.Title = "Weather Forecast API";
                document.Description = "This API returns weather forecast information";
                document.Version = "v1";
                document.IgnoreObsoleteProperties = true;
                document.SerializerSettings = new JsonSerializerSettings
                    {ContractResolver = new DefaultContractResolver()};
            }).AddOpenApiDocument(document =>
            {
                document.DocumentName = "v2";
                document.ApiGroupNames = new[] {"2"};
                document.Title = "Weather Forecast API";
                document.Description = "This API returns weather forecast information";
                document.Version = "v1";
                document.IgnoreObsoleteProperties = true;
                document.SerializerSettings = new JsonSerializerSettings
                    {ContractResolver = new DefaultContractResolver()};
                
                document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new SwaggerSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = nameof(Authorization),
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Field should be in this format: \nBearer {my token}"
                    }
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
        }
    }
}