using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminLogInPage : ContentPage
{
    private static Settings settings = AppShell.settings;
    /*private bool IsFlag = true;
    private bool IsLogIn = false;*/

    /*public static LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;*/
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
        /*if (!IsFlag) return;

        IsFlag = false;
        LogInButton.IsInProgress = true;


        if (string.IsNullOrEmpty(LoginEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            IsFlag = true;
            LogInButton.IsInProgress = false;
            return;
        }
        IsLogIn = true;


        // here if login success it will save
        if (SavingCheckBox.IsChecked && IsLogIn)
        {
            Preferences.Set("SavedLogIn", LoginEntry.Text);
            Preferences.Set("SavedPassword", PasswordEntry.Text);
        }
        else
        {
            LoginEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            Preferences.Set("SavedLogIn", string.Empty);
            Preferences.Set("SavedPassword", string.Empty);
        }
        IsFlag = true;
        LogInButton.IsInProgress = false;*/
        await Navigation.PushAsync(new AdminStoragePage());
    }
}