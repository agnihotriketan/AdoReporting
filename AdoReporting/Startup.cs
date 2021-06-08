using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AdoReporting.Core.Contracts;
using AdoReporting.Core.Services;

namespace AdoReporting
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
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            }).AddJsonOptions(jsonOption =>
            {
                jsonOption.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddSingleton<IQueryExecutor, QueryExecutor>(serviceProvider =>
            {
                string adoUrl = Configuration.GetValue<string>("AdoSettings:AdoUrl");
                string orgName = Configuration.GetValue<string>("AdoSettings:OrgName");
                string personalAccessToken = Configuration.GetValue<string>("AdoSettings:PersonalAccessToken");
                return new QueryExecutor(adoUrl, orgName, personalAccessToken);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithMethods("GET", "POST");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "jwt_auth v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
