namespace MLFoodAnalyzerClient.Extension;

class AlertService : IAlert
{
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    public void DisplayMessage(string? message)
    {
        Shell.Current.DisplayAlert(LocalizationResourceManager["AppName"].ToString(), message, "OK");
    }
}
