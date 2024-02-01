using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminLogInPage : ContentPage
{
    private bool IsFlag = true;
    private bool IsLogIn = false;
    private string SavedLogIn = "";
    private string SavedPassword = "";

    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    public AdminLogInPage()
    {
        SavedLogIn = Preferences.Get("SavedLogIn", SavedLogIn);
        SavedPassword = Preferences.Get("SavedPassword", SavedPassword);

        InitializeComponent();


        int getValue = Preferences.Get("FontSize", 20);
        TitleLabel.FontSize = getValue + 5;
        LoginEntry.FontSize = getValue;
        PasswordEntry.FontSize = getValue;
        DisplayLabel.FontSize = getValue;
        SavingLabel.FontSize = getValue;

        if (!string.IsNullOrEmpty(SavedLogIn))
        {
            LoginEntry.Text = SavedLogIn;
            SavingCheckBox.IsChecked = true;
        }
        if (!string.IsNullOrEmpty(SavedPassword))
        {
            PasswordEntry.Text = SavedPassword;
            SavingCheckBox.IsChecked = true;
        }
    }

    private void DisplayPassword_Changed(object sender, CheckedChangedEventArgs e) => PasswordEntry.IsPassword = !e.Value;

    public async void LogIn_Tapped(object sender, EventArgs e)
    {
        if (!IsFlag) return;

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
            LoginEntry.Text = "";
            PasswordEntry.Text = "";
            Preferences.Set("SavedLogIn", LoginEntry.Text);
            Preferences.Set("SavedPassword", PasswordEntry.Text);
        }
        IsFlag = true;
        LogInButton.IsInProgress = false;
        await Navigation.PushAsync(new AdminStoragePage());
    }
}