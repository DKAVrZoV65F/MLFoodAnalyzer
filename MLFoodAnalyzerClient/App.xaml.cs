namespace MLFoodAnalyzerClient;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var windows = base.CreateWindow(activationState);

        const int minWidth = 700;
        const int minHeight = 870;

        windows.MinimumWidth = minWidth;
        windows.MinimumHeight = minHeight;

        return windows;
    }
}
