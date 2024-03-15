using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class LanguagePage : ContentPage
{
    private LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private static Settings settings = AppShell.settings;
    private readonly AlertService alert = new();

    public LanguagePage()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];
        TitleLabel.FontSize = settings.FSize + 5;

        string currentLanguage = settings.Language;
        switch (currentLanguage)
        {
            case "en-US":
                EnglishRadioButton.IsChecked = true;
                break;
            case "ru-RU":
                RussianRadioButton.IsChecked = true;
                break;
            default:
                RussianRadioButton.IsChecked = true;
                break;
        }
    }

    void Language_Changed(object sender, CheckedChangedEventArgs e)
    {
        RadioButton selectedRadioButton = (RadioButton)sender;
        string? checkBoxValue = (selectedRadioButton.Value != null) ? selectedRadioButton.Value.ToString() : string.Empty;
        if (string.IsNullOrEmpty(checkBoxValue) || checkBoxValue == settings.Language) return;
        Preferences.Set("LanguageApp", checkBoxValue);

        alert.DisplayMessage(LocalizationResourceManager["ReloadApp"].ToString());
    }
}