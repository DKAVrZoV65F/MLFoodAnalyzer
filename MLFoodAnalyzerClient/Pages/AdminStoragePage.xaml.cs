using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService? alert;

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

        using TcpClient tcpClient = new();
        string translation = string.Empty;
        try
        {
            await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            NetworkStream networkStream = tcpClient.GetStream();

            int bytesRead = 10;
            await stream.WriteAsync(Encoding.UTF8.GetBytes("FOOD\0"));

            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            translation = Encoding.UTF8.GetString(response.ToArray());
            string[] rows = translation.Split('\n');

            Foods.Clear();
            foreach (string row in rows)
            {
                string[] words = row.Split('\t');
                Food food = new(int.Parse(words[0]), $"{words[1][0].ToString().ToUpper()}{words[1][1..]}", words[2]);
                Foods.Add(food);
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
}
