using System.Text;
using Microsoft.Data.SqlClient;

namespace Server.Extension;

public class Database
{
    private const string failExecute = "Error";
    private const string retUpdDesc = "success";
    private const string retDBLogIn = "0";
    private const string defaultDBName = "MLF3A7";

    private string? databaseName = null;
    private string? connectionString = null;


    public Database() : this(defaultDBName) { }
    public Database(string databaseName)
    {
        this.databaseName = databaseName;
        connectionString = $"Server=(localdb)\\Local;Database={this.databaseName};Trusted_Connection=True;";
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
            }
        }
    }

    public async Task<bool> Connect(string database)
    {
        SqlConnection connection = new($"Server=(localdb)\\Local;Database={database};Trusted_Connection=True;");
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
                    await UpdateDescriptionFood(parameters[0], int.Parse(parameters[1]), parameters[2], parameters[3]);
                    break;
                case SQLQuery.AllFood:
                    return await AllFood();
                case SQLQuery.History:
                    return await History(int.Parse(parameters[0]));
            }
        }
        return failExecute;
    }

    public async Task<string?> ExecuteQuery(Dictionary<float, string> parameters)
    {
        return await SelectDescriptionFood(parameters);
    }

    private async Task<string?> DBLogIn(string login, string password)
    {
        SqlConnection connection = new(connectionString);
        string sqlExpression = "SELECT TOP(1) Account.Nickname FROM Account INNER JOIN AccountProperty ON AccountProperty.Id = Account.Id " +
            "WHERE AccountProperty.Login = @login and AccountProperty.Password = @password";
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

    private async Task<string?> SelectDescriptionFood(Dictionary<float, string> data)
    {
        string language = data[0];
        data.Remove(data.Keys.Last());

        if (data == null || data.Count <= 0) return ".|0%|.|.\0";

        StringBuilder results = new();
        foreach (var food in data)
        {
            if (string.IsNullOrWhiteSpace(food.Value)) results.Append($"{food.Value}|{food.Key*100:f0}%|.|.\0");

            SqlConnection connection = new(connectionString);
            string sqlExpression = language.Equals(MLFood.RU) ? "select DescriptionRu from Food where Name = @foodName" : 
                "select DescriptionEn from Food where Name = @foodName";
            await connection.OpenAsync();

            SqlCommand command = new(sqlExpression, connection);
            SqlParameter foodNameParam = new("@foodName", food.Value);
            command.Parameters.Add(foodNameParam);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                object description = reader.GetValue(0);
                await reader.CloseAsync();
                results.Append($"{food.Value}|{food.Key*100:f0}%|{description?.ToString()}\n");
            }
            else
                results.Append($"{food.Value}|{food.Key * 100:f0}%|.|.\0");
            await reader.CloseAsync();
        }

        return results.ToString();
    }

    private async Task<string?> AllFood()
    {
        SqlConnection connection = new(connectionString);
        string sqlExpression = "SELECT Id, Name, DescriptionRu, DescriptionEn, DateUpdate FROM Food;";
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 5).Result;
        await reader.CloseAsync();
        return results;
    }

    private async Task<string?> History(int count = 0)
    {
        SqlConnection connection = new(connectionString);
        string sqlExpression = "SELECT IdFood, NameFood, IdAccount, NickName, Old_DescriptionRu, Old_DescriptionEn, New_DescriptionRu, New_DescriptionEn, LastUpdate " +
            "FROM History ORDER BY LastUpdate DESC OFFSET @count ROWS FETCH FIRST 10 ROWS ONLY;";
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlParameter countParam = new("@count", count);
        command.Parameters.Add(countParam);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        string? results = Result(reader, 9).Result;
        await reader.CloseAsync();
        return results;
    }

    private async Task<string?> UpdateDescriptionFood(string nickname, int foodId, string foodDescriptionRu, string foodDescriptionEn)
    {
        string sqlExpression = $"DECLARE @accountId INT, @Old_Description nvarchar(max), @NameFood Varchar(50);" +
            $"SELECT @accountId = Id FROM Account WHERE NickName = @nickname;" +
            $"SELECT @Old_DescriptionRu = DescriptionRul, @Old_DescriptionEn = DescriptionEn, @NameFood = Name FROM Food WHERE Id = @foodId;" +
            $"UPDATE Food SET DescriptionRu = @foodDescriptionRu, DescriptionEn = @foodDescriptionEn WHERE Id = @foodId;" +
            $"Insert into History (IdFood, NameFood, IdAccount, NickName, Old_DescriptionRu, Old_DescriptionEn, New_DescriptionRu, New_DescriptionEn, LastUpdate) " +
            $"Values(@foodId, @NameFood, @accountId, @nickname, @Old_DescriptionRu, @Old_DescriptionEn, @foodDescriptionRu, @foodDescriptionEn, CURRENT_TIMESTAMP);";

        SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        SqlCommand command = new(sqlExpression, connection);
        SqlParameter nicknameParam = new("@nickname", nickname);
        SqlParameter foodIdParam = new("@foodId", foodId);
        SqlParameter foodDescriptionParamRu = new("@foodDescriptionRu", foodDescriptionRu);
        SqlParameter foodDescriptionParamEn = new("@foodDescriptionEn", foodDescriptionEn);

        command.Parameters.Add(nicknameParam);
        command.Parameters.Add(foodIdParam);
        command.Parameters.Add(foodDescriptionParamRu);
        command.Parameters.Add(foodDescriptionParamEn);

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