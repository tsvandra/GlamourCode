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
        MainPage = new AppShell(_context);
    }
}