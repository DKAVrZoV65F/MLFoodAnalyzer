using Client.Extension;

namespace Client.Pages;

public partial class AdminLogInPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private bool IsFlag = true;
    private AlertService? alert;
    private Connection? connection;

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

        _ = SecureStorage.SetAsync("SavedLogIn", SavingCheckBox.IsChecked ? AppShell.settings.Login : string.Empty);
        _ = SecureStorage.SetAsync("SavedPassword", SavingCheckBox.IsChecked ? AppShell.settings.SavedPassword : string.Empty);

        if (string.IsNullOrWhiteSpace(AppShell.settings.Login) || string.IsNullOrWhiteSpace(AppShell.settings.SavedPassword) 
            || string.IsNullOrWhiteSpace(AppShell.settings.Password)) return;

        IsFlag = false;
        LogInButton.IsInProgress = true;

        connection ??= new();
        string? result = await connection.LogIn();

        IsFlag = true;
        LogInButton.IsInProgress = false;
        
        if (string.IsNullOrWhiteSpace(result))
        {
            Display(LocalizationResourceManager["DestinationHostUn"].ToString());
        }
        else if (result.Equals('0'))
        {
            Display(LocalizationResourceManager["ErrorLogIn"].ToString());
        }
        else if (result.Equals(LocalizationResourceManager["ErrorConToServ"].ToString()))
        {
            Display(LocalizationResourceManager["ErrorConToServ"].ToString());
        }
        else
        {
            AppShell.settings.NickName = result;
            await Navigation.PushAsync(new AdminStoragePage());
        }    
    }

    private void Display(string? message)
    {
        alert ??= new();
        alert.DisplayMessage(message);
    }
}