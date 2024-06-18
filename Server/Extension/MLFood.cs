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

        KeyValuePair<string, float> firstPair = Food.PredictAllLabels(sampleData).FirstOrDefault();
        Dictionary<float, string> results = new()
        {
            { firstPair.Value, firstPair.Key },
            { 0, language }
        };

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

        KeyValuePair<string, float> firstPair = Query_EN.PredictAllLabels(sampleData).FirstOrDefault();
        Dictionary<float, string> results = new()
        {
            { firstPair.Value, firstPair.Key },
            { 0, EN }
        };

        return results;
    }

    private Dictionary<float, string> PredictFoodRu()
    {
        Query_RU.ModelInput sampleData = new()
        {
            Text = text,
        };

        KeyValuePair<string, float> firstPair = Query_RU.PredictAllLabels(sampleData).FirstOrDefault();
        Dictionary<float, string> results = new()
        {
            { firstPair.Value, firstPair.Key },
            { 0, RU }
        };

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