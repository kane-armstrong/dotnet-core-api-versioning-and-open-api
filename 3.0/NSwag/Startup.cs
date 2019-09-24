using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.Generation.Processors.Security;

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
            });

            /*
                This approach looks nice, but explicitly calling AddOpenApiDocument for each version may be preferable 
                in that it makes it easier to maintain version-specific document configuration. Perhaps v1 is an 
                unauthenticated API, v2 introduces basic auth, and v3 uses a JWT issued by an OAuth2.0 or OIDC identity 
                provider. In this scenario, the version description enumeration approach is counterproductive.
             
                var versionDescriptionProvider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var versionDescription in versionDescriptionProvider.ApiVersionDescriptions)
                {
                    services.AddOpenApiDocument(document =>
                    {
                        document.DocumentName = versionDescription.GroupName;
                        document.ApiGroupNames = new[] {versionDescription.ApiVersion.ToString()};
                        document.Title = "Weather Forecast API";
                        document.Description = "This API returns weather forecast information";
                        document.IgnoreObsoleteProperties = true;
                        document.SerializerSettings = new JsonSerializerSettings
                            {ContractResolver = new DefaultContractResolver()};
                    });
                }
             */
            
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
                    new OpenApiSecurityScheme()
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