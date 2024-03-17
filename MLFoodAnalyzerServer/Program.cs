﻿using Newtonsoft.Json;
using MLFoodAnalyzerServer.Extension;
using System.IdentityModel.Tokens.Jwt;

namespace MLFoodAnalyzerServer;

internal class MLFoodAnalyzerServer
{
    public static Database database = new();
    public static Encryption encryption = new();
    public static Settings settings = new();
    public static TCPServer server = new();
    public static Store store = new();

    private static string filePath = "C:\\Users\\an0ni\\OneDrive\\Documentos\\file1.json";
    private static bool IsFlag = true;
    private static readonly string[] searchingIcons = [" |", " /", "--", " \\"];

    private static async Task Main()
    {
        Console.Title = settings.GetTitle();
        int selectedOption;
        do
        {
            Console.Clear();
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Run server");
            Console.WriteLine("2. Settings");
            Console.WriteLine("3. Information");
            Console.WriteLine("4. Exit");
            Console.Write("Please enter your selection: ");
            _ = int.TryParse(Console.ReadLine(), out selectedOption);
            Console.Clear();

            switch (selectedOption)
            {
                case 1:
                    Thread backgroundThread = new(new ThreadStart(LoadAll));
                    backgroundThread.Start();

                    while (IsFlag)
                    {
                        foreach (string icon in searchingIcons)
                        {
                            Console.Write($"\rLoading {icon}");
                            Thread.Sleep(100);
                        }
                    }
                    Console.Write($"\r{new String(' ', Console.BufferWidth)}");
                    Console.Clear();
                    await server.Run();
                    break;
                case 2:
                    Settings();
                    break;
                case 3:
                    Console.WriteLine("Information.");
                    Console.WriteLine(settings.GetInfo());
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
        } while (selectedOption != 4);
    }

    private static void LoadAll()
    {
        QRGenerate();
        LoadMLImage();
        LoadMLText();
        IsFlag = false;
    }

    private static void LoadMLImage()
    {
        Console.WriteLine("\rLoad ML Image.");
        MLFood? mLFood = new();
        mLFood.SetImage(Path.GetFullPath("Extension\\apple.jpg"));
    }

    private static void LoadMLText()
    {
        Console.WriteLine("\rLoad ML Text.");
        MLFood? mLFood = new();
        mLFood.SetText("apple");
    }

    private static void Settings()
    {
        int selectedOption;
        do
        {
            Console.WriteLine("Settings.");
            Console.WriteLine("Menu:");
            Console.WriteLine($"1. Port. Default = \"{server.GetPort()}\"");
            Console.WriteLine($"2. Timeout. Default = \"{server.GetTimeout()}\" ms");
            Console.WriteLine($"3. Store images. Default = \"{store.GetPath()}\"");
            Console.WriteLine($"4. Size of folder. Default = \"{store.GetSize()}\" GB");
            Console.WriteLine($"5. Name of image. Default = \"{store.GetName()}\"");
            Console.WriteLine($"6. Format image. Default = \"{store.GetFormat()}\"");
            Console.WriteLine($"7. Width image. Default = \"{store.GetWidth()}\"");
            Console.WriteLine($"8. Height image. Default = \"{store.GetHeight()}\"");
            Console.WriteLine($"9. Bits per pixel image. Default = \"{store.GetBitsPerPixel()}\"");
            Console.WriteLine("10. Exit");
            Console.Write("Please enter your selection: ");
            _ = int.TryParse(Console.ReadLine(), out selectedOption);

            switch (selectedOption)
            {
                case 1:
                    Console.WriteLine("Write new port.");
                    Console.WriteLine(server.SetPort(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 2:
                    Console.WriteLine("Write new timeout:");
                    Console.WriteLine(server.SetTimeout(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 3:
                    Console.WriteLine("\nWrite new path to store images:");
                    Console.WriteLine(store.SetPath(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 4:
                    Console.WriteLine("\nWrite new size of folder:");
                    Console.WriteLine(store.SetSize(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 5:
                    Console.WriteLine("\nWrite new name of image:");
                    Console.WriteLine(store.SetName(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 6:
                    Console.WriteLine("\nWrite new format image:");
                    Console.WriteLine(store.SetFormat(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 7:
                    Console.WriteLine("\nWrite new width image:");
                    Console.WriteLine(store.SetWidth(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 8:
                    Console.WriteLine("\nWrite new height image:");
                    Console.WriteLine(store.SetHeight(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 9:
                    Console.WriteLine("\nWrite new bits per pixel image:");
                    Console.WriteLine(store.SetBitsPerPixel(Console.ReadLine()!));
                    Console.ReadLine();
                    break;
                case 10:
                    break;
                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    Console.ReadLine();
                    break;
            }
            Console.Clear();
        } while (selectedOption != 10);
    }

    private static void QRGenerate()
    {
        Console.WriteLine("\rGenerate QRCode.");
        MessagingToolkit.QRCode.Codec.QRCodeEncoder encoder = new()
        {
            QRCodeScale = 8
        };
        System.Drawing.Bitmap bmp = encoder.Encode($"{server.GetIp()}|{server.GetPort()}");
        bmp.Save(filename: $"{store.GetPath()}\\QRServer.png");
    }


    private static void LoadJS()
    {
        /*YT? deserialized = JsonConvert.DeserializeObject<YT>(json);
        if (deserialized == null) return;
        Console.WriteLine(deserialized.Name);
        Console.WriteLine(deserialized.Channel);
        Console.WriteLine(deserialized.Active);

        Console.WriteLine();

        YY? deserialized1 = JsonConvert.DeserializeObject<YY>(json);
        if (deserialized1 == null) return;
        Console.WriteLine(deserialized1.ID);
        Console.WriteLine(deserialized1.Active2);*/
    }

    private static void SaveJS()
    {
        string jsonString = JsonConvert.SerializeObject(database, Formatting.Indented);
        if (File.Exists(filePath)) File.WriteAllText(filePath, jsonString);
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