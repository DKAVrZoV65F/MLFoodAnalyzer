namespace Client.Extension;

class AlertService : IAlert
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    public void DisplayMessage(string? message)
    {
        Shell.Current.DisplayAlert(LocalizationResourceManager["AppName"].ToString(), message, "OK");
    }

    public Task<bool> DisplayMessage(string? message1, string? message2, string? message3) => Shell.Current.DisplayAlert(LocalizationResourceManager["AppName"].ToString(), message1, message2, message3);
}
