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

        // Add DbContext with SQL Authentication
        builder.Services.AddDbContext<GlamourDbContext>(options =>
            options.UseSqlServer("Server=NITRO;Database=Glamour;User Id=sa;Password=.tomAsk08.;TrustServerCertificate=True;"));

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AppointmentsPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<App>();  // Changed from Singleton to Transient

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
