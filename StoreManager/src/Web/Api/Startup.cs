using Api.Configuration;
using Infrastructure;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwagger();
        services.AddMigrations(Configuration);
        services.AddAuthConfiguration(Configuration);
        services.AddDependencyInjection();
        services.AddAutoMapper();
        services.AddFluentValidationConfiguration();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwaggerConfiguration();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthConfiguration();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}