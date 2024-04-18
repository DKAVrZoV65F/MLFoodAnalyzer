using System.Text.Json;

namespace Server.Extension;

public class Settings
{
    public static TCPServer server = new();
    public static ImageStore store = new();
    public static Database database = new();
    public static Encryption encryption = new();

    private readonly string filePath = Path.GetFullPath("Settings.json");

    public int Port { get; init; } = (server != null) ? server.Port : 0;                                        // TCPServer
    public int Timeout { get; init; } = (server != null) ? server.Timeout : 0;                                  // TCPServer

    public string PathFolder { get; init; } = (store != null) ? store.PathFolder : string.Empty;                // Store
    public string NameFile { get; init; } = (store != null) ? store.NameFile : string.Empty;                    // Store
    public string ImageFormat { get; init; } = (store != null) ? store.Format : string.Empty;                   // Store

    public string DatabaseName { get; init; } = (database != null) ? database.DatabaseName : string.Empty;      // Database

    public string Password { get; init; } = (encryption != null) ? encryption.Password : string.Empty;          // Encryption

    public Settings() { }
    public Settings(string DatabaseName, string Password, string PathFolder, string NameFile, string ImageFormat, int Port, int Timeout)
    {
        this.DatabaseName = DatabaseName;
        this.Password = Password;
        this.PathFolder = PathFolder;
        this.NameFile = NameFile;
        this.ImageFormat = ImageFormat;
        this.Port = Port;
        this.Timeout = Timeout;
    }

    public bool LoadSettings()
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json)) return false;

        Settings? deserialized = JsonSerializer.Deserialize<Settings>(json);

        if (deserialized == null) return false;
        database.DatabaseName = deserialized.DatabaseName;
        encryption.Password = deserialized.Password;
        store.PathFolder = deserialized.PathFolder;
        store.NameFile = deserialized.NameFile;
        store.Format = deserialized.ImageFormat;
        server.Port = deserialized.Port;
        server.Timeout = deserialized.Timeout;
        return true;
    }

    public void SaveSettings(Settings workJson)
    {
        JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        JsonSerializerOptions options = jsonSerializerOptions;
        string jsonString = JsonSerializer.Serialize(workJson, options);
        if (File.Exists(filePath)) File.WriteAllText(filePath, jsonString);
        else
        {
            File.Create(filePath);
            File.WriteAllText(filePath, jsonString);
        }
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