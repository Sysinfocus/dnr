using System.Diagnostics;
using System.Text;

namespace DotNetRun;

internal static class SqlProjectBuilder
{
    internal static async Task Build(string[] args)
    {
        var path = Common.GetTemporaryFolder();
        var projectFile = args[0];

        var passArgs = args.Length > 1 ? string.Join(' ', args[1..]) : null;

        if (!File.Exists(projectFile))
        {
            Console.WriteLine($"File '{projectFile}' not found.");
            return;
        }
        var ext = Path.GetExtension(projectFile).ToLower();
        if (ext != ".sql")
        {
            Console.WriteLine($"File '{projectFile}' should be a valid .sql file.");
            return;
        }

        var projectName = Path.GetFileNameWithoutExtension(projectFile);
        var programFile = Path.Combine(path, projectName, "Program.cs");

        var csproj = Path.Combine(path, projectName, projectName + ".csproj");

        if (!File.Exists(csproj))
        {
            var pi = new ProcessStartInfo
            {
                WorkingDirectory = path,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "dotnet",
                Arguments = $"new console -n {projectName}"
            };
            Process.Start(pi)?.WaitForExit();
        }

        var csprojContent = File.ReadAllText(csproj);
        var lines = File.ReadAllLines(projectFile);
        HashSet<Package> packages = [];
        var sb = new StringBuilder();
        bool first = true;
        string database = string.Empty, connectionString = string.Empty;

        foreach (var line in lines)
        {
            if (first && line.StartsWith("-- "))
            {
                first = false;
                var details = line.Trim().Replace("-- ", "").Split(' ');
                database = details[0];
                connectionString = string.Join(' ', details[1..]);

                var package = database.ToLower() switch
                {
                    "sqlserver" => new Package("Microsoft.Data.SqlClient", ""),
                    "mysql" => new Package("MySql.Data", ""),
                    "mariadb" => new Package("MySqlConnector", ""),
                    "postgresql" => new Package("Npgsql", ""),
                    _ => new Package("Microsoft.Data.Sqlite", "")
                };

                if (!csprojContent.Contains($"Include=\"{package.Name}\""))
                {
                    var addPackage = new ProcessStartInfo
                    {
                        WorkingDirectory = path,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = "dotnet",
                        Arguments = $"add package {package.Name} --project ./{projectName}"
                    };
                    Process.Start(addPackage);
                }
            }
            else if (!line.StartsWith("--") && line.Trim() != "")
            {
                sb.AppendLine(line);
            }
        }

        var content = $$"""
        using Microsoft.Data.Sqlite;
        
        var connectionString = "{{connectionString}}";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var sql = @"{{sb.ToString().Trim()}}";

        using var command = new SqliteCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)} ");
            }
            Console.WriteLine();
        }

        connection.Close();
        """;

        File.WriteAllText(programFile, content.Trim());
        await Task.Delay(1000);

        Console.WriteLine($"Executing sql:\n\n{sb.ToString().Trim()}\n\n");

        var runProject = new ProcessStartInfo
        {
            WorkingDirectory = path,
            FileName = "dotnet",
            Arguments = $"run -v q {args} --project ./{projectName}"
        };
        Process.Start(runProject)?.WaitForExit();
    }
}