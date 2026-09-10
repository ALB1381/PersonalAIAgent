using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalAIAgent.Core.Data;
using PersonalAIAgent.Core.Interfaces;
using PersonalAIAgent.Core.Services;


namespace PersonalAIAgent.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<IAgentService, AgentService>();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonalAIAgent.db");
            builder.Services.AddDbContext<Context>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
