using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

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

        ResultEditor.Text += "You: ";
        foreach (var item in text)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }
        ResultEditor.Text += '\n';

        await SendText(text);
        ResultEditor.Text += "Server: ";
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

        ResultEditor.Text += "You pick a photo \"";
        foreach (var item in path)
        {
            ResultEditor.Text += item;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }
        ResultEditor.Text += "\"\n";
        await SendPicture(path);

        ResultEditor.Text += "Server: ";
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
#if ANDROID
        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Сфоткать", "Медиа");
        string res = (result) ? await GetPicture() : await GetMedia();
        return res;
#elif IOS
        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Сфоткать", "Медиа");
        string res = (result) ? await GetPicture() : await GetMedia();
        return res;
#else
        string res = await GetMedia();
        return res;
#endif
    }

    private async Task<string> GetPicture()
    {
        FileResult myPhoto = await MediaPicker.Default.CapturePhotoAsync();
        if (myPhoto == null) return "";
        string localFilePath = Path.Combine(FileSystem.CacheDirectory, myPhoto.FileName);
        using Stream sourceStream = await myPhoto.OpenReadAsync();
        using FileStream localFileStream = File.OpenWrite(localFilePath);
        await sourceStream.CopyToAsync(localFileStream);
        localFileStream.Close();
        return localFilePath;
    }

    private async Task<string> GetMedia()
    {
        FileResult myPhoto = await MediaPicker.Default.PickPhotoAsync();
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
        if (string.IsNullOrEmpty(ipServer))
        {
            textFromServer = "Error with server";
            return;
        }
        textFromServer = "";

        await tcpClient.ConnectAsync(ipServer, portServer);
        var stream = tcpClient.GetStream();

        // буфер для входящих данных
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; // для считывания байтов из потока
        await stream.WriteAsync(Encoding.UTF8.GetBytes("IMAGE" + "\0"));

        string fileName = path;
        FileInfo fileInfo = new(fileName);
        long fileSize = fileInfo.Length;

        if (!string.IsNullOrEmpty(Preferences.Get("SavedPasswordServer", "")))
        {
            string encword = EncryptText(fileSize.ToString());
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"1{encword}\0"));
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

        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }
        var translation = Encoding.UTF8.GetString(response.ToArray());
        textFromServer = translation + '\n';

        response.Clear();
        networkStream.Close();
    }

    private async Task SendText(string text)
    {
        using TcpClient tcpClient = new();
        ipServer = Preferences.Get("SavedIpServer", "");

        if (string.IsNullOrEmpty(ipServer))
        {
            textFromServer = "Error with server";
            return;
        }
        textFromServer = "";
        await tcpClient.ConnectAsync(ipServer, portServer);
        var stream = tcpClient.GetStream();

        // буфер для входящих данных
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; // для считывания байтов из потока
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));

        if (!string.IsNullOrEmpty(Preferences.Get("SavedPasswordServer", "")))
        {
            text = EncryptText(text);
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"1{text}\0"));
            try
            {
                while ((bytesRead = stream.ReadByte()) != '\0')
                {
                    // добавляем в буфер
                    response.Add((byte)bytesRead);
                }
            }
            finally
            {
                textFromServer = "Error on server 2\n";
            }

            string translation = Encoding.UTF8.GetString(response.ToArray());
            textFromServer = (DecryptText(translation))[1..] + '\n';
        }
        else
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"0{text}\0"));
            try
            {
                while ((bytesRead = stream.ReadByte()) != '\0')
                {
                    // добавляем в буфер
                    response.Add((byte)bytesRead);
                }
            }
            finally
            {
                textFromServer = "Error on server 2\n";
            }
            string translation = Encoding.UTF8.GetString(response.ToArray());
            textFromServer = translation[1..] + '\n';
        }

        response.Clear();
        networkStream.Close();
    }


    private static string DecryptText(string CipherText)
    {
        byte[] toEncryptArray = Convert.FromBase64String(CipherText);
        MD5CryptoServiceProvider objMD5CryptoService = new();

        //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(Preferences.Get("SavedPasswordServer", "")));
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            //Assigning the Security key to the TripleDES Service Provider.
            Key = securityKeyArray,
            //Mode of the Crypto service is Electronic Code Book.
            Mode = CipherMode.ECB,
            //Padding Mode is PKCS7 if there is any extra byte is added.
            Padding = PaddingMode.PKCS7
        };

        var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
        //Transform the bytes array to resultArray
        byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        objTripleDESCryptoService.Clear();

        //Convert and return the decrypted data/byte into string format.
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    private static string EncryptText(string PlainText)
    {
        // Getting the bytes of Input String.
        byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

        MD5CryptoServiceProvider objMD5CryptoService = new();
        //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(Preferences.Get("SavedPasswordServer", "")));
        //De-allocatinng the memory after doing the Job.
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            //Assigning the Security key to the TripleDES Service Provider.
            Key = securityKeyArray,
            //Mode of the Crypto service is Electronic Code Book.
            Mode = CipherMode.ECB,
            //Padding Mode is PKCS7 if there is any extra byte is added.
            Padding = PaddingMode.PKCS7
        };


        var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
        //Transform the bytes array to resultArray
        byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
        objTripleDESCryptoService.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
}
