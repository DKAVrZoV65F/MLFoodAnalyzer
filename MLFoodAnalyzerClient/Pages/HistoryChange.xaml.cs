using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class HistoryChange : ContentPage
{
    private ObservableCollection<History> Histories { get; set; }
    private static Settings settings = AppShell.settings;

    public HistoryChange()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];

        BindingContext = this;

        Histories =
        [
            new() {Date="Apple", HistoryChange = "Q" },
            new() {Date = "Banana", HistoryChange = "W"},
            new() {Date="Orange", HistoryChange = "E"},
            new() {Date = "Cherry", HistoryChange = "R" }
        ];

        historyListView.ItemsSource = Histories;
    }
}