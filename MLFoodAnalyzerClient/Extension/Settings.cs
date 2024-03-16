﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MLFoodAnalyzerClient.Extension;

public class Settings : INotifyPropertyChanged
{
    private double fontSize = Preferences.Get("FontSize", 20);

    private string ip = Preferences.Get("SavedIpServer", string.Empty);
    private int port = Preferences.Get("SavedPortServer", 0);
    private string? password = (string.IsNullOrEmpty(SecureStorage.GetAsync("SavedPasswordServer").Result?.ToString())) ? "" : SecureStorage.GetAsync("SavedPasswordServer").Result?.ToString();

    private string? login = (string.IsNullOrEmpty(SecureStorage.GetAsync("SavedLogIn").Result?.ToString())) ? "" : SecureStorage.GetAsync("SavedLogIn").Result?.ToString();
    private string? savedPassword = (string.IsNullOrEmpty(SecureStorage.GetAsync("SavedPassword").Result?.ToString())) ? "" : SecureStorage.GetAsync("SavedPassword").Result?.ToString();

    private string language = Preferences.Get("LanguageApp", "ru-RU");


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
        get => password == null ? "" : password;
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
        get => login == null ? "" : login;
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
        get => savedPassword == null ? "" : savedPassword;
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


    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
