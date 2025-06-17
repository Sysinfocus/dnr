using System.Diagnostics;
using System.Text;

if (args.Length != 1) return;

var temporaryFolder = GetTemporaryFolder();

var file = args[0];
if (!File.Exists(file))
{
    Console.WriteLine($"File '{file}' not found.");
    return;
}
var ext = Path.GetExtension(file).ToLower();
if (ext != ".cs")
{
    Console.WriteLine($"File '{file}' should be a valid C# file.");
    return;
}

ReadCSFile(temporaryFolder, args[0]);

Console.WriteLine();


static string GetTemporaryFolder()
{
    var tempPath = Path.GetTempPath();
    var newTempFolder = "DotNetRunScriptingFolder";
    var path = Path.Combine(tempPath, newTempFolder);    
    Directory.CreateDirectory(path);
    return path;
}

static void ReadCSFile(string path, string projectFile)
{    
    var projectName = Path.GetFileNameWithoutExtension(projectFile);
    var programFile = Path.Combine(path, projectName, "Program.cs");

    var csproj = Path.Combine(path, projectName, projectName + ".csproj");
    var csprojContent = File.ReadAllText(csproj);    

    if (!File.Exists(csproj))
    {
        var pi = new ProcessStartInfo
        {
            WorkingDirectory = path,
            UseShellExecute = false,
            CreateNoWindow = true,
            FileName = "dotnet",
            Arguments = $"new console -n {projectName}"
        };
        Process.Start(pi);
    }

    var lines = File.ReadAllLines(projectFile);
    HashSet<Package> packages = [];
    var sb = new StringBuilder();
    foreach (var line in lines)
    {
        if (line.StartsWith("#:"))
        {
            var package = line.Trim().Replace("#:package ", "").Split('@');
            packages.Add(new(package[0], package[1]));

            if (!csprojContent.Contains($"Include=\"{package[0]}\"") &&
                !csprojContent.Contains($"Version=\"{package[1]}\""))
            {
                var addPackage = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = "dotnet",
                    Arguments = $"add package {package} --project ./{projectName}"
                };
                Process.Start(addPackage);
            }
        }
        else
        {
            sb.AppendLine(line);
        }
    }

    File.WriteAllText(programFile, sb.ToString());
    var pix = new ProcessStartInfo
    {
        WorkingDirectory = path,
        FileName = "dotnet",
        Arguments = $"run --project ./{projectName}"
    };
    Process.Start(pix)?.WaitForExit();
}

record Package(string Name, string Version);