namespace MLFoodAnalyzerClient.Extension;

public class Food
{
    public string Name { get; init; } = "";
    public int Id { get; init; }
    public string Description { get; set; } = "";

    public Food() {}

    public Food(string name, int id, string description)
    {
        Name = name;
        Id = id;
        Description = description;
    }
}