using GlamourManager.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GlamourManager;

public partial class App : Application
{
    private readonly GlamourDbContext _context;

    public App(GlamourDbContext context)
    {
        InitializeComponent();
        _context = context;

        // Create shell with the context and set as MainPage
        var appShell = new AppShell(_context);
        // MainPage = appShell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "Glamour Hair Salon";
        
        window.Page = new AppShell(_context);
        return window;
    }
}