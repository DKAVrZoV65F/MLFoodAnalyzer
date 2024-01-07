using System.Text;

namespace MLFoodAnalyzerServer.Extension;

internal class MLFood
{
    private string text;
    private string filePath;

    public MLFood()
    {
        filePath = "";
        text = "";
    }

    public string SetImage(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath)) return "Error on server (1)";
        this.filePath = filePath;
        return PredictImage();
    }

    public string SetText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "Error on server (2)";
        this.text = text;
        return PredictText();
    }

    private string PredictImage()
    {
        // Create single instance of sample data from first line of dataset for model input
        var imageBytes = File.ReadAllBytes(filePath);
        Food.ModelInput sampleData = new()
        {
            ImageSource = imageBytes,
        };

        // Make a single prediction on the sample data and print results.
        var sortedScoresWithLabel = Food.PredictAllLabels(sampleData);
        return PredictResults(sortedScoresWithLabel);
    }

    private static string PredictResults(IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel)
    {
        StringBuilder message = new($"{"Class",-40}{"Score",-20}\n{"-----",-40}{"-----",-20}");

        foreach (var score in sortedScoresWithLabel)
        {
            if (score.Value * 100 >= 30) message.AppendLine($"{score.Key,-40}{score.Value * 100,-20}");
            else break;
        }

        return message.ToString();
    }

    private string PredictText() => text;
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