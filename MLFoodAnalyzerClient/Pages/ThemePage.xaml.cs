using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class ThemePage : ContentPage
{
    private LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;
    private static Settings settings = AppShell.settings;

    private AlertService? alert;

    public ThemePage()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];
        TitleLabel.FontSize = settings.FSize + 5;

        string getTheme = Preferences.Get("ThemeApp", "Default");
        switch (getTheme)
        {
            case "Light":
                LightRadioButton.IsChecked = true;
                break;
            case "Dark":
                DarkRadioButton.IsChecked = true;
                break;
            default:
                DefaultRadioButton.IsChecked = true;
                break;
        }
    }

    private void Theme_Changed(object sender, CheckedChangedEventArgs e)
    {
        RadioButton selectedRadioButton = ((RadioButton)sender);
        string? checkBoxValue = (selectedRadioButton.Value != null) ? selectedRadioButton.Value.ToString() : "";
        if (string.IsNullOrEmpty(checkBoxValue)) return;

        Application.Current!.UserAppTheme = checkBoxValue switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified,
        };
        Preferences.Set("ThemeApp", checkBoxValue);
    }

    private void Accept_Clicked(object sender, EventArgs e) {
        Preferences.Set("FontSize", Convert.ToInt32(FontSizeSlider.Value));
        alert = new();
        alert.DisplayMessage(LocalizationResourceManager["ReloadApp"].ToString());
    } 
}