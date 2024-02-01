namespace MLFoodAnalyzerClient.Pages;

public class Food
{
    public Food()
    {

    }
    public Food(string name, int age, string description)
    {
        Name = name;
        Id = age;
        Description = description;
    }

    public string Name { get; set; } = "";
    public int Id { get; set; }
    public string Description { get; set; } = "";
}