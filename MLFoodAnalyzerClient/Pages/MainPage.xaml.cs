using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private const string errorServer = "ErrorConToServ";
    private string text = "";
    private string textFromServer = "";
    private bool IsFlag = true;

    private string ipServer = "";
    private int portServer = 55555;


    readonly Random rnd = new();
    readonly int minValue = 10;
    readonly int maxValue = 50;

    public MainPage()
    {
        bool IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead) GoToPolicy();

        InitializeComponent();

        int getValue = Preferences.Get("FontSize", 20);
        InfoLabel.FontSize = getValue;
        ResultEditor.FontSize = getValue;
        QueryEditor.FontSize = getValue;
    }

    private async void GoToPolicy() => await Navigation.PushModalAsync(new PolicyPage());

    private async void SendText_Tapped(object sender, EventArgs e)
    {
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
        bool result = await DisplayAlert(LocalizationResourceManager["AppName"].ToString(), LocalizationResourceManager["SelectAnAction"].ToString(), LocalizationResourceManager["TakeAPicture"].ToString(), LocalizationResourceManager["Gallery"].ToString());
        string res = (result) ? await GetPicture() : await GetMedia();
        return res;
#else
        string res = await GetMedia();
        return res;
#endif
    }

    private async Task<string> GetPicture() => await GetPathToImage(await MediaPicker.Default.CapturePhotoAsync());

    private async Task<string> GetMedia() => await GetPathToImage(await MediaPicker.Default.PickPhotoAsync());

    private async Task<string> GetPathToImage(FileResult myPhoto)
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
        using TcpClient tcpClient = new();
        ipServer = Preferences.Get("SavedIpServer", "");
        portServer = Preferences.Get("SavedPortServer", 0);
        textFromServer = "";
        if (string.IsNullOrEmpty(ipServer) || portServer == 0)
        {
            textFromServer += LocalizationResourceManager[errorServer].ToString();
            return;
        }

        await tcpClient.ConnectAsync(ipServer, portServer);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("IMAGE" + "\0"));

        string fileName = path;
        FileInfo fileInfo = new(fileName);
        long fileSize = fileInfo.Length;

        if (!string.IsNullOrEmpty(Preferences.Get("SavedPasswordServer", "")))
        {
            string encWord = EncryptText(fileSize.ToString());
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"1{encWord}\0"));
        }
        else
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"0{fileSize}\0"));
        }

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
        textFromServer = translation[0] == '1' ? DecryptText(translation[1..]) + '\n' : translation[1..] + '\n';

        response.Clear();
        networkStream.Close();
    }

    private async Task SendText(string text)
    {
        using TcpClient tcpClient = new();
        ipServer = Preferences.Get("SavedIpServer", "");
        textFromServer = "";

        if (string.IsNullOrEmpty(ipServer))
        {
            textFromServer += LocalizationResourceManager[errorServer].ToString();
            return;
        }
        await tcpClient.ConnectAsync(ipServer, portServer);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));

        if (!string.IsNullOrEmpty(Preferences.Get("SavedPasswordServer", "")))
        {
            text = EncryptText(text);
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"1{text}\0"));
        }
        else
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"0{text}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);  //  Adding to the buffer

        string translation = Encoding.UTF8.GetString(response.ToArray());
        textFromServer = (DecryptText(translation))[1..] + '\n';

        response.Clear();
        networkStream.Close();
    }

    // Disable the warning.
#pragma warning disable SYSLIB0021
    private static string DecryptText(string CipherText)
    {
        byte[] toEncryptArray = Convert.FromBase64String(CipherText);

        MD5CryptoServiceProvider objMD5CryptoService = new();
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(Preferences.Get("SavedPasswordServer", "")));
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            Key = securityKeyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        byte[] resultArray = objTripleDESCryptoService.CreateDecryptor().TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        objTripleDESCryptoService.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    private static string EncryptText(string PlainText)
    {
        byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

        MD5CryptoServiceProvider objMD5CryptoService = new();
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(Preferences.Get("SavedPasswordServer", "")));
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            Key = securityKeyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        byte[] resultArray = objTripleDESCryptoService.CreateEncryptor().TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
        objTripleDESCryptoService.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    // Re-enable the warning.
#pragma warning restore SYSLIB0021
}
