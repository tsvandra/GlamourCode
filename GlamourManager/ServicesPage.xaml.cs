using GlamourManager.Data;
using GlamourManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace GlamourManager;

public partial class ServicesPage : ContentPage
{
    private readonly GlamourDbContext _context;
    private readonly List<Service> _services = new();
    public ICommand BookServiceCommand { get; }

    public ServicesPage(GlamourDbContext context)
    {
        InitializeComponent();
        _context = context;
        BookServiceCommand = new Command<Service>(async (service) => await BookService(service));
        BindingContext = this;
        Loaded += OnPageLoaded!;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadServicesAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadServicesAsync();
    }

    private async Task LoadServicesAsync()
    {
        try
        {
            _services.Clear();
            var services = await _context.Services
                .OrderBy(s => s.Name)
                .ToListAsync();

            _services.AddRange(services);
            ServicesCollection.ItemsSource = null;
            ServicesCollection.ItemsSource = _services;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load services: " + ex.Message, "OK");
        }
    }

    private async Task BookService(Service service)
    {
        // Pass the selected service to the booking page
        var bookingParameters = new Dictionary<string, object>
        {
            { "SelectedServiceId", service.Id }
        };
        
        await Shell.Current.GoToAsync("//MainPage", bookingParameters);
    }
}