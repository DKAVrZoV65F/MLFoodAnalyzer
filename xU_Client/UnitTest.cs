using MLFoodAnalyzerClient.Extension;

namespace xU_Client;

public class UnitTest
{
    [Fact]
    public void TestOnCreateFood()
    {
        Food food = new();
        Assert.NotNull(food);
    }

    [Fact]
    public void TestOnCreateFoodWithParameters()
    {
        Food food = new(name: "Apple", id: 1, description: "Sweet fruit");
        Assert.NotNull(food);
        Assert.Equal("Apple", food.Name);
        Assert.Equal(1, food.Id);
        Assert.Equal("Sweet fruit", food.Description);
    }
}