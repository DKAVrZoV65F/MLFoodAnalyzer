using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService? alert;
    private Connection? connection;

    public ObservableCollection<Food> Foods { get; set; } = [];

    public AdminStoragePage()
    {
        InitializeComponent();

        BindingContext = this;
        GetFruits();
        fruitsListView.ItemsSource = Foods;
    }

    private async void FruitsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Food tappedUser)
        {
            Food user = new(id: tappedUser.Id, name: tappedUser.Name, description: tappedUser.Description);
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

    private async void GetFruits()
    {
        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            alert ??= new();
            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        connection ??= new();
        string result = await connection.Food() ?? string.Empty;
        string[] rows = result.Split('\n');
        Foods.Clear();

        foreach (string row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) return;

            string[] words = row.Split('\t');
            Food food = new(int.Parse(words[0]), $"{words[1][0].ToString().ToUpper()}{words[1][1..]}", words[2] + words[3]);
            Foods.Add(food);
        }
    }
}
