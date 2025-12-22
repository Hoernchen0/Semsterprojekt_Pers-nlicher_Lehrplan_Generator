using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LernApp.Services
{
    public static class ServiceCollectionExtensions
    {
        // Central registration for LernApp core services and repositories.
        // Call from host apps (e.g. LehrplanGenerator) to reuse same logic.
        public static IServiceCollection AddLernAppServices(this IServiceCollection services, string? dbPath = null)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lernapp.db");
            }

            services.AddDbContext<LernApp.Data.LernAppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Repositories
            services.AddScoped<LernApp.Data.Repositories.IRepository<LernApp.Models.DateiAnalyse>, LernApp.Data.Repositories.Repository<LernApp.Models.DateiAnalyse>>();
            services.AddScoped<LernApp.Data.Repositories.IRepository<LernApp.Models.GenerierteCSV>, LernApp.Data.Repositories.Repository<LernApp.Models.GenerierteCSV>>();
            services.AddScoped<LernApp.Data.Repositories.IRepository<LernApp.Models.Prompt>, LernApp.Data.Repositories.Repository<LernApp.Models.Prompt>>();
            services.AddScoped<LernApp.Data.Repositories.IRepository<LernApp.Models.UserEinstellung>, LernApp.Data.Repositories.Repository<LernApp.Models.UserEinstellung>>();

            services.AddScoped<LernApp.Data.Repositories.IUserRepository, LernApp.Data.Repositories.UserRepository>();
            services.AddScoped<LernApp.Data.Repositories.ILernEinheitRepository, LernApp.Data.Repositories.LernEinheitRepository>();
            services.AddScoped<LernApp.Data.Repositories.IPromptRepository, LernApp.Data.Repositories.PromptRepository>();
            services.AddScoped<LernApp.Data.Repositories.IGenerierteCSVRepository, LernApp.Data.Repositories.GenerierteCSVRepository>();

            // Services
            services.AddScoped<ILernAppLogger, LernApp.Services.ConsoleLogger>();
            services.AddScoped<ILernplanService, LernApp.Services.LernplanService>();
            services.AddScoped<IAIService, LernApp.Services.AIService>();
            services.AddScoped<IDateiAnalyseService, LernApp.Services.DateiAnalyseService>();
            services.AddScoped<IUserService, LernApp.Services.UserService>();
            services.AddScoped<IUserAppSettingsService, LernApp.Services.UserAppSettingsService>();

            return services;
        }
    }
}
