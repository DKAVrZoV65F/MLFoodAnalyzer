using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class UpdatingStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private bool IsFlag = true;
    private AlertService? alert;
    private Food foodDetail;

    public UpdatingStoragePage(Food xyz)
    {
        InitializeComponent();

        BindingContext = this;

        foodDetail = xyz;
        IDLabel.Text = $"ID: {foodDetail.Id}";
        NameLabel.Text = $"{LocalizationResourceManager["Title"]} {foodDetail.Name[0].ToString().ToUpper()}{foodDetail.Name[1..]}";
        DescriptionEntry.Text = $"{foodDetail.Description}";
    }

    private async void SavingData(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        if (string.IsNullOrEmpty(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            alert ??= new();
            IsFlag = true;
            SavingButton.IsInProgress = false;
            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        IsFlag = false;
        SavingButton.IsInProgress = true;

        using TcpClient tcpClient = new();
        string translation = string.Empty;
        try
        {
            await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            NetworkStream networkStream = tcpClient.GetStream();

            int bytesRead = 10;
            await stream.WriteAsync(Encoding.UTF8.GetBytes("Update\0"));
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"{AppShell.settings.NickName}|{foodDetail.Id}|{DescriptionEntry.Text}\0"));

            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            translation = Encoding.UTF8.GetString(response.ToArray());
            response.Clear();
            networkStream.Close();
        }
        catch { }
        finally
        {
            tcpClient.Close();
        }

        IsFlag = true;
        SavingButton.IsInProgress = false;
        alert ??= new();
        alert.DisplayMessage(translation.Equals("success") ? LocalizationResourceManager["SuccessUpdateDescr"].ToString() : LocalizationResourceManager["ErrorUpdateDescr"].ToString());
        if (translation.Equals("success")) await Navigation.PopAsync();
    }
}