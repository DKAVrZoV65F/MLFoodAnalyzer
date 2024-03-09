using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class HistoryChange : ContentPage
{
    public ObservableCollection<History> Histories { get; set; }
    public HistoryChange()
    {
        InitializeComponent();

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