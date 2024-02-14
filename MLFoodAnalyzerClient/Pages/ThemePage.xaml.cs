using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class ThemePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    private int oldValue = 20;

    public ThemePage()
    {
        InitializeComponent();

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

        int getValue = Preferences.Get("FontSize", oldValue);
        FontSizeSlider.Value = getValue;
        oldValue = getValue;
        TitleLabel.FontSize = getValue + 5;
        InfoLabel.FontSize = getValue;
        AcceptButton.FontSize = getValue;
        LightRadioButton.FontSize = getValue;
        DarkRadioButton.FontSize = getValue;
        DefaultRadioButton.FontSize = getValue;
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

    private void FontSize_Changed(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        InfoLabel.FontSize = value;
        FontSizeLabel.Text = value.ToString();
        if (value != oldValue) oldValue = value;
    }

    private async void Accept_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("FontSize", oldValue);
        await DisplayAlert(LocalizationResourceManager["AppName"].ToString(), LocalizationResourceManager["ReloadApp"].ToString(), "OK");
    }
}