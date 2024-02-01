using MLFoodAnalyzerClient.Extension;
using System.Globalization;

namespace MLFoodAnalyzerClient.Pages;

public partial class LanguagePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public LanguagePage()
    {
        InitializeComponent();

        int getValue = Preferences.Get("FontSize", 20);
        TitleLabel.FontSize = getValue + 5;
        RussianRadioButton.FontSize = getValue;
        EnglishRadioButton.FontSize = getValue;

        string currentLanguage = Preferences.Get("LanguageApp", "ru-RU");
        switch (currentLanguage)
        {
            case "en-US":
                EnglishRadioButton.IsChecked = true;
                break;
            default:
                RussianRadioButton.IsChecked = true;
                break;
        }
    }

    void Language_Changed(object sender, CheckedChangedEventArgs e)
    {
        RadioButton selectedRadioButton = ((RadioButton)sender);
        string? checkBoxValue = (selectedRadioButton.Value != null) ? selectedRadioButton.Value.ToString() : "";
        if (string.IsNullOrEmpty(checkBoxValue)) return;

        CultureInfo cultureInfo = new(checkBoxValue);
        LocalizationResourceManager.Instance.SetCulture(cultureInfo);
        Preferences.Set("LanguageApp", checkBoxValue);
    }
}