using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MLFoodAnalyzerServer.Extension;

public class Store(string? pathFolder = null, string? nameFile = null, string? imageFormat = null)
{
    private string nameFile = nameFile ?? "img";
    private string imageFormat = imageFormat ?? "png";
    private static readonly string[] formats = ["png", "jpeg", "jpg"];
    private string pathFolder = pathFolder ?? (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/home/{Environment.UserName}" : $"C:\\Users\\{Environment.UserName}\\SFVPicture");
    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully";

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

    public override string ToString() => $"Name: {nameFile}\nFormat: {imageFormat}\nPath: {pathFolder}";

    public string PathFolder
    {
        get => pathFolder;
        set
        {
            if (!Path.Exists(value))
            {
                Console.WriteLine(unsuccess);
                return;
            }
            pathFolder = value;
            Console.WriteLine(success);
        }
    }

    public string Format
    {
        get => imageFormat;
        set
        {
            foreach (var item in formats)
            {
                if (value.Equals(item))
                {
                    imageFormat = value;
                    return;
                }
            }
        }
    }

    public string NameFile
    {
        get => nameFile;
        set
        {
            string pattern = @"^[a-zA-Z0-9]*$";
            nameFile = Regex.IsMatch(value, pattern) ? value : nameFile;
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