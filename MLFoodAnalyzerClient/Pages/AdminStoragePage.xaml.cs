using MLFoodAnalyzerClient.Extension;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace MLFoodAnalyzerClient.Pages;

public partial class AdminStoragePage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private static Settings settings = AppShell.settings;
    private AlertService? alert;

    public ObservableCollection<Food> Foods { get; set; } = new();

    public AdminStoragePage()
    {
        InitializeComponent();

        settings = (Settings)Resources["settings"];

        BindingContext = this;
        //GetFruits();
        /*Foods =
        [
            new() {Name="Apple", Id=1, Description = "Q" },
            new() {Name = "Banana", Id = 2, Description = "W"},
            new() {Name="Orange", Id = 3, Description = "E"},
            new() {Name = "Cherry", Id = 4, Description = "R" }
        ];*/

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

    private async void GetFruits()
    {
        if (string.IsNullOrEmpty(settings.Ip) || settings.Port == 0)
        {
            if (alert == null) alert = new();
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
            await stream.WriteAsync(Encoding.UTF8.GetBytes("FOOD\0"));

            while ((bytesRead = stream.ReadByte()) != '\0')
                response.Add((byte)bytesRead);

            translation = Encoding.UTF8.GetString(response.ToArray());

            string[] rows = translation.Split('\n');

            Foods.Clear();
            foreach (string row in rows)
            {
                Food? food = CreateMyClassInstance(row.Split('\t'));
                if (food != null) Foods.Add(food);
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

    private static Food? CreateMyClassInstance(string[] values)
    {
        string strClassName = nameof(Food);
        Type? calledType = Type.GetType(strClassName);
        object? myClassInstance = Activator.CreateInstance(calledType!);

        PropertyInfo[]? propertyInf = calledType?.GetProperties();
        for (int i = 0; i < propertyInf?.Length; i++)
        {
            PropertyInfo propertyInfo = propertyInf[i];
            if (propertyInfo.CanWrite)
            {
                switch (propertyInfo.Name)
                {
                    case "Id":
                        propertyInfo.SetValue(myClassInstance, int.Parse(values[i]));
                        break;
                    case "Name":
                        propertyInfo.SetValue(myClassInstance, values[i]);
                        break;
                    case "Description":
                        propertyInfo.SetValue(myClassInstance, values[i]);
                        break;
                }
            }
        }

        return (Food?)myClassInstance;
    }
}
