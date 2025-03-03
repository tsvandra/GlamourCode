using GlamourManager.Data;
using GlamourManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace GlamourManager;

public partial class AppointmentsPage : ContentPage
{
    private readonly GlamourDbContext _context;
    private readonly List<AppointmentViewModel> _appointments = new();
    public ICommand AcceptAppointmentCommand { get; }
    public ICommand RefuseAppointmentCommand { get; }
    public ICommand CancelAppointmentCommand { get; }
    public ICommand RestoreAppointmentCommand { get; }
    public ICommand DoneAppointmentCommand { get; }

    public AppointmentsPage(GlamourDbContext context)
    {
        InitializeComponent();
        _context = context;
        AcceptAppointmentCommand = new Command<AppointmentViewModel>(async (apt) => await UpdateAppointmentStatus(apt, 2)); // Accepted
        RefuseAppointmentCommand = new Command<AppointmentViewModel>(async (apt) => await UpdateAppointmentStatus(apt, 3)); // Refused
        CancelAppointmentCommand = new Command<AppointmentViewModel>(async (apt) => await UpdateAppointmentStatus(apt, 4)); // Cancelled
        RestoreAppointmentCommand = new Command<AppointmentViewModel>(async (apt) => await UpdateAppointmentStatus(apt, 1)); // Pending
        DoneAppointmentCommand = new Command<AppointmentViewModel>(async (apt) => await UpdateAppointmentStatus(apt, 5)); // Done
        
        BindingContext = this;
        Loaded += OnPageLoaded!;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadAppointmentsAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAppointmentsAsync();
    }

    private async Task LoadAppointmentsAsync()
    {
        try
        {
            _appointments.Clear();
            var appointments = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Stylist)
                .Include(a => a.Status)
                .OrderBy(a => a.DateTime)
                .ToListAsync();

            _appointments.AddRange(appointments.Select(a => new AppointmentViewModel(a)));
            AppointmentsCollection.ItemsSource = null;
            AppointmentsCollection.ItemsSource = _appointments;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load appointments: " + ex.Message, "OK");
        }
    }

    private async Task UpdateAppointmentStatus(AppointmentViewModel appointmentVm, int newStatusId)
    {
        string statusName = newStatusId switch
        {
            1 => "Pending",
            2 => "Accepted",
            3 => "Refused",
            4 => "Cancelled",
            5 => "Done",
            _ => "Unknown"
        };

        // For cancel and refuse, ask for confirmation
        if (newStatusId == 4 || newStatusId == 3 || newStatusId == 5) // Cancelled, Refused, or Done
        {
            bool confirm = await DisplayAlert($"Confirm {statusName}", 
                $"Are you sure you want to {statusName.ToLower()} this appointment?", 
                "Yes", "No");

            if (!confirm)
                return;
        }

        try
        {
            var appointment = await _context.Appointments
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == appointmentVm.Id);

            if (appointment != null)
            {
                appointment.StatusId = newStatusId;
                await _context.SaveChangesAsync();
                await LoadAppointmentsAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update appointment status: {ex.Message}", "OK");
        }
    }

    private async void OnBookNewClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}

public class AppointmentViewModel
{
    private readonly Appointment _appointment;

    public AppointmentViewModel(Appointment appointment)
    {
        _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
    }

    public int Id => _appointment.Id;
    public DateTime DateTime => _appointment.DateTime;
    public string StatusName => _appointment.Status?.Name ?? "Unknown";
    public int StatusId => _appointment.StatusId;
    public Client Client => _appointment.Client;
    public Service Service => _appointment.Service;
    public Stylist Stylist => _appointment.Stylist;

    // Status-specific button visibility
    public bool ShowAcceptButton => _appointment.StatusId == 1; // Pending
    public bool ShowRefuseButton => _appointment.StatusId == 1; // Pending
    public bool ShowCancelButton => (_appointment.StatusId == 2 || _appointment.StatusId == 1) // Accepted or Pending 
                                   && _appointment.DateTime > DateTime.Now;
    public bool ShowRestoreButton => _appointment.StatusId == 4 // Cancelled
                                  || _appointment.StatusId == 3 // Refused
                                  || _appointment.StatusId == 2; // Accepted
    public bool ShowDoneButton => _appointment.StatusId == 2; // Accepted
    
    public Color StatusColor => _appointment.StatusId switch
    {
        1 => Colors.Blue,      // Pending - Blue
        2 => Colors.Green,     // Accepted - Green
        3 => Colors.Orange,    // Refused - Orange
        4 => Colors.Red,       // Cancelled - Red
        5 => Colors.Purple,    // Done - Purple
        _ => Colors.Gray
    };
}