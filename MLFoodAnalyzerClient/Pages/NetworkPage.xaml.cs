using MLFoodAnalyzerClient.Extension;
using System.Text.RegularExpressions;

namespace MLFoodAnalyzerClient.Pages;

public partial class NetworkPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    private readonly AlertService alert = new();
    private Connection? connection;

    private bool IsFlag = true;
    private bool task = false;

    public NetworkPage()
    {
        InitializeComponent();

        TitleLabel.FontSize = AppShell.settings.FSize + 5;
        AppShell.settings = (Settings)Resources["settings"];
    }


    private async void PingServer(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        IsFlag = false;
        CheckIpPortButton.IsInProgress = true;

        if ((string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0) || !IsValidIpAddress(AppShell.settings.Ip) 
            || !IsValidPort(AppShell.settings.Port))
        {
            IsFlag = true;
            CheckIpPortButton.IsInProgress = false;

            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        await PingServerAsync(AppShell.settings.Ip, AppShell.settings.Port);

        string? result = (task) ? LocalizationResourceManager["Success"].ToString() : LocalizationResourceManager["DestinationHostUn"].ToString();
        alert.DisplayMessage(result);

        task = false;
        IsFlag = true;
        CheckIpPortButton.IsInProgress = false;
    }

    private bool IsValidIpAddress(string ipAddress)
    {
        Regex validateIPv4Regex = new("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\." +
            "(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");  // prints True
        return validateIPv4Regex.IsMatch(ipAddress);
    }

    private bool IsValidPort(int port) => port is >= 49152 and <= 65535;

    private async Task PingServerAsync(string ipAddress, int port)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;

        connection ??= new();
        string result = await connection.PingServer() ?? string.Empty;
        if ("SUCCESS" == result)
        {
            Preferences.Set("SavedIpServer", ipAddress);
            Preferences.Set("SavedPortServer", port);

            task = true;
        }
    }

    private async void QRScanner(object sender, EventArgs e) => await Navigation.PushModalAsync(new QRScanner());

    private void SavePSWD(object sender, EventArgs e)
    {
        string password = PasswordEntry.Text;
        AppShell.settings.Password = password;
        _ = SecureStorage.SetAsync("SavedPasswordServer", password);
        alert.DisplayMessage(LocalizationResourceManager["PSWDServer"].ToString());
    }

    private void DisplayPassword_Changed(object sender, CheckedChangedEventArgs e) => PasswordEntry.IsPassword = !e.Value;
}