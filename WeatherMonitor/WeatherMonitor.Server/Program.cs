
using Microsoft.EntityFrameworkCore;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Infrastructure.Data;
using WeatherMonitor.Infrastructure.Repositories;
using WeatherMonitor.Server.Middleware;
using WeatherMonitor.Server.Services;
using WeatherMonitor.Services.Services;

namespace WeatherMonitor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register services
            builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddHttpClient<IWeatherApiClient, OpenWeatherMapClient>();

            // Register background service
            builder.Services.AddHostedService<WeatherUpdateService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var client = builder.Configuration.GetSection("AllowedUrls").GetValue<string>("weathermonitor.client");

            var app = builder.Build();

            app.UseCors(opt => opt
                .WithOrigins(client)
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}
