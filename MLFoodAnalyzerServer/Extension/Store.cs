using System.Text.RegularExpressions;

namespace MLFoodAnalyzerServer.Extension;

public class Store
{
    private string name;
    private string format;
    private int width;
    private int height;
    private int bitsPerPixel;
    private static readonly string[] formats = ["png", "jpeg", "jpg"];
    private string path;
    private long size;

    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully";
    private static readonly int minSizeImg = 0;
    private static readonly int maxSizeImg = 3840;
    private static readonly long maxSizeFolder = 100000;


    public Store()
    {
        name = "img";
        format = "png";
        width = 500;
        height = 500;
        path = $"C:\\Users\\{Environment.UserName}\\SFVPicture";
        size = 10;
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

    public string GetInfo() => $"Name: {name}\nFormat: {format}\nWidth x Height: {width} x {height}\nPath: {path}\nMax size of folder: {size} GB";

    public string GetPath() => path;

    public long GetSize() => size;

    public string GetName() => string.IsNullOrEmpty(name) ? "{Name of image is increment}" : name;

    public string GetFormat() => format;

    public string GetWidth() => width.ToString();

    public string GetHeight() => height.ToString();

    public string GetBitsPerPixel() => bitsPerPixel.ToString();

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
        path = validation ? pathFolder : path;
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
        this.name = validation ? name : this.name;
        return validation ? success : unsuccess;
    }

    public string SetFormat(string format)
    {
        bool validation = IsValidFormat(format);
        this.format = validation ? format : this.format;
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

    public string SetBitsPerPixel(string bitsPerPixel)
    {
        _ = int.TryParse($"{bitsPerPixel:D}", out int outputParse);
        bool validation = IsValidBitsPerPixel(outputParse);
        this.bitsPerPixel = validation ? outputParse : this.bitsPerPixel;
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

    private static bool IsValidBitsPerPixel(int input) => input == 8 || input == 16 || input == 24 || input == 32 || input == 48;

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