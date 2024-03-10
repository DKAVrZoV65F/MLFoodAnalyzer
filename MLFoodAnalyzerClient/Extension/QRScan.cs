using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MLFoodAnalyzerClient.Extension;

public class QRScan : INotifyPropertyChanged
{
    string ip = "";
    int port = 0;
    string password = "";

    public event PropertyChangedEventHandler PropertyChanged;

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
        get => password;
        set
        {
            if (password != value)
            {
                password = value;
                OnPropertyChanged();
            }
        }
    }
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
