using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MLFoodAnalyzerClient.Extension;

public class Settings : INotifyPropertyChanged
{
    private double fontSize = Preferences.Get("FontSize", 20);

    private string ip = Preferences.Get("SavedIpServer", string.Empty);
    private int port = Preferences.Get("SavedPortServer", 0);
    private string? password = (string.IsNullOrWhiteSpace(SecureStorage.GetAsync("SavedPasswordServer").Result?.ToString())) ? string.Empty : SecureStorage.GetAsync("SavedPasswordServer").Result?.ToString();

    private string? login = string.IsNullOrWhiteSpace(SecureStorage.GetAsync("SavedLogIn").Result?.ToString()) ? string.Empty : SecureStorage.GetAsync("SavedLogIn").Result?.ToString();
    private string? savedPassword = string.IsNullOrWhiteSpace(SecureStorage.GetAsync("SavedPassword").Result?.ToString()) ? string.Empty : SecureStorage.GetAsync("SavedPassword").Result?.ToString();

    private string language = Preferences.Get("LanguageApp", "ru-RU");
    private string nickName = string.Empty;


    public event PropertyChangedEventHandler? PropertyChanged;

    public double FSize
    {
        get => fontSize;
        set
        {
            if (fontSize != value)
            {
                fontSize = value;
                OnPropertyChanged();
            }
        }
    }


    public string Ip
    {
        get => ip;
        set
        {
            if (ip != value)
            {
                ip = value;
                OnPropertyChanged();
            }
        }
    }

    public int Port
    {
        get => port;
        set
        {
            if (port != value)
            {
                port = value;
                OnPropertyChanged();
            }
        }
    }

    public string Password
    {
        get => password ?? string.Empty;
        set
        {
            if (password != value)
            {
                password = value;
                OnPropertyChanged();
            }
        }
    }

    public string Login
    {
        get => login ?? string.Empty;
        set
        {
            if (login != value)
            {
                login = value;
                OnPropertyChanged();
            }
        }
    }

    public string SavedPassword
    {
        get => savedPassword ?? string.Empty;
        set
        {
            if (savedPassword != value)
            {
                savedPassword = value;
                OnPropertyChanged();
            }
        }
    }

    public string Language
    {
        get => language;
        set
        {
            if (language != value)
            {
                language = value;
                OnPropertyChanged();
            }
        }
    }

    public string NickName
    {
        get => nickName;
        set
        {
            if (nickName != value)
            {
                nickName = value;
                OnPropertyChanged();
            }
        }
    }


    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
