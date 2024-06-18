using Client.Extension;
using System.Collections.ObjectModel;

namespace Client.Pages;

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
        foodListView.ItemsSource = Foods;

        MessagingCenter.Subscribe<UpdatingStoragePage>(this, "Update", (sender) =>
        {
            Button_Update(null!, null!);
        });
    }

    private async void FoodListView_ItemTapped(object sender, ItemTappedEventArgs e)
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
        foodListView.ItemsSource = (foodSearch > 0) ? FoodSearch : Foods;
        foodListView.IsVisible = (searchLength == 0 || foodSearch > 0);
        infoLabel.IsVisible = (searchLength != 0 && foodSearch == 0);

        if (Foods.Count == 0)
        {
            foodListView.IsVisible = false;
            infoLabel.IsVisible = true;
        }
    }

    private async void GoToHistory(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoryChange());
    }

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
        if (string.IsNullOrWhiteSpace(result))
        {
            foodListView.IsVisible = false;
            infoLabel.IsVisible = true;
            return;
        }


        string[] rows = result.Split('\n');
        Foods.Clear();

        foreach (string row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] words = row.Split('\t');
            Food food = new(int.Parse(words[0]), LocalizationResourceManager[words[1]].ToString(), words[2] + "\n\n" + words[3]);
            Foods.Add(food);
        }
    }

    private void Button_Update(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
        Foods.Clear();
        GetFruits();
    }
}
