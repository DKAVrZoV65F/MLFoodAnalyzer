namespace MLFoodAnalyzerServer.Extension;

public class Settings
{
    private readonly int version = 100;
    private readonly string title = "MLFoodServer";
    private readonly string osPlatform = "Windows";
    private readonly string indent = $"\n----------\n";


    /*
     Logging : name of file and where to store 
     */
    //private string path;

    public Settings() 
    {
        //path = $"C:\\Users\\{Environment.UserName}\\SFVPicture";
    }

    

    public string GetTitle() => title;

    public string GetInfo() {
        TCPServer tcpServer = MLFoodAnalyzerServer.server;
        Store store = MLFoodAnalyzerServer.store;
        Database database = MLFoodAnalyzerServer.database;

        return $"\nServer Info{indent}{tcpServer.GetInfo()}{indent}" +
            $"\nStore {indent}{store.GetInfo()}{indent}" +
            $"\nDatabase{indent}{database.Info()}{indent}" +
            $"\nApp info{indent}{title} for {osPlatform} {version:V#'.'#'.'#}{indent}" +
            $"\nPress any button to continue.";
    }

    /*public string GetPath() => path;


    public string SetPath(string pathFolder) 
    {
        bool validation = IsValidPath(pathFolder);
        path = validation ? pathFolder : path;
        return validation ? "Settings applied sucessfully" : "Settings applied unsucessfully";
    }


    private static bool IsValidPath(string pathFolder) => Path.Exists(pathFolder);*/
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