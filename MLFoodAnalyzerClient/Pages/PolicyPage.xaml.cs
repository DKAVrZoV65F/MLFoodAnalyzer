using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class PolicyPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    private List<string> _allImages = [];
    private Random _random = new();
    private bool IsPolicyRead = true;

    public List<string> ImageList1 { get; set; }
    public List<string> ImageList2 { get; set; }
    public List<string> ImageList3 { get; set; }
    public List<string> ImageList4 { get; set; }

    public PolicyPage()
    {
        InitializeComponent();

        int getValue = Preferences.Get("FontSize", 20);
        PolicyLabel.FontSize = getValue;
        InformationLabel.FontSize = getValue;
        AcceptButton.FontSize = getValue;

        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        InformationLabel.IsEnabled = IsPolicyRead;
        AgreeComboBox.IsEnabled = IsPolicyRead;
        AgreeComboBox.IsChecked = !IsPolicyRead;
        AcceptButton.IsEnabled = IsPolicyRead;

        GenerateData();

        ImageList1 = Randomize(source: _allImages);
        ImageList2 = Randomize(source: _allImages);
        ImageList3 = Randomize(source: _allImages);
        ImageList4 = Randomize(source: _allImages);

        BindingContext = this;
    }

    private void GenerateData()
    {
        //All Images
        _allImages = [];

        for (var i = 1; i <= 18; i++)
        {
            _allImages.Add($"img{i:00}.jpg");
        }
    }

    private List<T> Randomize<T>(List<T> source) => [.. source.OrderBy((item) => _random.Next())];

    private void Agree_Changed(object sender, CheckedChangedEventArgs e) => AcceptButton.IsEnabled = e.Value;

    private async void Accept_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("IsPolicyRead", false);
        await Navigation.PopModalAsync();
    }

    /*await DisplayAlert(Title, LocalizationResourceManager["ErrorWithAlgorithm"].ToString(), "ОK");
        string result = await DisplayActionSheet(LocalizationResourceManager["AppInfo"].ToString(), LocalizationResourceManager["Thanks"].ToString(), "GitHub", LocalizationResourceManager["Version"].ToString() + $" {AppInfo.Current.VersionString}", LocalizationResourceManager["Language"].ToString() + $"  {currentLanguage}", LocalizationResourceManager["Author"].ToString());

        if (result == null) return;
        else if (result == "GitHub") await Clipboard.SetTextAsync("https://github.com/DKAVrZoV65F/Digital-Terrain-Models");
        else if (result.Contains(ENGLISH) || result.Contains(RUSSIAN))
        {
            var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName.
                Equals("en", StringComparison.InvariantCultureIgnoreCase) ?
                new CultureInfo("ru-RU") : new CultureInfo("en-US");

            LocalizationResourceManager.Instance.SetCulture(switchToCulture);

            currentLanguage = (currentLanguage.Equals(ENGLISH)) ? RUSSIAN : ENGLISH;
        }

        var response = await FilePicker.PickAsync();
        if (response == null) return;
        await DisplayAlert(Title, response.FullPath, "OK");*/
}