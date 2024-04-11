using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;

namespace MLFoodAnalyzerClient.Pages;

public partial class HistoryChange : ContentPage
{
    private ObservableCollection<History> Histories { get; set; } = [];
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService? alert;
    private Connection? connection;

    public HistoryChange()
    {
        InitializeComponent();

        BindingContext = this;
        GetHistory(Histories.Count);
        historyListView.ItemsSource = Histories;
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = SearchEntry.Text;
        int searchLength = search.Length;
        ObservableCollection<History> FoodSearch = new(Histories.Where(x => string.Equals(x.NameFood, search, StringComparison.OrdinalIgnoreCase)));

        int foodSearch = FoodSearch.Count;
        historyListView.ItemsSource = (foodSearch > 0) ? FoodSearch : Histories;
        historyListView.IsVisible = (searchLength == 0 || foodSearch > 0);
        infoLabel.IsVisible = (searchLength != 0 && foodSearch == 0);

        if (Histories.Count == 0)
        {
            historyListView.IsVisible = false;
            infoLabel.IsVisible = true;
        }
    }

    private async void HistoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is History tappedHistory)
        {
            History history = new(idFood: tappedHistory.IdFood, nameFood: tappedHistory.NameFood, idAccount: tappedHistory.IdAccount, nickName: tappedHistory.NickName, oldDescription: tappedHistory.Old_Description, newDescription: tappedHistory.New_Description, lastUpdate: tappedHistory.LastUpdate);
            await Navigation.PushAsync(new DetailInformation(history));
        }
    }

    private async void GetHistory(int count)
    {
        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0)
        {
            alert ??= new();
            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        connection ??= new();
        string result = await connection.History(count) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(result))
        {
            historyListView.IsVisible = false;
            infoLabel.IsVisible = true;
            return;
        }

        string[] rows = result.Split('\n');


        foreach (string row in rows)
        {
            string[] words = row.Split('\t');
            DateTime dateTimeValue = DateTime.Now;
            DateTime.TryParseExact(words[8], "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out dateTimeValue);
            History history = new(int.Parse(words[0]), $"{words[1][0].ToString().ToUpper()}{words[1][1..]}", int.Parse(words[2]), words[3], words[4] + words[5], words[6] + words[7], dateTimeValue);
            Histories.Add(history);
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
        GetHistory(Histories.Count);
    }
}