using MLFoodAnalyzerServer.Extension;

namespace MLFoodAnalyzerServer;

internal class MLFoodAnalyzerServer
{
    public static Settings? settings;
    public static Database? database;
    public static Encryption? encryption;
    public static TCPServer? server;
    public static Store? store;
    public static WorkJson workJson = new();

    private static async Task Main()
    {
        string[] Menu = ["Menu:", "1. Run server", "2. Settings", "3. Information", "4. Exit", "Please enter your selection: "];
        LoadSettings();
        Console.Title = settings!.GetTitle();
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
        workJson.LoadJS();
        database = WorkJson.database ??= new();
        encryption = WorkJson.encryption ??= new();
        server = WorkJson.server ??= new();
        store = WorkJson.store ??= new();
        settings ??= new();
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
                Console.WriteLine(settings?.GetInfo());
                Console.ReadLine();
                break;
            case 4:
                Console.WriteLine("Exiting...");
                break;
            default:
                Console.WriteLine("Invalid selection. Please try again.");
                Console.ReadLine();
                break;
        }
        Console.Clear();
    }

    private static void Settings(Database database, Encryption encryption, TCPServer server, Store store)
    {
        int selectedOption;
        do
        {
            string[] SettingsMenu = ["Settings:", $"1. Port. Default = \"{server.Port}\"", $"2. Timeout. Default = \"{server.Timeout}\" ms",
                $"3. Store images. Default = \"{store.GetPath()}\"", $"4. Size of folder. Default = \"{store.GetSize()}\" GB", $"5. Name of image. Default = \"{store.GetName()}\"",
                $"6. Format image. Default = \"{store.GetFormat()}\"", $"7. Width image. Default = \"{store.GetWidth()}\"", $"8. Height image. Default = \"{store.GetHeight()}\"",
                "10. Exit", "Please enter your selection: "];
            foreach (var item in SettingsMenu) Console.WriteLine(item);
            _ = int.TryParse(Console.ReadLine(), out selectedOption);

            Console.Write("Write new ");
            switch (selectedOption)
            {
                case 1:
                    Console.Write("port: ");
                    Console.Write(server.Port = int.Parse(Console.ReadLine()!));
                    break;
                case 2:
                    Console.Write("timeout: ");
                    Console.Write(server.Timeout = int.Parse(Console.ReadLine()!));
                    break;
                case 3:
                    Console.Write("path to store images: ");
                    Console.Write(store.SetPath(Console.ReadLine()!));
                    break;
                case 4:
                    Console.Write("size of folder: ");
                    Console.Write(store.SetSize(Console.ReadLine()!));
                    break;
                case 5:
                    Console.Write("name of image: ");
                    Console.Write(store.SetName(Console.ReadLine()!));
                    break;
                case 6:
                    Console.Write("format image: ");
                    Console.Write(store.SetFormat(Console.ReadLine()!));
                    break;
                case 7:
                    Console.Write("width image: ");
                    Console.Write(store.SetWidth(Console.ReadLine()!));
                    break;
                case 8:
                    Console.Write("height image: ");
                    Console.Write(store.SetHeight(Console.ReadLine()!));
                    break;

                    // password


                case 10:
                    break;
                default:
                    break;
            }

            workJson = new(DatabaseName: "MLF3A7", SecurityKey: encryption.Password, Size: (int)store.GetSize(), Port: server.Port, Timeout: server.Timeout, PathFolder: store.GetPath(), ImageFormat: store.GetFormat(), NameFiles: store.GetName());
            workJson.SaveJS(workJson);
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
        _ = await database.ExecuteQuery("LogIn", "TEST", "TEST");
    }

    private static void LoadMLImage()
    {
        Console.WriteLine("\rLoad ML Image.");
        MLFood? mLFood = new()
        {
            Image = Path.GetFullPath("Extension\\apple.jpg")
        };
        mLFood.PredictImage();
    }

    private static void LoadMLText()
    {
        Console.WriteLine("\rLoad ML Text.");
        MLFood? mLFood = new()
        {
            Text = "apple"
        };
        mLFood.PredictText();
    }

    private static void QRGenerate()
    {
        Console.WriteLine("\rGenerate QRCode.");
        MessagingToolkit.QRCode.Codec.QRCodeEncoder encoder = new()
        {
            QRCodeScale = 8
        };
        System.Drawing.Bitmap bmp = encoder.Encode($"{server?.Ip}|{server?.Port}");
        bmp.Save(filename: $"{store!.GetPath()}\\QRServer.png");
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