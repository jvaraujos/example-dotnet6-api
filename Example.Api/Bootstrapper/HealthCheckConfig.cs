using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Example.Api.Bootstrapper
{
    public static class HelthCheckConfig
    {
        public static void ConfigHelthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PomarContext");
            services.AddHealthChecks()
                .AddSqlServer(connectionString, name: "DataBaseSQL");
        }

        public static void StartHelthCheck(this IApplicationBuilder app, string environmentName)
        {
            app.UseHealthChecks("/admin/healthcheck",
               new HealthCheckOptions()
               {
                   ResponseWriter = async (context, report) =>
                   {
                       var result = JsonConvert.SerializeObject(
                           new
                           {
                               statusApplication = report.Status.ToString(),
                               EnvironmentName = environmentName,
                               healthChecks = report.Entries.Select(e => new
                               {
                                   check = e.Key,
                                   ErrorMessage = e.Value.Exception?.Message,
                                   status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                               })
                           });
                       context.Response.ContentType = MediaTypeNames.Application.Json;
                       await context.Response.WriteAsync(result);
                   }
               });
        }
    }
}
