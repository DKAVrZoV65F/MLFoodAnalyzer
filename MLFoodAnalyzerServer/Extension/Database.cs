using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace MLFoodAnalyzerServer.Extension;

public class Database
{
    private const string failExecute = "Error";
    private const string retUpdDesc = "success";
    private const string retDBLogIn = "0";
    private const string defaultDBName = "MLF3A7";

    private string? databaseName = null;
    private string? connectionString = null;
    private SqlConnection connection;

    public Database() : this(defaultDBName) { }
    public Database(string databaseName)
    {
        this.databaseName = databaseName;
        connectionString = $"Server=(localdb)\\Local;Database={this.databaseName};Trusted_Connection=True;";
        connection ??= new(connectionString);
    }

    public string DatabaseName
    {
        get => databaseName ?? defaultDBName;
        set
        {
            if (Connect(value).Result)
            {
                databaseName = value;
                connectionString = $"Server=(localdb)\\Local;Database={databaseName};Trusted_Connection=True;";
                connection = new(connectionString);
            }
        }
    }

    public async Task<bool> Connect(string database)
    {
        connection = new($"Server=(localdb)\\Local;Database={database};Trusted_Connection=True;");
        try
        {
            await connection.OpenAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override string ToString() => $"The database name: {databaseName}";

    public async Task<string?> ExecuteQuery(string command, params string[] parameters)
    {
        if (Enum.TryParse(command, true, out SQLQuery query))
        {
            switch (query)
            {
                case SQLQuery.LogIn:
                    return await DBLogIn(parameters[0], parameters[1]);
                case SQLQuery.Update:
                    await UpdateDescriptionFood(parameters[0], int.Parse(parameters[1]), parameters[2]);
                    break;
                case SQLQuery.CurrentDescription:
                    return await SelectDescriptionFood(parameters!);
                case SQLQuery.AllFood:
                    return await FoodSelect();
                case SQLQuery.History:
                    return await History(int.Parse(parameters[0]));
            }
        }
        return failExecute;
    }

    private async Task<string?> DBLogIn(string login, string password)
    {
        string sqlExpression = "SELECT TOP(1) Account.Nickname FROM Account INNER JOIN AccountProperty ON AccountProperty.Id = Account.Id WHERE AccountProperty.Login = @login and AccountProperty.Password = @password";
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

    private async Task<string?> SelectDescriptionFood(string[] message)
    {
        if (message == null || message.Length <= 0) return "Nothing\n";

        StringBuilder results = new();
        foreach (string foodName in message)
        {
            if (string.IsNullOrWhiteSpace(foodName)) results.Append("Nothing\n");
            string sqlExpression = "select Description from Food where Name = @foodName";
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
                results.Append(description?.ToString() + '\n');
            }
            else
                results.Append("Nothing\n");
            await reader.CloseAsync();
        }

        return results.ToString();
    }

    private async Task<string?> FoodSelect()
    {
        string sqlExpression = "SELECT Id, Name, Description, DateUpdate FROM Food;";
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 3).Result;
        await reader.CloseAsync();
        return results;
    }

    private async Task<string?> History(int count = 0)
    {
        string sqlExpression = "SELECT IdFood, NameFood, IdAccount, NickName, Old_Description, New_Description, LastUpdate FROM History ORDER BY LastUpdate DESC OFFSET @count ROWS FETCH FIRST 10 ROWS ONLY;";
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlParameter countParam = new("@count", count);
        command.Parameters.Add(countParam);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 7).Result;
        await reader.CloseAsync();
        return results;
    }

    private async Task<string?> UpdateDescriptionFood(string nickname, int foodId, string foodDescription)
    {
        string sqlExpression = $"DECLARE @accountId INT;" +
            $"DECLARE @Old_Description nvarchar(max);" +
            $"DECLARE @NameFood Varchar(50);" +
            $"SELECT @accountId = Id FROM Account WHERE NickName = @nickname;SELECT @Old_Description = Description FROM Food WHERE Id = @foodId;" +
            $"SELECT @NameFood = Name FROM Food WHERE Id = @foodId;UPDATE Food SET Description = @foodDescription WHERE Id = @foodId;" +
            $"Insert into History (IdFood, NameFood, IdAccount, NickName, Old_Description, New_Description, LastUpdate) " +
            $"Values(@foodId, @NameFood, @accountId, @nickname, @Old_Description, @foodDescription, CURRENT_TIMESTAMP);";

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

    enum SQLQuery
    {
        LogIn,
        Update,
        CurrentDescription,
        AllFood,
        History
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