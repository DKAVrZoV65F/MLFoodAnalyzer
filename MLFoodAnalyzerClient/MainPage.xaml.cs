namespace MLFoodAnalyzerClient;

public partial class MainPage : ContentPage
{
    int count = 0;
    public string UserName { get; set; }

    public MainPage()
    {
        InitializeComponent();

        UserName = Preferences.Default.Get(nameof(UserName), "User");
        TestLabel.Text = $"Hello {UserName}!";
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);

        UserName = "Qwerty";
        Preferences.Default.Set(nameof(UserName), UserName);
        TestLabel.Text = $"Hello {UserName}!";
    }
}
