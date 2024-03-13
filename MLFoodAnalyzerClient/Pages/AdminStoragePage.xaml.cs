using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminStoragePage : ContentPage
{
    private static Settings settings = AppShell.settings;

    public ObservableCollection<Food> Foods { get; set; }

    public AdminStoragePage()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];

        BindingContext = this;

        Foods =
        [
            new() {Name="Apple", Id=1, Description = "Q" },
            new() {Name = "Banana", Id = 2, Description = "W"},
            new() {Name="Orange", Id = 3, Description = "E"},
            new() {Name = "Cherry", Id = 4, Description = "R" }
        ];

        fruitsListView.ItemsSource = Foods;
    }

    private async void FruitsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Food tappedUser)
        {
            Food user = new(tappedUser.Name, tappedUser.Id, tappedUser.Description);
            await Navigation.PushAsync(new UpdatingStoragePage(user));
        }
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = SearchEntry.Text;
        int searchLength = search.Length;
        ObservableCollection<Food> FoodSearch = new(Foods.Where(x => string.Equals(x.Name, search, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Id.ToString(), search, StringComparison.OrdinalIgnoreCase)));

        int foodSearch = FoodSearch.Count;
        fruitsListView.ItemsSource = (foodSearch > 0) ? FoodSearch : Foods;
        fruitsListView.IsVisible = (searchLength == 0 || foodSearch > 0);
        infoLabel.IsVisible = (searchLength != 0 && foodSearch == 0);
    }

    private async void GoToHistory(object sender, EventArgs e) => await Navigation.PushAsync(new HistoryChange());
}
