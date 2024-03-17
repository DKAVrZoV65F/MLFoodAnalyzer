﻿namespace MLFoodAnalyzerServer.Extension;

public class MLFood
{
    private string text;
    private string filePath;

    public MLFood()
    {
        filePath = "";
        text = "";
    }

    public string[] SetImage(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath)) return ["Error on server (1)"];
        this.filePath = filePath;
        return PredictImage();
    }

    public string[] SetText(string text)
    {
        if (string.IsNullOrEmpty(text)) return ["Error on server (1)"];
        this.text = text.ToLower();
        return PredictText();
    }

    private string[] PredictImage()
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

    private string[] PredictText()
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

        if (rusCount > engCount) return PredictFoodRU();
        else return PredictFoodEn();
    }

    private string[] PredictFoodEn()
    {
        Query_EN.ModelInput sampleData = new()
        {
            Text = text,
        };

        var sortedScoresWithLabel = Query_EN.PredictAllLabels(sampleData);
        return PredictResults(sortedScoresWithLabel);
    }

    private string[] PredictFoodRU()
    {
        Query_RU.ModelInput sampleData = new()
        {
            Text = text,
        };

        var sortedScoresWithLabel = Query_RU.PredictAllLabels(sampleData);
        return PredictResults(sortedScoresWithLabel);
    }

    private static string[] PredictResults(IOrderedEnumerable<KeyValuePair<string, float>> sortedScoresWithLabel)
    {
        List<string> message = [];
        foreach (var score in sortedScoresWithLabel)
        {
            if (score.Value * 1000 >= 30) message.Add(score.Key);
            else break;
        }
        return [.. message];
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