namespace MLFoodAnalyzerServer.Extension;

public class MLFood
{
    private string text;
    private string filePath;
    internal const string RU = "ru";
    internal const string EN = "en";

    public MLFood()
    {
        filePath = "";
        text = "";
    }

    public string Text
    {
        get => text; 
        set 
        {
            if (!string.IsNullOrWhiteSpace(value)) text = value.ToLower();
        }
    }

    public string Image
    {
        get => filePath;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && File.Exists(value)) filePath = value;
        }
    }

    public Dictionary<float, string> PredictImage(string language)
    {
        var imageBytes = File.ReadAllBytes(filePath);
        Food.ModelInput sampleData = new()
        {
            ImageSource = imageBytes,
        };

        var sortedScoresWithLabel = Food.PredictAllLabels(sampleData);
        return PredictResults(language, sortedScoresWithLabel);
    }

    public Dictionary<float, string> PredictText()
    {
        string str = text;
        int engCount = 0;
        int rusCount = 0;
        foreach (char c in str)
        {
            if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                rusCount++;
            else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                engCount++;
        }

        if (rusCount > engCount) return PredictFoodRu();
        else return PredictFoodEn();
    }

    private Dictionary<float, string> PredictFoodEn()
    {
        Query_EN.ModelInput sampleData = new()
        {
            Text = text,
        };

        var sortedScoresWithLabel = Query_EN.PredictAllLabels(sampleData);
        return PredictResults(EN, sortedScoresWithLabel);
    }

    private Dictionary<float, string> PredictFoodRu()
    {
        Query_RU.ModelInput sampleData = new()
        {
            Text = text,
        };

        var sortedScoresWithLabel = Query_RU.PredictAllLabels(sampleData);
        return PredictResults(RU, sortedScoresWithLabel);
    }

    private static Dictionary<float, string> PredictResults(string language, IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel)
    {
        Dictionary<float, string> message = [];
        message.Add(0, language);
        foreach (var score in sortedScoresWithLabel)
        {
            if (score.Value * 100 >= 30) message.Add(score.Value, score.Key);
            else break;
        }
        return message;
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