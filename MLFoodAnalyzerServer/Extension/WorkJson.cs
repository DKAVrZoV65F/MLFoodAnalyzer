using Newtonsoft.Json;

namespace MLFoodAnalyzerServer.Extension;

public class WorkJson
{
    public static Database? database;
    public static Encryption? encryption;
    public static TCPServer? server;
    public static Store? store;


    private static string filePath = Path.GetFullPath("Settings.json");

    public string DatabaseName { get; set; } = string.Empty;    // Database

    public string SecurityKey { get; set; } = string.Empty;     // Encryption

    public string PathFolder { get; set; } = string.Empty;      // Store
    public string NameFiles { get; set; } = string.Empty;       // Store
    public string ImageFormat { get; set; } = string.Empty;     // Store
    public int Size { get; set; } = 0;                          // Store

    public int Port { get; set; } = 0;                          // TCPServer
    public int Timeout { get; set; } = 0;                       // TCPServer

    public WorkJson() { }
    public WorkJson(string DatabaseName, string SecurityKey, string PathFolder, string NameFiles, string ImageFormat, int Size, int Port, int Timeout)
    {
        this.DatabaseName = DatabaseName;
        this.SecurityKey = SecurityKey;
        this.PathFolder = PathFolder;
        this.NameFiles = NameFiles;
        this.ImageFormat = ImageFormat;
        this.Size = Size;
        this.Port = Port;
        this.Timeout = Timeout;
    }

    public void LoadJS()
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        WorkJson? deserialized = JsonConvert.DeserializeObject<WorkJson>(json);

        if (deserialized == null) return;
        database = new(deserialized.DatabaseName);
        encryption = new(deserialized.SecurityKey);
        store = new(pathFolder: deserialized.PathFolder, nameFiles: deserialized.NameFiles, imageFormat: deserialized.ImageFormat, size: deserialized.Size);
        server = new(port: deserialized.Port, timeout: deserialized.Timeout);
    }

    public void SaveJS(WorkJson workJson)
    {
        string jsonString = JsonConvert.SerializeObject(workJson, Formatting.Indented);
        if (File.Exists(filePath)) File.WriteAllText(filePath, jsonString);
    }
}
