﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MLFoodAnalyzerClient.Extension;

public class Settings : INotifyPropertyChanged
{
    private double fontSize = Preferences.Get("FontSize", 20);

    private string ip = Preferences.Get("SavedIpServer", string.Empty);
    private int port = Preferences.Get("SavedPortServer", 0);
    private string password = Preferences.Get("SavedPasswordServer", string.Empty);

    private string login = Preferences.Get("SavedLogIn", string.Empty);
    private string savedPassword = Preferences.Get("SavedPassword", string.Empty);

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

    public string Login
    {
        get => login;
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
        get => savedPassword;
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