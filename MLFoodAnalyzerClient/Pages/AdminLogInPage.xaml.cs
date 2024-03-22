using MLFoodAnalyzerClient.Extension;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminLogInPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private bool IsFlag = true;
    private AlertService? alert;

    public AdminLogInPage()
    {
        InitializeComponent();

        TitleLabel.FontSize = AppShell.settings.FSize + 5;
        AppShell.settings = (Settings)Resources["settings"];
        SavingCheckBox.IsChecked = !string.IsNullOrWhiteSpace(AppShell.settings.Login);
    }

    private void DisplayPassword_Changed(object sender, CheckedChangedEventArgs e) => PasswordEntry.IsPassword = !e.Value;

    public async void LogIn_Tapped(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            alert ??= new();
            IsFlag = true;
            LogInButton.IsInProgress = false;
            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        _ = SecureStorage.SetAsync("SavedLogIn", SavingCheckBox.IsChecked ? AppShell.settings.Login : string.Empty);
        _ = SecureStorage.SetAsync("SavedPassword", SavingCheckBox.IsChecked ? AppShell.settings.SavedPassword : string.Empty);

        if (!SavingCheckBox.IsChecked)
        {
            AppShell.settings.Login = string.Empty;
            AppShell.settings.SavedPassword = string.Empty;
        }

        IsFlag = false;
        LogInButton.IsInProgress = true;

        if (string.IsNullOrWhiteSpace(AppShell.settings.Login) || string.IsNullOrWhiteSpace(AppShell.settings.SavedPassword))
        {
            IsFlag = true;
            LogInButton.IsInProgress = false;
            return;
        }

        string encryptText = EncryptText($"{AppShell.settings.Login}|{AppShell.settings.SavedPassword}");
        using TcpClient tcpClient = new();
        string translation = string.Empty;
        try
        {
            await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            NetworkStream networkStream = tcpClient.GetStream();

            int bytesRead = 10;
            await stream.WriteAsync(Encoding.UTF8.GetBytes("LOGIN\0"));
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"{encryptText}\0"));

            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            translation = Encoding.UTF8.GetString(response.ToArray());
            translation = DecryptText(translation);
            response.Clear();
            networkStream.Close();
        }
        catch { }
        finally
        {
            tcpClient.Close();
            encryptText = string.Empty;
        }

        IsFlag = true;
        LogInButton.IsInProgress = false;
        if (translation.Equals("0") && !string.IsNullOrWhiteSpace(translation))
        {
            alert ??= new();
            alert.DisplayMessage(LocalizationResourceManager["ErrorLogIn"].ToString());
            return;
        }
        else if (string.IsNullOrWhiteSpace(translation))
        {
            alert ??= new();
            alert.DisplayMessage(LocalizationResourceManager["DestinationHostUn"].ToString());
            return;
        }
        AppShell.settings.NickName = translation;
        await Navigation.PushAsync(new AdminStoragePage());
    }

    public static string EncryptText(string plainText)
    {
        byte[] toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
        byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(AppShell.settings.Password));

        using TripleDES des = TripleDES.Create();
        des.Key = securityKeyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform objCryptoTransform = des.CreateEncryptor();
        byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
        des.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    public static string DecryptText(string CipherText)
    {
        byte[] toEncryptArray = Convert.FromBase64String(CipherText);
        byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(AppShell.settings.Password));

        using TripleDES des = TripleDES.Create();
        des.Key = securityKeyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform objCryptoTransform = des.CreateDecryptor();
        byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        des.Clear();
        return Encoding.UTF8.GetString(resultArray);
    }
}