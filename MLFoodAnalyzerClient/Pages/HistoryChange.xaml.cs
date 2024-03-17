using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class HistoryChange : ContentPage
{
    private ObservableCollection<History> Histories { get; set; } = new();
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private static Settings settings = AppShell.settings;
    private AlertService? alert;

    public HistoryChange()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];

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
        if (string.IsNullOrEmpty(settings.Ip) || settings.Port == 0)
        {
            alert ??= new();
            alert.DisplayMessage(LocalizationResourceManager["ErrorWithIPOrPort"].ToString());
            return;
        }

        using TcpClient tcpClient = new();
        string translation = string.Empty;
        try
        {
            await tcpClient.ConnectAsync(settings.Ip, settings.Port);
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            NetworkStream networkStream = tcpClient.GetStream();

            int bytesRead = 10;
            await stream.WriteAsync(Encoding.UTF8.GetBytes("History\0"));
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"{count}\0"));

            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            translation = Encoding.UTF8.GetString(response.ToArray());
            string[] rows = translation.Split('\n');

            foreach (string row in rows)
            {
                string[] words = row.Split('\t');
                DateTime dateTimeValue = DateTime.Now;
                DateTime.TryParseExact(words[6], "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out dateTimeValue);

                History food = new(int.Parse(words[0]), words[1], int.Parse(words[2]), words[3], words[4], words[5], dateTimeValue);
                Histories.Add(food);
            }
            response.Clear();
            networkStream.Close();
        }
        catch { }
        finally
        {
            tcpClient.Close();
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
        GetHistory(Histories.Count);
    }
}