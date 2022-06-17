using System;
using System.Threading;
using System.Threading.Tasks;
using HttpDataServer.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HttpDataServer;

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
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "HttpDataServer", Version = "v1" }); });
        services.AddHostedService<HostedService>();
        services.AddControllers().ConfigureApiBehaviorOptions(opt => { opt.SuppressMapClientErrors = true; });
        services.AddControllers().AddNewtonsoftJson();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        });

        SetInjectionToController(services);
    }

    // Dependency Injection
    private void SetInjectionToController(IServiceCollection services)
    {
        services.AddScoped<AccountRepo>();
        services.AddScoped<DeviceRepo>();
        services.AddScoped<ValidationCodeRepo>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        Console.WriteLine("env.IsDevelopment()", env.IsDevelopment());
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HttpDataServer v1"));
        }

        // app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public class HostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Server.DBMgr.Dispose();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}