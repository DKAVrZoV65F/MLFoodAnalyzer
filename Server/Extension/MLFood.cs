namespace Server.Extension;

public class MLFood
{
    private string text;
    private string filePath;
    internal const string RU = "ru";
    internal const string EN = "en";

    public MLFood()
    {
        filePath = string.Empty;
        text = string.Empty;
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
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Food.ModelInput sampleData = new()
        {
            ImageSource = imageBytes,
        };

        IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel = Food.PredictAllLabels(sampleData)
                                                                                        .Where(kvp => kvp.Value * 100 >= 30)
                                                                                        .OrderByDescending(kvp => kvp.Value * 100);
        Dictionary<float, string> results = sortedScoresWithLabel.ToDictionary(pair => pair.Value, pair => pair.Key);
        results.Add(0, RU);
        return results;
    }

    public Dictionary<float, string> PredictText()
    {
        string str = text.ToLower();
        LanguageDetect(out int engCount, out int rusCount, ref str);

        if (rusCount > engCount) return PredictFoodRu();
        else return PredictFoodEn();
    }

    private Dictionary<float, string> PredictFoodEn()
    {
        Query_EN.ModelInput sampleData = new()
        {
            Text = text,
        };

        IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel = Query_EN.PredictAllLabels(sampleData)
                                                                                        .Where(kvp => kvp.Value * 100 >= 30)
                                                                                        .OrderByDescending(kvp => kvp.Value * 100);

        Dictionary<float, string> results = sortedScoresWithLabel.ToDictionary(pair => pair.Value, pair => pair.Key);
        results.Add(0, EN);
        return results;
    }

    private Dictionary<float, string> PredictFoodRu()
    {
        Query_RU.ModelInput sampleData = new()
        {
            Text = text,
        };

        IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel = Query_RU.PredictAllLabels(sampleData)
                                                                                        .Where(kvp => kvp.Value * 100 >= 30)
                                                                                        .OrderByDescending(kvp => kvp.Value * 100);

        Dictionary<float, string> results = sortedScoresWithLabel.ToDictionary(pair => pair.Value, pair => pair.Key);
        results.Add(0, RU);
        return results;
    }

    private void LanguageDetect(out int engCount, out int rusCount, ref string text)
    {
        engCount = rusCount = 0;
        foreach (char c in text)
        {
            if (c > 'а' && c < 'я')
                rusCount++;
            else if (c > 'a' && c < 'z')
                engCount++;
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