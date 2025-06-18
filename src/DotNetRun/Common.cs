namespace DotNetRun;

internal static class Common
{
    internal const string SQLITE =
    """"

    using Microsoft.Data.Sqlite;
    
    var connectionString = @"{connectionString}";    
    
    var sql = 
    """
    {sb}
    """;
    
    using var connection = new SqliteConnection(connectionString);
    try
    {
        connection.Open();
        using var command = new SqliteCommand(sql, connection);
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Command executed successfully.");
        }
        Console.WriteLine();
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        connection.Close();
    }
    """";

    internal const string SQLSERVER =
    """"

    using Microsoft.Data.SqlClient;
    
    var connectionString = @"{connectionString}";    
    
    var sql = 
    """
    {sb}
    """;
    
    using var connection = new SqlConnection(connectionString);
    try
    {
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Command executed successfully.");
        }
        Console.WriteLine();
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        connection.Close();
    }
    """";

    internal const string POSTGRES =
    """"

    using Npgsql;
    
    var connectionString = @"{connectionString}";    
    
    var sql = 
    """
    {sb}
    """;
    
    using var connection = new NpgsqlConnection(connectionString);
    try
    {
        connection.Open();
        using var command = new NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Command executed successfully.");
        }
        Console.WriteLine();
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        connection.Close();
    }
    """";

    internal static string GetTemporaryFolder()
    {
        var tempPath = Path.GetTempPath();
        var newTempFolder = "DotNetRunScriptingFolder";
        var path = Path.Combine(tempPath, newTempFolder);
        Directory.CreateDirectory(path);
        return path;
    }
}
