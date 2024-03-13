using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class PolicyPage : ContentPage
{
    private static Settings settings = AppShell.settings;
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

        settings = (Settings)Resources["settings"];
        PolicyLabel.FontSize = settings.FSize;
        InformationLabel.FontSize = settings.FSize;
        AcceptButton.FontSize = settings.FSize;

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
}