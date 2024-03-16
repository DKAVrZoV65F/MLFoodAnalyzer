using Microsoft.Data.SqlClient;

namespace MLFoodAnalyzerServer.Extension;

public class Database
{

    private readonly string connectionString = "Server=(localdb)\\Local;Database=MLF3A7;Trusted_Connection=True;";


    public Database(string connectionString = "Server=(localdb)\\Local;Database=MLF3A7;Trusted_Connection=True;") => this.connectionString = connectionString;

    public async void Info()
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        Console.WriteLine("Connection is open");
        Console.WriteLine("Connection Properties:");
        Console.WriteLine($"\tConnection string: {connection.ConnectionString}");
        Console.WriteLine($"\tThe database: {connection.Database}");
        Console.WriteLine($"\tServer: {connection.DataSource}");
        Console.WriteLine($"\tServer Version: {connection.ServerVersion}");
        Console.WriteLine($"\tState: {connection.State}");
        Console.WriteLine($"\tWorkstation Id: {connection.WorkstationId}\n");
    }

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
        return "0";
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

        if (reader.HasRows)
        {
            await reader.ReadAsync();
            object id = reader.GetValue(0);
            object name = reader.GetValue(1);
            object description = reader.GetValue(2);
            Console.WriteLine($"{id}\t{name}\t{description}");
        }

        await reader.CloseAsync();
        return "Nothing";
    }
}
