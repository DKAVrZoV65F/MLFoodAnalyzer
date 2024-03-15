using MLFoodAnalyzerClient.Extension;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminLogInPage : ContentPage
{
    private static Settings settings = AppShell.settings;
    private bool IsFlag = true;

    public AdminLogInPage()
    {

        InitializeComponent();

        settings = (Settings)Resources["settings"];
        TitleLabel.FontSize = settings.FSize + 5;
        /*if (!string.IsNullOrEmpty(SavedLogIn))
        {
            LoginEntry.Text = SavedLogIn;
            SavingCheckBox.IsChecked = true;
        }
        if (!string.IsNullOrEmpty(SavedPassword))
        {
            PasswordEntry.Text = SavedPassword;
            SavingCheckBox.IsChecked = true;
        }*/
    }

    private void DisplayPassword_Changed(object sender, CheckedChangedEventArgs e) => PasswordEntry.IsPassword = !e.Value;

    public async void LogIn_Tapped(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        IsFlag = false;
        LogInButton.IsInProgress = true;
        string login = LoginEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            IsFlag = true;
            LogInButton.IsInProgress = false;
            return;
        }
        login = EncryptText(login);
        password = EncryptText(password);

        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(settings.Ip, settings.Port);
        var stream = tcpClient.GetStream();

        //  Buffer for incoming data
        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("LOGIN\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{login}|{password}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);

        string translation = Encoding.UTF8.GetString(response.ToArray());
        translation = DecryptText(translation);
        response.Clear();
        networkStream.Close();


        if (SavingCheckBox.IsChecked && !translation.Equals("No account found"))
        {
            Preferences.Set("SavedLogIn", login);
            Preferences.Set("SavedPassword", password);
        }
        else
        {
            Preferences.Set("SavedLogIn", string.Empty);
            Preferences.Set("SavedPassword", string.Empty);
            LoginEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
        }

        IsFlag = true;
        LogInButton.IsInProgress = false;
        await DisplayAlert("", translation, "OK");
        if(!translation.Equals("No account found")) await Navigation.PushAsync(new AdminStoragePage());
    }



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
}