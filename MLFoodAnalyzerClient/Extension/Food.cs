namespace MLFoodAnalyzerClient.Extension;

public class Food
{
    public int Id { get; init; }

    public string Name { get; init; } = "";

    public string Description { get; set; } = "";

    public Food() {}

    public Food(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}