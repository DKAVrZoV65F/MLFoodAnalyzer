using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService? alert;

    private const string logo = """
                          ``
          `.              `ys
          +h+             +yh-
          yyh:           .hyys
         .hyyh.          oyyyh`
         /yyyyy`        .hyydy/
         syyhhy+        oyyhsys
         hyyyoyh.      .hyyy:hh`
        .hyyyy:ho      +yyys-yh-
        :hyyyh-oh.    `hyyyo-oy/
        /yyyyh-:h+    -hyyh/-oy+
        +yyyyh:-yy    +yyyh--oyo
        +yyyyh/-sh.   syyyh--oyo
        +yyyyh/-oy/  `hyyyy--syo
        +yyyyh/-+y+  `hyyys--yy+
        :yyyyh/-+ys  .hyyyo-:hy:
        .hyyyh+-+ys  .hyyyo-oyh`
        `yyyyyo-oyy  .hyyy+-yyy
         +yyyys-syy  `hyyh/oyy/
         .hyyyh-hyy  `hyyh/hyh
          oyyyhshys   yyyhyyy+
          oyyyhshys   yyyhyyy+
           /hyyyyyo`.-oyyyyh/
           `syyyyyyyhyyyyyyho.
            .hyyyyhNdyyyyyyymh/`
        """;
    private const string errorServer = "ErrorConToServ";
    //private string textFromServer = string.Empty;
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

        string text = QueryEditor.Text;
        QueryEditor.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(text) || !IsFlag) return;

        IsFlag = false;
        SendTextButton.IsInProgress = true;
        SendPictureButton.IsInProgress = true;

        if (text[0].Equals('/'))
        {
            await Command(text[1..]);
            SendTextButton.IsInProgress = false;
            SendPictureButton.IsInProgress = false;
            IsFlag = true;
            return;
        }

        ResultEditor.Text += LocalizationResourceManager["AttachedAText"].ToString() + '\n';

        Connection connection = new();
        text = await connection.SendText(text) ?? string.Empty;
        ResultEditor.Text += LocalizationResourceManager["Server"].ToString();
        foreach (var item in text)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }

        SendTextButton.IsInProgress = false;
        SendPictureButton.IsInProgress = false;
        IsFlag = true;
    }

    private void QueryEditor_Changed(object sender, TextChangedEventArgs e)
    {
        if (SendPictureButton is null || SendTextButton is null) return;

        SendPictureButton.IsVisible = string.IsNullOrWhiteSpace(QueryEditor.Text);
        SendTextButton.IsVisible = !string.IsNullOrWhiteSpace(QueryEditor.Text);
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
        if (string.IsNullOrWhiteSpace(path))
        {
            SendTextButton.IsInProgress = false;
            SendPictureButton.IsInProgress = false;
            IsFlag = true;
            return;
        }

        ResultEditor.Text += LocalizationResourceManager["AttachedAPicture"].ToString() + '\n';
        string text = await SendPicture(path) ?? string.Empty;

        ResultEditor.Text += LocalizationResourceManager["Server"].ToString();
        foreach (var item in text)
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
        alert ??= new();
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
        if (myPhoto is null) return string.Empty;
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, myPhoto.FileName);
        using Stream sourceStream = await myPhoto.OpenReadAsync();
        using FileStream localFileStream = File.OpenWrite(localFilePath);
        await sourceStream.CopyToAsync(localFileStream);
        localFileStream.Close();
        return localFilePath;
    }

    private async Task<string?> SendPicture(string path)
    {
        string fileName = path;
        FileInfo fileInfo = new(fileName);
        long fileSize = fileInfo.Length;

        if (fileSize >= 8_000_000) return $"{LocalizationResourceManager["LimitImage"]}{fileSize / 1_000_000} MB";


        using TcpClient tcpClient = new();
        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            return LocalizationResourceManager[errorServer].ToString();
        }

        await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
        var stream = tcpClient.GetStream();

        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10;
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

        while ((bytesRead = stream.ReadByte()) != '\0') response.Add((byte)bytesRead);

        string translation = Encoding.UTF8.GetString(response.ToArray());

        response.Clear();
        networkStream.Close();
        return translation + '\n';
    }

    /*
    private async Task<string?> SendText(string text)
    {
        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip)) return LocalizationResourceManager[errorServer].ToString();

        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{text}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);

        string translation = Encoding.UTF8.GetString(response.ToArray());

        response.Clear();
        networkStream.Close();
        return translation + '\n';
    }*/

    private async Task Command(string command)
    {
        switch (command)
        {
            case "clear":
                ResultEditor.Text = string.Empty;
                break;
            case "Kamchatka":
                ResultEditor.FontFamily = "LogoFont";
                ResultEditor.Text = logo;
                ResultEditor.TextType = TextType.Text;
                await Task.Delay(5000);
                ResultEditor.FontFamily = "RegularFont";
                ResultEditor.TextType = TextType.Html;
                break;
            default:
                break;
        }
    }
}





/*
                  ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/