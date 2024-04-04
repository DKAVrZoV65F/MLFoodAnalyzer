namespace MLFoodAnalyzerServer.Extension;

public class Settings
{
    private const int version = 100;
    private const string? title = "MLFoodServer";
    private const string osPlatform = "Windows";
    private const string indent = $"\n----------\n";

    public Settings() { }


    public override string ToString() => $"{title} for {osPlatform} {version:V#'.'#'.'#}";

    public string? Title{ get; init; }

    public string GetInfo() {
        Store store = MLFoodAnalyzerServer.store ??= new();
        TCPServer tcpServer = MLFoodAnalyzerServer.server ??= new();
        Database database = MLFoodAnalyzerServer.database ??= new();
        Encryption encryption = MLFoodAnalyzerServer.encryption ??= new();

        return $"Information.\n" +
            $"\nServer Info{indent}{tcpServer}\n" +
            $"\nStore {indent}{store}\n" +
            $"\nDatabase{indent}{database}\n" +
            $"\nEncryption{indent}{encryption}\n" +
            $"\nApp info{indent}{ToString()}\n" +
            $"\nPress any button to continue.";
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