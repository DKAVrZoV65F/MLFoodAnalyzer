using System.Text.RegularExpressions;

namespace MLFoodAnalyzerServer.Extension;

public class Store
{
    private string nameFiles;
    private string imageFormat;
    private int width;
    private int height;
    private static readonly string[] formats = ["png", "jpeg", "jpg"];
    private string pathFolder;
    private long size;

    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully";
    private static readonly int minSizeImg = 0;
    private static readonly int maxSizeImg = 3840;
    private static readonly long maxSizeFolder = 100000;


    public Store(string? pathFolder = null, string? nameFiles = "img", string? imageFormat = "png", int size = 10)
    {
        this.nameFiles = nameFiles!;
        this.imageFormat = imageFormat!;
        this.pathFolder = (pathFolder == null) ?  $"C:\\Users\\{Environment.UserName}\\SFVPicture" : pathFolder;
        this.size = size;
        width = 500;
        height = 500;
    }

    private static void ClearDirectory(string path)
    {
        var files = Directory.EnumerateFiles(path);
        foreach (var file in files)
        {
            File.Delete(file);
        }

        var directories = Directory.EnumerateDirectories(path);
        foreach (var directory in directories)
        {
            ClearDirectory(directory);
        }
    }

    public string GetInfo() => $"Name: {nameFiles}\nFormat: {imageFormat}\nWidth x Height: {width} x {height}\nPath: {pathFolder}\nMax size of folder: {size} GB";

    public string GetPath() => pathFolder;

    public long GetSize() => size;

    public string? GetName() => string.IsNullOrWhiteSpace(nameFiles) ? "{Name of image is increment}" : nameFiles;

    public string GetFormat() => imageFormat;

    public string GetWidth() => width.ToString();

    public string GetHeight() => height.ToString();


    private static long GetDirectorySize(string path)
    {
        long sizeDirectory = 0;

        var files = Directory.EnumerateFiles(path);
        foreach (var file in files)
        {
            var info = new FileInfo(file);
            sizeDirectory += info.Length;
        }

        var directories = Directory.EnumerateDirectories(path);
        foreach (var directory in directories)
        {
            sizeDirectory += GetDirectorySize(directory);
        }

        return sizeDirectory;
    }


    public string SetPath(string pathFolder)
    {
        bool validation = IsValidPath(pathFolder) && IsValidFolderSize(pathFolder);
        pathFolder = validation ? pathFolder : pathFolder;
        return validation ? success : unsuccess;
    }

    public string SetSize(string input)
    {
        _ = long.TryParse($"{input:D}", out long newSize);
        bool validation = IsValidSize(newSize);
        size = validation ? newSize : size;
        return validation ? success : unsuccess;
    }

    public string SetName(string name)
    {
        bool validation = IsValidName(name);
        this.nameFiles = validation ? name : this.nameFiles;
        return validation ? success : unsuccess;
    }

    public string SetFormat(string format)
    {
        bool validation = IsValidFormat(format);
        this.imageFormat = validation ? format : this.imageFormat;
        return validation ? success : unsuccess;
    }

    public string SetWidth(string width)
    {
        _ = int.TryParse($"{width:D}", out int outputParse);
        bool validation = IsValidImageSize(outputParse);
        this.width = validation ? outputParse : this.width;
        return validation ? success : unsuccess;
    }

    public string SetHeight(string height)
    {
        _ = int.TryParse($"{height:D}", out int outputParse);
        bool validation = IsValidImageSize(outputParse);
        this.height = validation ? outputParse : this.height;
        return validation ? success : unsuccess;
    }


    private static bool IsValidPath(string pathFolder) => Path.Exists(pathFolder);

    private static bool IsValidSize(long input) => input < maxSizeFolder;

    private static bool IsValidName(string input)
    {
        string pattern = @"^[a-zA-Z0-9]*$";
        return Regex.IsMatch(input, pattern);
    }

    private static bool IsValidFormat(string input)
    {
        foreach (var item in formats)
        {
            if (input.Equals(item)) return true;
        }
        return false;
    }

    private static bool IsValidImageSize(int input) => input > minSizeImg && input <= maxSizeImg;

    private bool IsValidFolderSize(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                var directorySize = GetDirectorySize(folderPath);
                var totalSize = directorySize / (1024 * 1024 * 1024);

                if (totalSize > size)
                {
                    ClearDirectory(folderPath);
                    Console.WriteLine("All files have been deleted from the directory.");
                }
                else Console.WriteLine($"Total size in directory: {totalSize} GB");
            }
            else
            {
                DirectoryInfo directory = Directory.CreateDirectory(folderPath);
                Console.WriteLine("Directory created at: " + directory.FullName);
            }
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"There was an error creating the directory: {exception.Message}");
            return false;
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