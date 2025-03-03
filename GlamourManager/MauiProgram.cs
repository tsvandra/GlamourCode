using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GlamourManager.Data;

namespace GlamourManager;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add DbContext
        builder.Services.AddDbContext<GlamourDbContext>(options =>
            options.UseSqlServer("Server=NITRO;Database=Glamour;Trusted_Connection=True;TrustServerCertificate=True;"));

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AppointmentsPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddSingleton<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
