using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class UpdatingStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private bool IsFlag = true;
    private AlertService? alert;
    private Connection? connection;
    private readonly Food foodDetail;

    public UpdatingStoragePage(Food food)
    {
        InitializeComponent();

        BindingContext = this;

        foodDetail = food;
        IDLabel.Text = $"ID: {foodDetail.Id}";
        NameLabel.Text = $"{LocalizationResourceManager["Title"]} {foodDetail.Name[0].ToString().ToUpper()}{foodDetail.Name[1..]}";
        DescriptionEntry.Text = $"{foodDetail.Description}";
    }

    private async void SavingData(object sender, EventArgs e)
    {
        if (!IsFlag) return;

        IsFlag = false;
        SavingButton.IsInProgress = true;

        connection ??= new();
        string result = await connection.Update($"{AppShell.settings.NickName}|{foodDetail.Id}|{DescriptionEntry.Text}") ?? string.Empty;

        IsFlag = true;
        SavingButton.IsInProgress = false;
        alert ??= new();
        alert.DisplayMessage(result.Equals("success") ? LocalizationResourceManager["SuccessUpdateDescr"].ToString() : (result.Equals("error") ? LocalizationResourceManager["ErrorUpdateDescr"].ToString() : LocalizationResourceManager["ErrorWithIPOrPort"].ToString()));
        if (result.Equals("success")) await Navigation.PopAsync();
    }
}