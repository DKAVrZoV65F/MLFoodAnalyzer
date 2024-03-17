using Microsoft.Data.SqlClient;

namespace MLFoodAnalyzerServer.Extension;

public class Database
{
    private static readonly string nameDB = "MLF3A7";
    private static readonly string retUpdDesc = "success";
    private static readonly string retDBLogIn = "0";
    private readonly string connectionString = $"Server=(localdb)\\Local;Database={nameDB};Trusted_Connection=True;";

    public Database(string connectionString = "Server=(localdb)\\Local;Database=MLF3A7;Trusted_Connection=True;") => this.connectionString = connectionString;

    public string Info() => $"The database name: {nameDB}";

    public async Task<string?> DBLogIn(string login, string password)
    {
        string sqlExpression = "SELECT TOP(1) Account.Nickname FROM Account INNER JOIN AccountProperty ON AccountProperty.Id = Account.Id WHERE AccountProperty.Login = @login and AccountProperty.Password = @password";

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlParameter nameParam = new("@login", login);
        command.Parameters.Add(nameParam);

        SqlParameter ageParam = new("@password", password);
        command.Parameters.Add(ageParam);

        SqlDataReader reader = await command.ExecuteReaderAsync();

        if (reader.HasRows)
        {
            await reader.ReadAsync();
            object nickname = reader.GetValue(0);
            await reader.CloseAsync();
            return nickname?.ToString();
        }
        await reader.CloseAsync();
        return retDBLogIn;
    }


    public async Task<string?[]> SelectDescriptionFood(string[]? foodNames)
    {
        if (foodNames == null || foodNames.Length <= 0) return ["Nothing"];

        List<string?> results = [];
        foreach (string foodName in foodNames)
        {
            if (string.IsNullOrEmpty(foodName)) results.Add("Nothing");
            string sqlExpression = "select Description from Food where Name = @foodName";

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            SqlCommand command = new(sqlExpression, connection);
            SqlParameter foodNameParam = new("@foodName", foodName);
            command.Parameters.Add(foodNameParam);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                object description = reader.GetValue(0);
                await reader.CloseAsync();
                results.Add(description?.ToString());
            }
            else
                results.Add("Nothing");
            await reader.CloseAsync();
        }

        return [.. results];
    }

    public async Task<string?> FoodSelect()
    {
        string sqlExpression = "SELECT Id, Name, Description, DateUpdate FROM Food;";

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 3).Result;
        await reader.CloseAsync();
        return results;
    }

    public async Task<string?> History(int count = 0)
    {
        string sqlExpression = "SELECT IdFood, NameFood, IdAccount, NickName, Old_Description, New_Description, LastUpdate FROM History ORDER BY LastUpdate DESC OFFSET @count ROWS FETCH FIRST 10 ROWS ONLY;";

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlParameter countParam = new("@count", count);
        command.Parameters.Add(countParam);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 7).Result;
        await reader.CloseAsync();
        return results;
    }

    private async Task<string?> Result(SqlDataReader reader, int count)
    {
        string results = string.Empty;
        if (reader.HasRows)
        {
            object[] values = new object[count];
            while (await reader.ReadAsync())
            {
                for (int j = 0; j < values.Length; j++)
                {
                    values[j] = reader.GetValue(j);
                }
                results += string.Join("\t", values);
                results += '\n';
            }
        }
        return results;
    }

    public async Task<string?> UpdateDescriptionFood(string nickname, int foodId, string foodDescription)
    {
        string sqlExpression = $"DECLARE @accountId INT;" +
            $"DECLARE @Old_Description nvarchar(max);" +
            $"DECLARE @NameFood Varchar(50);" +
            $"SELECT @accountId = Id FROM Account WHERE NickName = @nickname;SELECT @Old_Description = Description FROM Food WHERE Id = @foodId;" +
            $"SELECT @NameFood = Name FROM Food WHERE Id = @foodId;UPDATE Food SET Description = @foodDescription WHERE Id = @foodId;" +
            $"Insert into History (IdFood, NameFood, IdAccount, NickName, Old_Description, New_Description, LastUpdate) " +
            $"Values(@foodId, @NameFood, @accountId, @nickname, @Old_Description, @foodDescription, CURRENT_TIMESTAMP);";

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);

        SqlParameter nicknameParam = new("@nickname", nickname);
        command.Parameters.Add(nicknameParam);
        SqlParameter foodIdParam = new("@foodId", foodId);
        command.Parameters.Add(foodIdParam);
        SqlParameter foodDescriptionParam = new("@foodDescription", foodDescription);
        command.Parameters.Add(foodDescriptionParam);

        await command.ExecuteNonQueryAsync();

        return retUpdDesc;
    }
}





/*
                  ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/