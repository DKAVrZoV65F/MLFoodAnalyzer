using Client.Extension;

namespace xU_Client;

public class UnitTest
{
    //  Test on create default classes
    //  Test on create current classes
    //  Test on create with some parameters classes
    //  Test to change parameters of class
    //  Test on function each class
    //  Test to fail classes
    //  Test to fail functions

    #region Food

    [Fact]
    public void Create_Class_Food_Default()
    {
        // Arrange
        Food food = new();

        // Assert
        Assert.NotNull(food);
    }

    [Theory]
    [InlineData(0, "DKAVrZoV65F", "DKAVrZoV65F")]
    [InlineData(1, "apple", "tester")]
    [InlineData(2, "orange", "старый")]
    [InlineData(3, "banana", "новый")]
    [InlineData(5, "5", "5")]
    public void Create_Class_Food_Current(int id, string name, string description)
    {
        // Arrange
        Food food = new(id, name, description);

        // Act
        int expectedId = food.Id;
        string expectedName = food.Name;
        string expectedDesc = food.Description;

        // Assert
        Assert.Equal(id, expectedId);
        Assert.Equal(name, expectedName);
        Assert.Equal(description, expectedDesc);
    }    
    
    [Fact]
    public void Fail_Create_Class_Food_Default()
    {
        // Arrange
        Food food = new();

        //Act
        food = null!;

        // Assert
        Assert.Null(food);
    }

    [Theory]
    [InlineData(0, "DKAVrZoV65F", "DKAVrZoV65F")]
    [InlineData(1, "apple", "tester")]
    [InlineData(2, "orange", "старый")]
    [InlineData(3, "banana", "новый")]
    [InlineData(5, "5", "5")]
    public void Fail_Create_Class_Food_Current(int id, string name, string description)
    {
        // Arrange
        Food food = new(id, name, description);

        //Act
        food = null!;

        // Assert
        Assert.Null(food);
    }
    #endregion

    #region History

    [Fact]
    public void Create_Class_History_Default()
    {
        // Arrange
        History history = new();

        // Assert
        Assert.NotNull(history);
    }

    [Theory]
    [InlineData(1, "apple", 999, "tester", "old", "new")]
    [InlineData(2, "orange", 11, "Tester", "старый", "new")]
    [InlineData(3, "banana", 10, "tester", "old", "новый")]
    [InlineData(7, "DKAVrZoV65F", 0, "DKAVrZoV65F", "DKAVrZoV65F", "DKAVrZoV65F")]
    [InlineData(5, "5", 5, "5", "5", "5")]
    public void Create_Class_History_Current(int idFood, string foodName, int idUser, string userName, string oldDescription, string newDescription)
    {
        // Arrange
        History history = new(idFood, foodName, idUser, userName, oldDescription, newDescription, DateTime.Now);

        // Act
        int resultIdFood = history.IdFood;
        string resultNameFood = history.NameFood;
        int resultIdUser = history.IdAccount;
        string resultUserName = history.NickName;
        string resultOldDesc = history.Old_Description;
        string resultNewDesc = history.New_Description;

        // Assert
        Assert.Equal(idFood, resultIdFood);
        Assert.Equal(foodName, resultNameFood);
        Assert.Equal(idUser, resultIdUser);
        Assert.Equal(userName, resultUserName);
        Assert.Equal(oldDescription, resultOldDesc);
        Assert.Equal(newDescription, resultNewDesc);
    }

    [Fact]
    public void Fail_Create_Class_History_Default()
    {
        // Arrange
        History history = new();

        //Act
        history = null!;

        // Assert
        Assert.Null(history);
    }

    [Theory]
    [InlineData(1, "apple", 999, "tester", "old", "new")]
    [InlineData(2, "orange", 11, "Tester", "старый", "new")]
    [InlineData(3, "banana", 10, "tester", "old", "новый")]
    [InlineData(7, "DKAVrZoV65F", 0, "DKAVrZoV65F", "DKAVrZoV65F", "DKAVrZoV65F")]
    [InlineData(5, "5", 5, "5", "5", "5")]
    public void Fail_Create_Class_History_Current(int idFood, string foodName, int idUser, string userName, string oldDescription, string newDescription)
    {
        // Arrange
        History history = new(idFood, foodName, idUser, userName, oldDescription, newDescription, DateTime.Now);

        //Act
        history = null!;

        // Assert
        Assert.Null(history);
    }
    #endregion
}