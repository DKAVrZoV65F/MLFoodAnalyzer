namespace MLFoodAnalyzerClient.Extension;

public interface IAlert
{
    void DisplayMessage(string? message);
    Task<bool> DisplayMessage(string? message1, string? message2, string? message3);
}
