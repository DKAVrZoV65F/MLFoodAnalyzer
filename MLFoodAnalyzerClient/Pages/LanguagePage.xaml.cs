﻿using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class LanguagePage : ContentPage
{
    private LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private readonly AlertService alert = new();

    public LanguagePage()
    {
        InitializeComponent();

        TitleLabel.FontSize = AppShell.settings.FSize + 5;

        string currentLanguage = AppShell.settings.Language;
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
        if (string.IsNullOrEmpty(checkBoxValue) || checkBoxValue == AppShell.settings.Language) return;
        Preferences.Set("LanguageApp", checkBoxValue);

        alert.DisplayMessage(LocalizationResourceManager["ReloadApp"].ToString());
    }
}