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

    [Fact]
    public void TestOnCreateFoodWithoutParameters()
    {
        Food food = new();
        Assert.NotNull(food);
        Assert.Equal("", food.Name);
        Assert.Equal(0, food.Id);
        Assert.Equal("", food.Description);
    }

    [Fact]
    public void TestChnageFoodParametr()
    {
        Food food = new(name: "Apple", id: 1, description: "Sweet fruit");
        Assert.NotNull(food);

        food.Description = "Sweet fruit and tasty";

        Assert.Equal("Apple", food.Name);
        Assert.Equal(1, food.Id);
        Assert.Equal("Sweet fruit and tasty", food.Description);
    }

    [Fact]
    public void TestChnageFoodParametrs()
    {
        Food food = new(name: "Apple", id: 1, description: "Sweet fruit");
        Assert.NotNull(food);

        Assert.Equal("Apple", food.Name);
        Assert.Equal(1, food.Id);
        Assert.Equal("Sweet fruit and tasty", food.Description);

        food = new(name: "Apple_2", id: 1, description: "Sweet fruit and tasty");
        Assert.NotNull(food);

        Assert.Equal("Apple_2", food.Name);
        Assert.Equal(1, food.Id);
        Assert.Equal("Sweet fruit and tasty", food.Description);
    }
}