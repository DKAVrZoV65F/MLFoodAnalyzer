using MLFoodAnalyzerClient.Extension;
using System.Globalization;

namespace MLFoodAnalyzerClient.Pages;

public partial class SettingsPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    private static Settings settings = AppShell.settings;
    private AlertService? alert;

    private int counter = 0;
    private bool IsFlag = false;
    private readonly string email = "gw9ckwfsp@mozmail.com";

    public SettingsPage()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];
        SettingsLabel.FontSize = settings.FSize + 5;
        HelpLabel.FontSize = settings.FSize + 5;
        AppVersionLabel.FontSize = settings.FSize - 5;

        string getLanguage = Preferences.Get("LanguageApp", "ru-RU");
        CultureInfo currentCulture = new(getLanguage);
        string currentLanguage = currentCulture.DisplayName;
        CurrentLanguageLabel.Text = $"{currentLanguage[0].ToString().ToUpper()}{currentLanguage[1..]}";

        IsFlag = Preferences.Get("IsAdminPanel", false);
#if ANDROID
        AppVersionLabel.Text = (LocalizationResourceManager["AppName"].ToString() + ' ' + LocalizationResourceManager["For"].ToString() + " Android v0.7.5");
#elif IOS
        AppVersionLabel.Text = (LocalizationResourceManager["AppName"].ToString() + ' ' + LocalizationResourceManager["For"].ToString() + " IOS v0.7.5");
#elif MACCATALYST
        AppVersionLabel.Text = (LocalizationResourceManager["AppName"].ToString() + ' ' + LocalizationResourceManager["For"].ToString() + " Maccatalyst v0.7.5");
#elif WINDOWS
        AppVersionLabel.Text = (LocalizationResourceManager["AppName"].ToString() + ' ' + LocalizationResourceManager["For"].ToString() + " Windows v0.7.5");
#endif
    }

    private async void NetworkLabel_Tapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new NetworkPage());

    private async void LanguageLabel_Tapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new LanguagePage());

    private async void ChatSettingsLabel_Tapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new ThemePage());

    private async void GitHubLabel_Tapped(object sender, TappedEventArgs e) => await Launcher.OpenAsync("https://github.com/DKAVrZoV65F/MLFoodAnalyzer");

    private async void PolicyLabel_Tapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new PolicyPage());

    private async void MailLabel_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.SetTextAsync(email);
        if (alert is null)
        {
            alert = new();
        }
        alert.DisplayMessage(LocalizationResourceManager["MailMessage"].ToString());
    }

    private void Secret_Tapped(object sender, TappedEventArgs e)
    {
        if (IsFlag) return;
        if (counter > 5)
        {
            Preferences.Set("IsAdminPanel", true);
            IsFlag = true;

            if (alert is null)
            {
                alert = new();
            }
            alert.DisplayMessage(LocalizationResourceManager["ReloadApp"].ToString());
        }
        else if (counter <= 5) counter++;
    }
}