using System.Diagnostics;
using System.Text;

namespace DotNetRun;

internal static class SqlProjectBuilder
{
    internal static async Task Build(string[] args, CancellationToken cancellationToken = default)
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
            await Process.Start(pi)!.WaitForExitAsync(cancellationToken);
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
                    "postgres" => new Package("Npgsql", ""),
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
                    Process.Start(addPackage)?.WaitForExit();
                }
            }
            else if (!line.StartsWith("--") && line.Trim() != "")
            {
                sb.AppendLine(line);
            }
        }

        var content = database switch
        {
            "sqlserver" => $"""
                        {Common.SQLSERVER.Replace("{connectionString}", connectionString.Replace("\"", "'"))
                            .Replace("{sb}", sb.ToString().Trim())};
                        """,
            "postgres" => $"""
                        {Common.POSTGRES.Replace("{connectionString}", connectionString.Replace("\"", "'"))
                            .Replace("{sb}", sb.ToString().Trim())};
                        """,
            _ => $"""
                        {Common.SQLITE.Replace("{connectionString}", connectionString.Replace("\"", "'"))
                            .Replace("{sb}", sb.ToString().Trim())};
                        """,
        };

        await File.WriteAllTextAsync(programFile, content.Trim(), cancellationToken);
        await Task.Delay(1000, cancellationToken);

        var oc = Console.ForegroundColor;
        Console.WriteLine($"Executing sql:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{sb.ToString().Trim()}");
        Console.ForegroundColor = oc;
        Console.WriteLine();

        var runProject = new ProcessStartInfo
        {
            WorkingDirectory = path,
            FileName = "dotnet",
            Arguments = $"run -v q {passArgs} --project ./{projectName}"
        };
        Process.Start(runProject)?.WaitForExit();
    }
}