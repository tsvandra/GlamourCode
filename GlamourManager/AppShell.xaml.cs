using GlamourManager.Data;

namespace GlamourManager;

public partial class AppShell : Shell
{
    private readonly GlamourDbContext _context;

    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(AppointmentsPage), typeof(AppointmentsPage));
        Routing.RegisterRoute(nameof(ServicesPage), typeof(ServicesPage));
    }
    
    // This constructor is used when AppShell is created by App.xaml.cs
    public AppShell(GlamourDbContext context) : this()
    {
        _context = context;
    }
}
