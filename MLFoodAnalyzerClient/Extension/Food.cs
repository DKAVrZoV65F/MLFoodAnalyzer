namespace MLFoodAnalyzerClient.Extension;

public class Food
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Food() {}

    public Food(int id, string? name, string description)
    {
        Id = id;
        Name = name ??= "NoResults";
        Description = description;
    }
}