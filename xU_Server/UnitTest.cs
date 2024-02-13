using MLFoodAnalyzerServer.Extension;

namespace xU_Server;

public class UnitTest
{
    [Fact]
    public void TestOnCreateMLFood()
    {
        MLFood mLFood = new();
        Assert.NotNull(mLFood);
    }

    [Fact]
    public void TestOnCreateSettings()
    {
        Settings settings = new();
        Assert.NotNull(settings);
    }

    [Fact]
    public void TestOnCreateStore()
    {
        Store store = new();
        Assert.NotNull(store);
    }

    [Fact]
    public void TestOnCreateTCPServer()
    {
        TCPServer tCPServer = new();
        Assert.NotNull(tCPServer);
    }
}