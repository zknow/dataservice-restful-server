using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DataServer.Database;
using DataServer.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Tools;

namespace DataServer;

public class Startup
{
    public static Logrotator logrotator;
    public static DBManager dbManager;

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
        Configuration = builder.Build();

        InitLogger();

        Configuration.GetSection("Database").Bind(DBManager.DatabaseConfig);
        DBManager.Instance = new DBManager();
    }

    private void InitLogger()
    {
        Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();

        logrotator = new Logrotator();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddControllers(option => option.Filters.Add<ExceptionFilter>());
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataServer", Version = "v1" }); });
        services.AddControllers().ConfigureApiBehaviorOptions(opt => { opt.SuppressMapClientErrors = true; });
        services.AddControllers().AddNewtonsoftJson();

        services.AddApiVersioning(options =>
           {
               options.DefaultApiVersion = new ApiVersion(1, 0);
               options.AssumeDefaultVersionWhenUnspecified = true;
               options.ReportApiVersions = true;
               options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
           });

        //將執行關閉較耗時的 service 最後註冊，中止時會優先執行
        services.AddHostedService<HostedService>();

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
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataServer v1"));
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
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Log.Information($"Current Environment : {env}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                dbManager.Dispose();
                logrotator.Dispose();
                dbManager = null;
                logrotator = null;

                Log.Information("Server Dispose Success!");
            }
            catch (System.Exception ex)
            {
                Log.Warning(ex, "Server Dispose Failed!");
            }

            return Task.CompletedTask;
        }
    }

    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Log.Fatal($"ExceptionFilter : {context.Exception}");
        }
    }
}