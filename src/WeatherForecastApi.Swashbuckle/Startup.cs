using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace WeatherForecastApi.Swashbuckle
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.AddSwaggerGen(options =>
            {
                OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
                {
                    var info = new OpenApiInfo
                    {
                        Title = $"Weather Forecast API (v{description.ApiVersion})",
                        Version = description.ApiVersion.ToString(),
                        Description = "The weather forecast API does weather forecast APi stuff."
                    };

                    if (description.IsDeprecated)
                    {
                        info.Title += " (DEPRECATED)";
                    }

                    Debug.WriteLine(JsonConvert.SerializeObject(info));

                    return info;
                }

                // Unlike with NSwag, there doesn't appear to be any real value to registering documents with independent
                // SwaggerDoc calls for each version. Stuff like JSON serialization and security definitions is not defined
                // at the version level.
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    Debug.WriteLine(description.GroupName);
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider versionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }
    }
}