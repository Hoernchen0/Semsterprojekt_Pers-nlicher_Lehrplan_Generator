using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Data;
using LernApp.Data.Repositories;
using LernApp.Models;
using LernApp.Services;

namespace LernApp.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string dbPath)
        {
            // DbContext
            services.AddDbContext<LernAppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Generic repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Specific repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILernEinheitRepository, LernEinheitRepository>();
            services.AddScoped<IPromptRepository, PromptRepository>();
            services.AddScoped<IGenerierteCSVRepository, GenerierteCSVRepository>();

            // Services
            services.AddScoped<ILernAppLogger, ConsoleLogger>();
            services.AddScoped<ILernplanService, LernplanService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IDateiAnalyseService, DateiAnalyseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAppSettingsService, UserAppSettingsService>();

            return services;
        }
    }
}
