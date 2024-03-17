using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService alert;

    private const string errorServer = "ErrorConToServ";
    private string text = "";
    private string textFromServer = "";
    private bool IsFlag = true;
    bool IsPolicyRead;

    readonly Random rnd = new();
    readonly int minValue = 5;
    readonly int maxValue = 25;

    public MainPage()
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead) GoToPolicy();

        InitializeComponent();

        alert = new();

        InfoLabel.FontSize = AppShell.settings.FSize + 5;
    }

    private async void GoToPolicy() => await Navigation.PushModalAsync(new PolicyPage());

    private async void SendText_Tapped(object sender, EventArgs e)
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead)
        {
            GoToPolicy();
            return;
        }

        text = QueryEditor.Text;
        if (string.IsNullOrEmpty(text) || !IsFlag) return;

        IsFlag = false;
        SendTextButton.IsInProgress = true;
        SendPictureButton.IsInProgress = true;
        QueryEditor.Text = "";

        ResultEditor.Text += LocalizationResourceManager["You"].ToString();
        foreach (var item in text)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }
        ResultEditor.Text += '\n';

        await SendText(text);
        ResultEditor.Text += LocalizationResourceManager["Server"].ToString();
        foreach (var item in textFromServer)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }

        SendTextButton.IsInProgress = false;
        SendPictureButton.IsInProgress = false;
        text = "";
        IsFlag = true;
    }

    private void QueryEditor_Changed(object sender, TextChangedEventArgs e)
    {
        if (SendPictureButton == null || SendTextButton == null) return;

        SendPictureButton.IsVisible = string.IsNullOrEmpty(QueryEditor.Text);
        SendTextButton.IsVisible = !string.IsNullOrEmpty(QueryEditor.Text);
    }

    private async void SendPicture_Tapped(object sender, EventArgs e)
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead)
        {
            GoToPolicy();
            return;
        }

        if (!IsFlag) return;

        IsFlag = false;
        SendTextButton.IsInProgress = true;
        SendPictureButton.IsInProgress = true;

        string path = await GetPicturePath();
        if (string.IsNullOrEmpty(path))
        {
            SendTextButton.IsInProgress = false;
            SendPictureButton.IsInProgress = false;
            IsFlag = true;
            return;
        }

        ResultEditor.Text += LocalizationResourceManager["AttachedAPicture"].ToString() + '\"';
        foreach (var item in path)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }
        ResultEditor.Text += "\"\n";
        await SendPicture(path);

        ResultEditor.Text += LocalizationResourceManager["Server"].ToString();
        foreach (var item in textFromServer)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }

        SendTextButton.IsInProgress = false;
        SendPictureButton.IsInProgress = false;
        IsFlag = true;
    }

    private async Task<string> GetPicturePath()
    {
#if ANDROID || IOS
        bool result = await alert.DisplayMessage(LocalizationResourceManager["SelectAnAction"].ToString(), LocalizationResourceManager["TakeAPicture"].ToString(), LocalizationResourceManager["Gallery"].ToString());
        string res = (result) ? await GetPicture() : await GetMedia();
        return res;
#else
        string res = await GetMedia();
        return res;
#endif
    }

    private async Task<string> GetPicture() => await GetPathToImage(await MediaPicker.Default.CapturePhotoAsync());

    private async Task<string> GetMedia() => await GetPathToImage(await MediaPicker.Default.PickPhotoAsync());

    private async Task<string> GetPathToImage(FileResult? myPhoto)
    {
        if (myPhoto == null) return "";
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, myPhoto.FileName);
        using Stream sourceStream = await myPhoto.OpenReadAsync();
        using FileStream localFileStream = File.OpenWrite(localFilePath);
        await sourceStream.CopyToAsync(localFileStream);
        localFileStream.Close();
        return localFilePath;
    }

    private async Task SendPicture(string path)
    {
        string fileName = path;
        FileInfo fileInfo = new(fileName);
        long fileSize = fileInfo.Length;

        if (fileSize >= 8_000_000)
        {
            alert.DisplayMessage($"The image must not exceed 8MB! You are uploading a picture with the size {fileSize / 1_000_000} MB");
            return;
        }


        using TcpClient tcpClient = new();
        textFromServer = "";
        if (string.IsNullOrEmpty(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            textFromServer += LocalizationResourceManager[errorServer].ToString();
            return;
        }

        await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("IMAGE\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{fileSize}\0"));

        byte[] buffer = new byte[1024];
        int bytesReadImg;
        string filePath = path;
        FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

        while ((bytesReadImg = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            tcpClient.GetStream().Write(buffer, 0, bytesReadImg);
        }
        fileStream.Close();

        while ((bytesRead = stream.ReadByte()) != '\0') response.Add((byte)bytesRead);  //  Adding to the buffer

        string translation = Encoding.UTF8.GetString(response.ToArray());
        textFromServer = translation + '\n';

        response.Clear();
        networkStream.Close();
    }

    private async Task SendText(string text)
    {
        if (string.IsNullOrEmpty(AppShell.settings.Ip))
        {
            textFromServer += LocalizationResourceManager[errorServer].ToString();
            return;
        }

        using TcpClient tcpClient = new();
        textFromServer = "";
        await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{text}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);  //  Adding to the buffer

        string translation = Encoding.UTF8.GetString(response.ToArray());
        textFromServer = translation + '\n';

        response.Clear();
        networkStream.Close();
    }
}
