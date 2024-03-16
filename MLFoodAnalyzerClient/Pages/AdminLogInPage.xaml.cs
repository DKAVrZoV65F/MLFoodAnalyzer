using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminLogInPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private static Settings settings = AppShell.settings;
    private bool IsFlag = true;
    private AlertService? alert;

    public AdminLogInPage()
    {

        InitializeComponent();

        settings = (Settings)Resources["settings"];
        TitleLabel.FontSize = settings.FSize + 5;
        SavingCheckBox.IsChecked = !string.IsNullOrEmpty(settings.Login);
    }

    private void DisplayPassword_Changed(object sender, CheckedChangedEventArgs e) => PasswordEntry.IsPassword = !e.Value;

    public async void LogIn_Tapped(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        _ = SecureStorage.SetAsync("SavedLogIn", SavingCheckBox.IsChecked ? settings.Login : string.Empty);
        _ = SecureStorage.SetAsync("SavedPassword", SavingCheckBox.IsChecked ? settings.SavedPassword : string.Empty);

        IsFlag = false;
        LogInButton.IsInProgress = true;

        if (string.IsNullOrEmpty(settings.Login) || string.IsNullOrEmpty(settings.SavedPassword))
        {
            IsFlag = true;
            LogInButton.IsInProgress = false;
            return;
        }

        string encryptText = EncryptText($"{settings.Login}|{settings.SavedPassword}");
        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(settings.Ip, settings.Port);
        var stream = tcpClient.GetStream();
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10;
        await stream.WriteAsync(Encoding.UTF8.GetBytes("LOGIN\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{encryptText}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);

        string translation = Encoding.UTF8.GetString(response.ToArray());
        translation = DecryptText(translation);
        response.Clear();
        networkStream.Close();

        IsFlag = true;
        LogInButton.IsInProgress = false;
        if (!translation.Equals("0"))
        {
            if (alert == null) alert = new();
            alert.DisplayMessage(LocalizationResourceManager["ErrorLogIn"].ToString());
            await Navigation.PushAsync(new AdminStoragePage());
        }
    }

    private string DecryptText(string CipherText)
    {
        byte[] toEncArray = Convert.FromBase64String(CipherText);
        return UTF8Encoding.UTF8.GetString(CryptoService(toEncArray));
    }

    private string EncryptText(string PlainText)
    {
        byte[] toEncArray = UTF8Encoding.UTF8.GetBytes(PlainText);
        byte[] resultArray = CryptoService(toEncArray);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private byte[] CryptoService(byte[] toEncArray)
    {
        MD5CryptoServiceProvider objMD5CryptoService = new();
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(settings.Password));
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            Key = securityKeyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        byte[] resultArray = objTripleDESCryptoService.CreateEncryptor().TransformFinalBlock(toEncArray, 0, toEncArray.Length);
        objTripleDESCryptoService.Clear();
        return resultArray;
    }
}