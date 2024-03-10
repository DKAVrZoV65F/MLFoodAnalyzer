using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MLFoodAnalyzerClient.Pages;

public partial class NetworkPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private bool IsFlag = true;
    private bool task = false;

    public static QRScan qR = new();


    public NetworkPage()
    {
        InitializeComponent();

        int getValue = Preferences.Get("FontSize", 20);
        TitleLabel.FontSize = getValue + 5;
        IPLabel.FontSize = getValue;
        IPEntry.FontSize = getValue;
        PortLabel.FontSize = getValue;
        PortEntry.FontSize = getValue;
        PasswordLabel.FontSize = getValue;
        PasswordEntry.FontSize = getValue;

        IPEntry.Text = Preferences.Get("SavedIpServer", "");
        PortEntry.Text = Preferences.Get("SavedPortServer", 0).ToString();
        PasswordEntry.Text = Preferences.Get("SavedPasswordServer", "");

        qR = Resources["qR"] as QRScan;
    }


    private async void PingServer(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        IsFlag = false;
        CheckIpPortButton.IsInProgress = true;

        string ipAddress = IPEntry.Text;
        int port = Convert.ToInt32(PortEntry.Text);
        if ((string.IsNullOrEmpty(ipAddress) || port == 0) || !IsValidIpAddress(ipAddress) || !IsValidPort(port))
        {
            IsFlag = true;
            CheckIpPortButton.IsInProgress = false;
            await DisplayAlert(LocalizationResourceManager["AppName"].ToString(), LocalizationResourceManager["ErrorWithIPOrPort"].ToString(), "OK");
            return;
        }
        string password = PasswordEntry.Text;

        await PingServerAsync(ipAddress, port, password);

        string? result = (task) ? LocalizationResourceManager["Success"].ToString() : LocalizationResourceManager["DestinationHostUn"].ToString();
        await DisplayAlert(LocalizationResourceManager["AppName"].ToString(), result, "OK");

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

    private async Task PingServerAsync(string ipAddress, int port, string password)
    {
        if (string.IsNullOrEmpty(ipAddress)) return;


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
            {
                //  adding to the buffer
                response.Add((byte)bytesRead);
            }

            var translation = Encoding.UTF8.GetString(response.ToArray());
            if ("SUCCESS" == translation)
            {
                Preferences.Set("SavedIpServer", ipAddress);
                Preferences.Set("SavedPortServer", port);
                Preferences.Set("SavedPasswordServer", password);
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
}