namespace MLFoodAnalyzerClient.Extension;

public class Food
{
    public Food() {}
    public Food(string name, int id, string description)
    {
        Name = name;
        Id = id;
        Description = description;
    }

    public string Name { get; set; } = "";
    public int Id { get; set; }
    public string Description { get; set; } = "";
}