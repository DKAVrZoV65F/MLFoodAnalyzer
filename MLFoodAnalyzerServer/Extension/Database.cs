using Microsoft.Data.SqlClient;

namespace MLFoodAnalyzerServer.Extension;

public class Database
{

    private string connectionString = "Server=(localdb)\\Local;Database=MLF3A7;Trusted_Connection=True;";


    public Database(string connectionString = "Server=(localdb)\\Local;Database=MLF3A7;Trusted_Connection=True;")
    {
        this.connectionString = connectionString;
    }

    public async void Info()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("Connection is open");

            // Вывод информации о подключении
            Console.WriteLine("Connection Properties:");
            Console.WriteLine($"\tConnection string: {connection.ConnectionString}");
            Console.WriteLine($"\tThe database: {connection.Database}");
            Console.WriteLine($"\tServer: {connection.DataSource}");
            Console.WriteLine($"\tServer Version: {connection.ServerVersion}");
            Console.WriteLine($"\tState: {connection.State}");
            Console.WriteLine($"\tWorkstation Id: {connection.WorkstationId}\n");
        }
    }

    public async Task<string?> DBLogIn(string login, string password)
    {
        string sqlExpression = "SELECT TOP(1) Account.Nickname FROM Account INNER JOIN AccountProperty ON AccountProperty.Id = Account.Id WHERE AccountProperty.Login = @login and AccountProperty.Password = @password";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlParameter nameParam = new SqlParameter("@login", login);
            command.Parameters.Add(nameParam);

            SqlParameter ageParam = new SqlParameter("@password", password);
            command.Parameters.Add(ageParam);

            SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();

                object id = reader.GetValue(0);
                await reader.CloseAsync();
                return id?.ToString();
            }
            await reader.CloseAsync();
            return "0";
        }
    }
}
