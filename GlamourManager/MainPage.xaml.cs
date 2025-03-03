using GlamourManager.Data;
using GlamourManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GlamourManager;

[QueryProperty(nameof(SelectedServiceId), "SelectedServiceId")]
public partial class MainPage : ContentPage
{
    private readonly GlamourDbContext _context;
    private readonly List<Service> _services = new();
    private readonly List<Stylist> _stylists = new();

    // Define business hours (9 AM to 5 PM)
    private static readonly TimeSpan BusinessStart = new(9, 0, 0);
    private static readonly TimeSpan BusinessEnd = new(17, 0, 0);
    
    private int _selectedServiceId;
    public int SelectedServiceId 
    { 
        get => _selectedServiceId;
        set
        {
            _selectedServiceId = value;
            OnSelectedServiceIdChanged();
        }
    }

    public MainPage(GlamourDbContext context)
    {
        InitializeComponent();
        _context = context;
        Loaded += OnPageLoaded!;
        
        // Set default time to business start
        AppointmentTime.Time = BusinessStart;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _services.Clear();
            _services.AddRange(await _context.Services.ToListAsync());

            _stylists.Clear();
            _stylists.AddRange(await _context.Stylists.ToListAsync());

            SetupPickers();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load data: " + ex.Message, "OK");
        }
    }

    private void SetupPickers()
    {
        ServicePicker.ItemsSource = _services;
        ServicePicker.ItemDisplayBinding = new Binding("Name");

        StylistPicker.ItemsSource = _stylists;
        StylistPicker.ItemDisplayBinding = new Binding("Name");
    }

    private void OnServiceSelected(object sender, EventArgs e)
    {
        ServiceDetailsPanel.IsVisible = ServicePicker.SelectedItem != null;
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        if (!ValidateInputs())
        {
            await DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        if (!ValidateAppointmentTime())
        {
            await DisplayAlert("Error", "Please select a time between 9 AM and 5 PM", "OK");
            return;
        }

        try
        {
            var selectedService = (Service)ServicePicker.SelectedItem;
            var appointmentDateTime = AppointmentDate.Date.Add(AppointmentTime.Time);
            var appointmentEndTime = appointmentDateTime.AddMinutes(selectedService.DurationMinutes);

            // Check for overlapping appointments
            var hasOverlap = await CheckForOverlappingAppointments(appointmentDateTime, appointmentEndTime);
            if (hasOverlap)
            {
                await DisplayAlert("Error", "This time slot is already booked. Please select a different time.", "OK");
                return;
            }

            var appointment = await CreateAppointmentAsync();
            await DisplayAlert("Success", "Your appointment has been booked!", "OK");
            ClearForm();
            
            // Navigate to appointments page using Shell navigation
            await Shell.Current.GoToAsync("//AppointmentsPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to book appointment: " + ex.Message, "OK");
        }
    }

    private async void OnViewAppointmentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AppointmentsPage");
    }

    private bool ValidateInputs()
    {
        return !string.IsNullOrWhiteSpace(NameEntry.Text)
            && !string.IsNullOrWhiteSpace(PhoneEntry.Text)
            && !string.IsNullOrWhiteSpace(EmailEntry.Text)
            && ServicePicker.SelectedItem != null
            && StylistPicker.SelectedItem != null;
    }

    private bool ValidateAppointmentTime()
    {
        var time = AppointmentTime.Time;
        return time >= BusinessStart && time <= BusinessEnd;
    }

    private async Task<bool> CheckForOverlappingAppointments(DateTime start, DateTime end)
    {
        var selectedStylist = (Stylist)StylistPicker.SelectedItem;
        
        return await _context.Appointments
            .Where(a => a.StylistId == selectedStylist.Id)
            .AnyAsync(a => 
                (a.DateTime <= start && a.DateTime.AddMinutes(a.Service.DurationMinutes) > start) ||
                (a.DateTime < end && a.DateTime.AddMinutes(a.Service.DurationMinutes) >= end));
    }

    private async Task<Appointment> CreateAppointmentAsync()
    {
        var selectedDate = AppointmentDate.Date;
        var selectedTime = AppointmentTime.Time;
        var appointmentDateTime = selectedDate.Add(selectedTime);

        // Check if client already exists
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == EmailEntry.Text);

        if (client == null)
        {
            client = new Client
            {
                Name = NameEntry.Text,
                PhoneNumber = PhoneEntry.Text,
                Email = EmailEntry.Text
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        var appointment = new Appointment
        {
            DateTime = appointmentDateTime,
            ClientId = client.Id,
            ServiceId = ((Service)ServicePicker.SelectedItem).Id,
            StylistId = ((Stylist)StylistPicker.SelectedItem).Id,
            StatusId = AppointmentStatus.Pending
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return appointment;
    }

    private void ClearForm()
    {
        NameEntry.Text = string.Empty;
        PhoneEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        ServicePicker.SelectedItem = null;
        StylistPicker.SelectedItem = null;
        AppointmentDate.Date = DateTime.Today;
        AppointmentTime.Time = DateTime.Now.TimeOfDay;
    }

    private void OnSelectedServiceIdChanged()
    {
        if (_selectedServiceId > 0 && _services.Count > 0)
        {
            var service = _services.FirstOrDefault(s => s.Id == _selectedServiceId);
            if (service != null)
            {
                ServicePicker.SelectedItem = service;
                ServiceDetailsPanel.IsVisible = true;
            }
        }
    }
}

