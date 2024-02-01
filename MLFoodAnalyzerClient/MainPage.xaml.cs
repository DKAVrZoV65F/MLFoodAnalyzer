namespace MLFoodAnalyzerClient;

public partial class MainPage : ContentPage
{
    // public string UserName { get; set; }

    public MainPage()
    {
        InitializeComponent();

        // UserName = Preferences.Default.Get(nameof(UserName), "User");
        // TestLabel.Text = $"Hello {UserName}!";
    }

    /*private void OnCounterClicked(object sender, EventArgs e)
    {
        UserName = "Qwerty";
        Preferences.Default.Set(nameof(UserName), UserName);
        TestLabel.Text = $"Hello {UserName}!";
    }*/
}
