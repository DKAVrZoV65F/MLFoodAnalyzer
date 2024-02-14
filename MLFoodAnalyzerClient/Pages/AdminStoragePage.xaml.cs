using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public ObservableCollection<Food> Foods { get; set; }

    public AdminStoragePage()
    {
        InitializeComponent();

        BindingContext = this;

        int getValue = Preferences.Get("FontSize", 20);
        SearchEntry.FontSize = getValue;

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

        ObservableCollection<Food> Search = new(Foods.Where(x => string.Equals(x.Name, SearchEntry.Text, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Id.ToString(), SearchEntry.Text, StringComparison.OrdinalIgnoreCase)));
        fruitsListView.ItemsSource = (Search.Count > 0) ? Search : Foods;
    }
}
