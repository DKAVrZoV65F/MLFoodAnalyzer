using Server.Extension;

namespace xU_Server;

public class UnitTest
{
    //  Test on create default classes
    //  Test on create current classes
    //  Test on create with some parameters classes
    //  Test to change parameters of class
    //  Test on function each class
    //  Test to fail classes
    //  Test to fail functions

    #region AppInfo

    [Fact]
    public void Create_Class_AppInfo_Default()
    {
        // Arrange
        AppInfo appInfo = new();

        // Assert
        Assert.NotNull(appInfo);
    }

    [Fact]
    public void Function_AppInfo_ToString_OnName_Default()
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        string expected = "MLFoodServer for Windows V2.0.0";
        string result = appInfo.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Function_AppInfo_ToString_OnLength_Default()
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        int expectedLength = "MLFoodServer for Windows V2.0.0".Length;
        int result = appInfo.ToString().Length;

        // Assert
        Assert.Equal(expectedLength, result);
    }

    [Fact]
    public void Function_AppInfo_GetInfo_Default()
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        string result = appInfo.GetInfo();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Fail_Create_Class_AppInfo_Default()
    {
        // Arrange
        AppInfo appInfo = null!;

        // Assert
        Assert.Null(appInfo);
    }

    [Theory]
    [InlineData("ZDTXZFPYQEOUPGMEIYCEUSK")]
    [InlineData("PSSR34YB")]
    [InlineData("1234567890-")]
    [InlineData("V2.0.0")]
    [InlineData("MLF3A7")]
    public void Fail_Function_AppInfo_ToString_OnName_Default(string description)
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        string result = appInfo.ToString();

        // Assert
        Assert.NotEqual(description, result);
    }

    [Theory]
    [InlineData("ZDTXZFPYQEOUPGMEIYCEUSK")]
    [InlineData("PSSR34YB")]
    [InlineData("1234567890-")]
    [InlineData("V2.0.0")]
    [InlineData("MLF3A7")]
    public void Fail_Function_AppInfo_ToString_OnLength_Default(string description)
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        int expected = description.Length;
        int result = appInfo.ToString().Length;

        // Assert
        Assert.NotEqual(expected, result);
    }

    [Theory]
    [InlineData("ZDTXZFPYQEOUPGMEIYCEUSK")]
    [InlineData("PSSR34YB")]
    [InlineData("1234567890-")]
    [InlineData("MLFoodServer for Windows V2.0.0")]
    [InlineData("MLF3A7")]
    public void Fail_Function_AppInfo_GetInfo_Default(string info)
    {
        // Arrange
        AppInfo appInfo = new();

        // Act
        string result = appInfo.GetInfo();

        // Assert
        Assert.NotEqual(info, result);
    }
    #endregion

    #region Database

    [Fact]
    public void Create_Class_Database_Default()
    {
        // Arrange
        Database database = new();

        // Assert
        Assert.NotNull(database);
    }

    [Theory]
    [InlineData("TestDB")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Create_Class_Database_Current(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Assert
        Assert.NotNull(database);
    }

    [Theory]
    [InlineData("MLF3A7")]
    public void Create_Class_Database_Change(string databaseName)
    {
        // Arrange
        Database database = new()
        {
            // Act
            DatabaseName = databaseName
        };

        // Assert
        Assert.Equal(databaseName, database.DatabaseName);
    }

    [Fact]
    public void Function_Database_ToString_Default()
    {
        // Arrange
        Database database = new();

        // Act
        string expected = $"The database name: {database.DatabaseName}";
        string result = database.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("TestDB")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Function_Database_ToString_Current(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Act
        string expected = $"The database name: {databaseName}";
        string result = database.ToString();

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public async void Function_Database_Connect_Default()
    {
        // Arrange
        Database database = new();

        // Act
        string databaseName = database.DatabaseName;
        bool result = await database.Connect(databaseName);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("MLF3A7")]
    public async void Function_Database_Connect_Current(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Act
        bool result = await database.Connect(databaseName);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("SK", "LogIn", "BBBE4C423FDBB5CEF31293E58906622CE34FC9B97ACD95F43BCC04A800FEC72B", "435C554A2E9CD54D2D3431B8AF2B5D7BA740C64F1DCA92B7AF8A76B05D484EF3")]
    public async void Function_Database_ExecuteQuery_LogIn(string actual, string command, params string[] parameters)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, parameters);

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("success", "Update", "SK", "1011", "тест", "test")]
    public async void Function_Database_ExecuteQuery_Update(string actual, string command, params string[] parameters)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, parameters);

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("AllFood")]
    public async void Function_Database_ExecuteQuery_AllFood(string command)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, null!);

        // Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("History", "5")]
    public async void Function_Database_ExecuteQuery_History(string command, string length)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, length);

        // Assert
        Assert.NotNull(result);
    }


    [Theory]
    [InlineData("ru", 0.5f, "test", "тест")]
    [InlineData("en", 0.75f, "test", "test")]
    [InlineData("easdn", 1f, "test", "test")]
    public async void Function_Database_ExecuteQuery_Current(string language, float percent, string food, string description)
    {
        // Arrange
        Dictionary<float, string> parameters = new()
        {
            { percent, food },
            { 0, language }
        };
        Database database = new();

        // Act
        string actual = $"{food}|{percent * 100:f0}%|{description}\n";
        string? result = await database.ExecuteQuery(parameters);

        // Assert
        Assert.Equal(actual, result);
    }

    [Fact]
    public void Fail_Create_Class_Database_Default()
    {
        // Arrange
        Database database = null!;

        // Assert
        Assert.Null(database);
    }

    [Theory]
    [InlineData("TestDB")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Create_Class_Database_Current(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Act
        database = null!;

        // Assert
        Assert.Null(database);
    }

    [Fact]
    public void Fail_Function_Database_ToString_Default()
    {
        // Arrange
        Database database = new();

        // Act
        database = null!;
        string? result = database?.ToString() ?? null;

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("TestDB")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Function_Database_ToString_Current(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Act
        database = null!;
        string? result = database?.ToString() ?? null;

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("1ASDJhhgasudg")]
    public async void Fail_Function_Database_Connect(string databaseName)
    {
        // Arrange
        Database database = new(databaseName: databaseName);

        // Act
        bool result = await database.Connect(databaseName);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("SK", "LogIn", "test", "test")]
    public async void Fail_Function_Database_ExecuteQuery_LogIn(string actual, string command, params string[] parameters)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, parameters);

        // Assert
        Assert.NotEqual(actual, result);
    }



    [Theory]
    [InlineData("success", "WhereIShouldUpdate?", "SK", "1011", "test", "test")]
    public async void Fail_Function_Database_ExecuteQuery(string actual, string command, params string[] parameters)
    {
        // Arrange
        Database database = new();

        // Act
        string? result = await database.ExecuteQuery(command, parameters);

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("ru", 0.5f, "test", "test")]
    [InlineData("en", 0.75f, "тес1т", "312312")]
    [InlineData("easdn", 1f, "тест", "тест")]
    public async void Fail_Function_Database_ExecuteQuery_Current(string language, float percent, string food, string description)
    {
        // Arrange
        Dictionary<float, string> parameters = new()
        {
            { percent, food },
            { 0, language }
        };
        Database database = new();

        // Act
        string actual = $"{food}|{percent * 100:f0}%|{description}\n";
        string? result = await database.ExecuteQuery(parameters);

        // Assert
        Assert.NotEqual(actual, result);
    }
    #endregion

    #region Encryption

    [Fact]
    public void Create_Class_Encryption_Default()
    {
        // Arrange
        Encryption encryption = new();

        // Assert
        Assert.NotNull(encryption);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Create_Class_Encryption_Current(string privateKey)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: privateKey);

        // Assert
        Assert.NotNull(encryption);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Create_Class_Encryption_Change(string securityKey)
    {
        // Arrange
        Encryption encryption = new()
        {
            // Act
            Password = securityKey
        };
        string result = encryption.Password;

        // Assert
        Assert.Equal(securityKey, result);
    }

    [Theory]
    [InlineData("qwerty", "pSU4NVW8KaI=")]
    [InlineData("", "u9ybT9Jw4GM=")]
    [InlineData("MLF3A7", "/XaZyFvOFrk=")]
    [InlineData("qe213z", "dPA/0PqWrSY=")]
    [InlineData("WE/'.", "ebQVDRTbubE=")]
    public void Function_Encryption_EncryptText_Default(string text, string actual)
    {
        // Arrange
        Encryption encryption = new();

        // Act
        string result = encryption.EncryptText(text);

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("K", "qwerty", "TpGn7ow3PLs=")]
    [InlineData("A", "", "eH9SFjSES/w=")]
    [InlineData("M", "MLF3A7", "OW3AwNTDqv4=")]
    [InlineData("4", "qe213z", "lUm4faCJmP4=")]
    [InlineData("7", "WE/'.", "+L8FxPxF7aE=")]
    public void Function_Encryption_EncryptText_Current(string password, string text, string actual)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);

        // Act
        string result = encryption.EncryptText(text);

        // Assert
        Assert.Equal(actual, result);
    }


    [Theory]
    [InlineData("qwerty", "pSU4NVW8KaI=")]
    [InlineData("", "u9ybT9Jw4GM=")]
    [InlineData("MLF3A7", "/XaZyFvOFrk=")]
    [InlineData("qe213z", "dPA/0PqWrSY=")]
    [InlineData("WE/'.", "ebQVDRTbubE=")]
    public void Function_Encryption_DecryptText_Default(string actual, string text)
    {
        // Arrange
        Encryption encryption = new();

        // Act
        string result = encryption.DecryptText(text);

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("K", "qwerty", "TpGn7ow3PLs=")]
    [InlineData("A", "", "eH9SFjSES/w=")]
    [InlineData("M", "MLF3A7", "OW3AwNTDqv4=")]
    [InlineData("4", "qe213z", "lUm4faCJmP4=")]
    [InlineData("7", "WE/'.", "+L8FxPxF7aE=")]
    public void Function_Encryption_DecryptText_Current(string password, string actual, string text)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);

        // Act
        string result = encryption.DecryptText(text);

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("SK", "1AE55232C15B65F071015B34BA493DC4C0057CEF485968CF2193536E5A19D92A")]
    [InlineData("10", "4A44DC15364204A80FE80E9039455CC1608281820FE2B24F1E5233ADE6AF1DD5")]
    [InlineData("avrZzSYgSl", "4DE70A628BCE5333B998E6101C5B127239A9C3AF97BF788FF703A654E76CD3CC")]
    [InlineData("test", "9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08")]
    [InlineData("WE/'.", "494396E8248592A05C85DD7CB86FBC15FE71BA900E67FB670259CD5935A63158")]
    public void Function_Encryption_ConvertToHash_Current(string text, string actual)
    {
        // Act
        string result = Encryption.ConvertToHash(text);

        // Assert
        Assert.Equal(actual, result);
    }

    [Fact]
    public void Function_Encryption_ToString_Default()
    {
        // Arrange
        Encryption encryption = new();

        // Act
        string actual = "Password: QWERTY";
        string result = encryption.ToString();

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Function_Encryption_ToString_Current(string password)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);

        // Act
        string actual = $"Password: {password}";
        string result = encryption.ToString();

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Function_Encryption_ToString_Change(string password)
    {
        // Arrange
        Encryption encryption = new()
        {
            // Act
            Password = password
        };
        string actual = $"Password: {password}";
        string result = encryption.ToString();

        // Assert
        Assert.Equal(actual, result);
    }

    [Fact]
    public void Fail_Create_Class_Encryption_Default()
    {
        // Arrange
        Encryption encryption = new();

        // Act
        encryption = null!;

        // Assert
        Assert.Null(encryption);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Create_Class_Encryption_Current(string privateKey)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: privateKey);

        // Act
        encryption = null!;

        // Assert
        Assert.Null(encryption);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Create_Class_Encryption_Change(string securityKey)
    {
        // Arrange
        Encryption encryption = new()
        {
            // Act
            Password = securityKey
        };
        encryption = null!;
        string? result = encryption?.Password;

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("QWERTYU", "pSU4NVW8KaI=")]
    [InlineData("", "u9ybT9Jw4GM=123")]
    [InlineData("mlf3a7", "/XaZyFvOFrk=")]
    [InlineData("QW312Z", "dPA/0PqWrSY=")]
    [InlineData("WE/'.", "EbQVDRTbubE=")]
    public void Fail_Function_Encryption_EncryptText_Default(string text, string actual)
    {
        // Arrange
        Encryption encryption = new();

        // Act
        string? result = encryption.EncryptText(text);

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("k", "QETUO", "TpGn7ow3PLs=")]
    [InlineData("a", "", "eH9SFjSES/w=")]
    [InlineData("m", "MLF3A7", "OW3AwNTDqv4=")]
    [InlineData("4", "qe213Z", "lUm4FaCJmP4=")]
    [InlineData("7", "WE/'.", "+L8FXPxF7aE=")]
    public void Fail_Function_Encryption_EncryptText_Current(string password, string text, string actual)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);

        // Act
        string result = encryption.EncryptText(text);

        // Assert
        Assert.NotEqual(actual, result);
    }


    [Theory]
    [InlineData("QRYI", "pSU4NVW8KaI=")]
    [InlineData("M", "u9ybT9Jw4GM=")]
    [InlineData("y1312", "/XaZyFvOFrk=")]
    [InlineData("xcvb", "dPA/0PqWrSY=")]
    [InlineData("we/'.", "dPA/0PqWrSY=")]
    public void Fail_Function_Encryption_DecryptText_Default(string actual, string text)
    {
        // Arrange
        Encryption encryption = new();

        // Act
        string result = encryption.DecryptText(text);

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("K", "QRYI", "TpGn7ow3PLs=")]
    [InlineData("A", "_", "eH9SFjSES/w=")]
    [InlineData("M", "y1312", "OW3AwNTDqv4=")]
    [InlineData("4", "xcvb", "lUm4faCJmP4=")]
    [InlineData("7", "we/'.", "+L8FxPxF7aE=")]
    public void Fail_Function_Encryption_DecryptText_Current(string password, string actual, string text)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);

        // Act
        string result = encryption.DecryptText(text);

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("sk", "1AE55232C15B65F071015B34BA493DC4C0057CEF485968CF2193536E5A19D92A")]
    [InlineData("01", "4A44DC15364204A80FE80E9039455CC1608281820FE2B24F1E5233ADE6AF1DD5")]
    [InlineData("vrZzSYgSl", "4DE70A628BCE5333B998E6101C5B127239A9C3AF97BF788FF703A654E76CD3CC")]
    [InlineData("TESTER", "9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08")]
    [InlineData("we/'.", "494396E8248592A05C85DD7CB86FBC15FE71BA900E67FB670259CD5935A63158")]
    public void Fail_Function_Encryption_ConvertToHash_Current(string text, string actual)
    {
        // Act
        string result = Encryption.ConvertToHash(text);

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Fact]
    public void Fail_Function_Encryption_ToString_Default()
    {
        // Arrange
        Encryption encryption = new();
        string actual = "Password: qwerty";

        // Act
        string result = encryption.ToString();

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Function_Encryption_ToString_Current(string password)
    {
        // Arrange
        Encryption encryption = new(SecurityKey: password);
        string actual = $"Password: {password}";

        // Act
        encryption = null!;
        string? result = encryption?.ToString();

        // Assert
        Assert.NotEqual(actual, result);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData("MLF3A7")]
    [InlineData("qe213z")]
    [InlineData("WE/'.")]
    public void Fail_Function_Encryption_ToString_Change(string password)
    {
        // Arrange
        Encryption encryption = new()
        {
            // Act
            Password = password
        };
        string actual = $"Password: {password}";
        encryption = null!;
        string? result = encryption?.ToString();

        // Assert
        Assert.NotEqual(actual, result);
    }
    #endregion

    #region ImageStore

    [Fact]
    public void Create_Class_ImageStore_Default()
    {
        // Arrange
        ImageStore imageStore = new();

        // Assert
        Assert.NotNull(imageStore);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "png")]
    [InlineData("C:\\Users\\SFVPicture", "i", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "png")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jpg")]
    public void Create_Class_ImageStore_Current(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new(pathFolder: pathFolder, nameFile: nameFile, imageFormat: imageFormat);

        // Assert
        Assert.NotNull(imageStore);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "png")]
    [InlineData("C:\\Users\\SFVPicture", "i", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "png")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jpg")]
    public void Create_Class_ImageStore_Change(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            PathFolder = pathFolder,
            NameFile = nameFile,
            Format = imageFormat
        };

        // Assert
        Assert.NotNull(imageStore);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture")]
    public void Create_Class_ImageStore_Change_Path(string pathFolder)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            PathFolder = pathFolder
        };
        string result = imageStore.PathFolder;

        // Assert
        Assert.Equal(pathFolder, result);
    }

    [Theory]
    [InlineData("Image")]
    public void Create_Class_ImageStore_Change_NameFile(string nameFile)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            NameFile = nameFile
        };
        string result = imageStore.NameFile;

        // Assert
        Assert.Equal(nameFile, result);
    }

    [Theory]
    [InlineData("png")]
    [InlineData("jpeg")]
    [InlineData("jpg")]
    public void Create_Class_ImageStore_Change_ImageFormat(string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            Format = imageFormat
        };
        string result = imageStore.Format;

        // Assert
        Assert.Equal(imageFormat, result);
    }

    [Fact]
    public void Function_ImageStore_ToString_Default()
    {
        // Arrange
        ImageStore imageStore = new();

        // Act
        string result = imageStore.ToString();

        // Assert
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "png")]
    [InlineData("C:\\Users\\SFVPicture", "i", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "png")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jpg")]
    public void Function_ImageStore_ToString_Current(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new(pathFolder: pathFolder, nameFile: nameFile, imageFormat: imageFormat);

        // Act
        string actual = $"Name: {nameFile}\nFormat: {imageFormat}\nPath: {pathFolder}";
        string result = imageStore.ToString();

        // Assert
        Assert.Equal(actual, result);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "png")]
    [InlineData("C:\\Users\\SFVPicture", "i", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "png")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jpg")]
    public void Function_ImageStore_ToString_Change(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            PathFolder = pathFolder,
            NameFile = nameFile,
            Format = imageFormat
        };

        // Act

        string actual = $"Name: {nameFile}\nFormat: {imageFormat}\nPath: {pathFolder}";
        string result = imageStore.ToString();

        // Assert
        Assert.Equal(actual, result);
    }

    [Fact]
    public void Fail_Create_Class_ImageStore_Default()
    {
        // Arrange
        ImageStore imageStore = new();

        // Act
        imageStore = null!;

        // Assert
        Assert.Null(imageStore);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "png")]
    [InlineData("C:\\Users\\SFVPicture", "i", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "png")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jpg")]
    public void Fail_Create_Class_ImageStore_Current(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new(pathFolder: pathFolder, nameFile: nameFile, imageFormat: imageFormat);

        // Act
        imageStore = null!;

        // Assert
        Assert.Null(imageStore);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "pngg")]
    [InlineData("C:\\Users\\SFVPicture", "/'", "png")]
    [InlineData("C:\\Users\\SFVPicture", "4", "pnng")]
    [InlineData("C:\\Users\\SFVPicture", "12", "jjpeg")]
    [InlineData("C:\\SFVPicture", "Images", "jpg")]
    public void FailCreate_Class_ImageStore_Change(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            PathFolder = pathFolder,
            NameFile = nameFile,
            Format = imageFormat
        };

        // Act
        imageStore = null!;

        // Assert
        Assert.Null(imageStore);
    }

    [Theory]
    [InlineData("C:\\SFVPicture")]
    public void Fail_Create_Class_ImageStore_Change_Path(string pathFolder)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            PathFolder = pathFolder
        };
        string result = imageStore.PathFolder;

        // Assert
        Assert.NotEqual(pathFolder, result);
    }

    [Theory]
    [InlineData("/'/")]
    public void Fail_Create_Class_ImageStore_Change_NameFile(string nameFile)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            NameFile = nameFile
        };
        string result = imageStore.NameFile;

        // Assert
        Assert.NotEqual(nameFile, result);
    }

    [Theory]
    [InlineData("pnng")]
    [InlineData("jpegg")]
    [InlineData("jjpg")]
    public void Fail_Create_Class_ImageStore_Change_ImageFormat(string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            // Act
            Format = imageFormat
        };
        string result = imageStore.Format;

        // Assert
        Assert.NotEqual(imageFormat, result);
    }

    [Fact]
    public void Fail_Function_ImageStore_ToString_Default()
    {
        // Arrange
        ImageStore imageStore = new();

        // Act
        imageStore = null!;
        string? result = imageStore?.ToString();

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("C:\\Users\\SFVPicture", "img", "pgng")]
    [InlineData("C:\\Users\\SFVPicture", "\'", "pnng")]
    [InlineData("C:\\Users\\SFVPicture", "_", "pngg")]
    [InlineData("C:\\Users\\SFVPicture", "=", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jjpg")]
    public void Fail_Function_ImageStore_ToString_Current(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new(pathFolder: pathFolder, nameFile: nameFile, imageFormat: imageFormat);

        // Act
        imageStore = null!;
        string? result = imageStore?.ToString();

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("C:\\SFVPicture", "img", "pgng")]
    [InlineData("C:\\Users\\SFVPicture", "\'", "png")]
    [InlineData("C:\\Users\\SFVPicture", "\\", "png")]
    [InlineData("C:\\Users\\SFVPicture", "=", "jpeg")]
    [InlineData("C:\\Users\\SFVPicture", "Images", "jjpg")]
    public void Fail_Function_ImageStore_ToString_Change(string pathFolder, string nameFile, string imageFormat)
    {
        // Arrange
        ImageStore imageStore = new()
        {
            PathFolder = pathFolder,
            NameFile = nameFile,
            Format = imageFormat
        };

        // Act

        string actual = $"Name: {nameFile}\nFormat: {imageFormat}\nPath: {pathFolder}";
        string result = imageStore.ToString();

        // Assert
        Assert.NotEqual(actual, result);
    }
    #endregion
}