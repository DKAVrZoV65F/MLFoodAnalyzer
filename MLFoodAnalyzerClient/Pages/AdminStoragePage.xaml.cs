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

        Foods = new ObservableCollection<Food>
        {
            new Food {Name="Tom", Id=1, Description = "Q" },
            new Food {Name = "Bob", Id = 2, Description = "W"},
            new Food {Name="Sam", Id = 3, Description = "E"},
            new Food {Name = "Alice", Id = 4, Description = "R" }
        };
        
        fruitsListView.ItemsSource = Foods;
    }

    private async void FruitsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var tappedUser = e.Item as Food;
        Food user = new(tappedUser.Name, tappedUser.Id, tappedUser.Description);
        await Navigation.PushAsync(new UpdatingStoragePage(user));
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        ObservableCollection<Food> Search = new(Foods.Where(x => x.Name.Contains(SearchEntry.Text) || x.Id.ToString().Contains(SearchEntry.Text)));
        fruitsListView.ItemsSource = (Search.Count > 0) ? Search : Foods;
    }
}