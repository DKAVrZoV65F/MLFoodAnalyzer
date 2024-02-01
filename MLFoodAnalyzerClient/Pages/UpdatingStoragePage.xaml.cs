using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class UpdatingStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;
    private bool IsFlag = true;
	public UpdatingStoragePage(Food xyz)
	{
		InitializeComponent();

        BindingContext = this;

        int getValue = Preferences.Get("FontSize", 20);
        IDLabel.FontSize = getValue;
        NameLabel.FontSize = getValue;
        DescriptionLabel.FontSize = getValue;
        DescriptionEntry.FontSize = getValue;

        IDLabel.Text = $"ID: {xyz.Id}"; 
        NameLabel.Text = $"{LocalizationResourceManager["Title"]} {xyz.Name}";
        DescriptionEntry.Text = $"{xyz.Description}";
    }

	private async void SavingData(object sender, EventArgs e)
	{
        if (!IsFlag) return;

        IsFlag = false;
        SavingButton.IsInProgress = true;

		await Task.Delay(1000);

        IsFlag = true;
        SavingButton.IsInProgress = false;
        await Navigation.PopAsync();
    }
}