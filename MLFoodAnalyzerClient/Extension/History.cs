namespace MLFoodAnalyzerClient.Extension;

public class History
{
    public string Date { get; init; } = "";
    public string HistoryChange { get; init; } = "";
    public History() { }

    public History(string date, string historyChange)
    {
        Date = date;
        HistoryChange = historyChange;
    }
}
