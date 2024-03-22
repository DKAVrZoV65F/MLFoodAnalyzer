using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MLFoodAnalyzerClient.Pages;

public partial class NetworkPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    private readonly AlertService alert = new();

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

        if ((string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0) || !IsValidIpAddress(AppShell.settings.Ip) || !IsValidPort(AppShell.settings.Port))
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
        Regex validateIPv4Regex = new("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");  // prints True
        return validateIPv4Regex.IsMatch(ipAddress);
    }

    private bool IsValidPort(int port) => port is >= 49152 and <= 65535;

    private async Task PingServerAsync(string ipAddress, int port)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;

        using TcpClient tcpClient = new();
        try
        {
            await tcpClient.ConnectAsync(ipAddress, port);
            var stream = tcpClient.GetStream();

            //  buffer for incoming data
            var response = new List<byte>();
            NetworkStream networkStream = tcpClient.GetStream();

            int bytesRead = 10; //  to read bytes from a stream
            await stream.WriteAsync(Encoding.UTF8.GetBytes("PING\0"));
            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            var translation = Encoding.UTF8.GetString(response.ToArray());
            if ("SUCCESS" == translation)
            {
                Preferences.Set("SavedIpServer", ipAddress);
                Preferences.Set("SavedPortServer", port);
                
                task = true;
            }

            response.Clear();
            networkStream.Close();
        }
        catch {}
        finally
        {
            tcpClient.Close();
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