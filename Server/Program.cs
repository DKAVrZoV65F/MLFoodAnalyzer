using QRCoder;
using Server.Extension;
using System.Net;
using System.Runtime.InteropServices;

namespace Server;

internal class Server
{
    public static AppInfo? appinfo;
    public static Database? database;
    public static Encryption? encryption;
    public static TCPServer? server;
    public static ImageStore? store;
    public static Settings settings = new();

    private static async Task Main()
    {
        string[] Menu = ["Menu:", "1. Run server", "2. Settings", "3. Information", "4. Exit", "Please enter your selection: "];
        LoadSettings();
        Console.Title = appinfo?.Title ?? string.Empty;
        int selectedOption;
        do
        {
            foreach (var item in Menu) Console.WriteLine(item);
            _ = int.TryParse(Console.ReadLine(), out selectedOption);
            Console.Clear();
            await SelectOption(selectedOption);
        } while (selectedOption != 4);
    }

    private static void LoadSettings()
    {
        bool result = settings.LoadSettings();
        if (!result) settings.SaveSettings(settings);

        database = Extension.Settings.database;
        encryption = Extension.Settings.encryption;
        server = Extension.Settings.server;
        store = Extension.Settings.store;
        appinfo ??= new();
    }

    private static async Task SelectOption(int index)
    {
        string[] searchingIcons = [" |", " /", "--", " \\"];
        switch (index)
        {
            case 1:
                Thread backgroundThread = new(new ThreadStart(LoadAll));
                backgroundThread.Start();
                while (backgroundThread.IsAlive)
                {
                    foreach (string icon in searchingIcons)
                    {
                        Console.Write($"\rLoading {icon}");
                        Thread.Sleep(100);
                    }
                }
                Console.Write($"\r{new String(' ', Console.BufferWidth)}");
                Console.Clear();
                await server!.Run();
                break;
            case 2:
                Settings(database!, encryption!, server!, store!);
                break;
            case 3:
                Console.WriteLine(appinfo?.GetInfo());
                Console.ReadLine();
                break;
            case 4:
                Console.WriteLine("Close server...");
                break;
            default:
                Console.WriteLine("Invalid selection. Please try again.");
                Console.ReadLine();
                break;
        }
        Console.Clear();
    }

    private static void Settings(Database database, Encryption encryption, TCPServer server, ImageStore store)
    {
        int selectedOption;
        do
        {
            string[] SettingsMenu = ["Settings:",
                $"1. Ip. Current = \"{server.Ip}\"",
                $"2. Port. Current = \"{server.Port}\"",
                $"3. Timeout. Current = \"{server.Timeout}\" ms",
                $"4. Name of image. Current = \"{store.NameFile}\"",
                $"5. Format image. Current = \"{store.Format}\"",
                $"6. Store images. Current = \"{store.PathFolder}\"",
                $"7. Name database. Current = \"{database.DatabaseName}\"",
                $"8. Password of encryption. Current = \"{encryption.Password}\"",
                "10. Exit", "Please enter your selection: "];
            foreach (var item in SettingsMenu) Console.WriteLine(item);
            _ = int.TryParse(Console.ReadLine(), out selectedOption);

            Console.Write("Write new ");
            switch (selectedOption)
            {
                case 1:
                    Console.Write("ip: ");
                    Console.Write(server.Ip = IPAddress.Parse(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 2:
                    Console.Write("port: ");
                    Console.Write(server.Port = int.Parse(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 3:
                    Console.Write("timeout: ");
                    Console.Write(server.Timeout = int.Parse(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 4:
                    Console.Write("name of image: ");
                    Console.Write(store.NameFile = Console.ReadLine()!);
                    Console.ReadLine();
                    break;
                case 5:
                    Console.Write("format image: ");
                    Console.Write(store.Format = Console.ReadLine()!);
                    Console.ReadLine();
                    break;
                case 6:
                    Console.Write("path to store images: ");
                    Console.Write(store.PathFolder = Console.ReadLine()!);
                    Console.ReadLine();
                    break;
                case 7:
                    Console.Write("name of database: ");
                    Console.Write(database.DatabaseName = Console.ReadLine()!);
                    Console.ReadLine();
                    break;
                case 8:
                    Console.Write("password of encryption: ");
                    Console.Write(encryption.Password = Console.ReadLine()!);
                    Console.ReadLine();
                    break;
                default:
                    break;
            }

            settings = new(DatabaseName: database.DatabaseName, Password: encryption.Password, Port: server.Port, Timeout: server.Timeout, PathFolder: store.PathFolder, ImageFormat: store.Format, NameFile: store.NameFile);
            settings.SaveSettings(settings);
            Console.Clear();
        } while (selectedOption != 10);
    }

    private static void LoadAll()
    {
        LoadSettings();
        QRGenerate();
        LoadMLImage();
        LoadMLText();
        LoadDatabase();
    }

    private static async void LoadDatabase()
    {
        Console.WriteLine("\rLoad Database.");
        database ??= new();
        if (!database.Connect(database.DatabaseName).Result)
        {
            Console.WriteLine("Fail to connect to database!\nServer close");
            return;
        }
        _ = await database.ExecuteQuery("LogIn", "TEST", "TEST");
    }

    private static void LoadMLImage()
    {
        Console.WriteLine("\rLoad ML Image.");
        MLFood? mLFood = new()
        {
            Image = Path.GetFullPath(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Extension/apple.jpg" : "Extension\\apple.jpg")
        };
        mLFood.PredictImage("ru");
    }

    private static void LoadMLText()
    {
        Console.WriteLine("\rLoad ML Text.");
        MLFood? mLFood = new()
        {
            Text = "apple"
        };
        mLFood.PredictText();
        mLFood.Text = "яблоко";
        mLFood.PredictText();
    }

    private static void QRGenerate()
    {
        Console.WriteLine("\rGenerate QRCode.");
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode($"{server?.Ip}|{server?.Port}", QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        string filePath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"{store!.PathFolder}/QRServer.png" : $"{store!.PathFolder}\\QRServer.png";
        File.WriteAllBytes(filePath, qrCodeImage);
    }
}





/*
                  ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/