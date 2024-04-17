using System.Text.Json;

namespace Server.Extension;

public class Settings
{
    public static Database? database;
    public static Encryption? encryption;
    public static TCPServer? server;
    public static ImageStore? store;


    private readonly string filePath = Path.GetFullPath("Settings.json");

    public string DatabaseName { get; init; } = string.Empty;    // Database

    public string SecurityKey { get; init; } = string.Empty;     // Encryption

    public string PathFolder { get; init; } = string.Empty;      // Store
    public string NameFile { get; init; } = string.Empty;        // Store
    public string ImageFormat { get; init; } = string.Empty;     // Store

    public int Port { get; init; } = 0;                          // TCPServer
    public int Timeout { get; init; } = 0;                       // TCPServer

    public Settings() { }
    public Settings(string DatabaseName, string SecurityKey, string PathFolder, string NameFile, string ImageFormat, int Port, int Timeout)
    {
        this.DatabaseName = DatabaseName;
        this.SecurityKey = SecurityKey;
        this.PathFolder = PathFolder;
        this.NameFile = NameFile;
        this.ImageFormat = ImageFormat;
        this.Port = Port;
        this.Timeout = Timeout;
    }

    public void LoadJS()
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        Settings? deserialized = JsonSerializer.Deserialize<Settings>(json);

        if (deserialized == null) return;
        database = new(deserialized.DatabaseName);
        encryption = new(deserialized.SecurityKey);
        store = new(pathFolder: deserialized.PathFolder, nameFile: deserialized.NameFile, imageFormat: deserialized.ImageFormat);
        server = new(port: deserialized.Port, timeout: deserialized.Timeout);
    }

    public void SaveJS(Settings workJson, string filePath = "Settings.json")
    {
        JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        JsonSerializerOptions options = jsonSerializerOptions;
        string jsonString = JsonSerializer.Serialize(workJson, options);
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