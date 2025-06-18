using System.Diagnostics;
using System.Text;

if (args.Length == 0) return;

var temporaryFolder = GetTemporaryFolder();

var file = args[0];
var passArgs = args.Length > 1 ? string.Join(' ', args[1..]) : null;

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

ReadCSFile(temporaryFolder, args[0], passArgs);

Console.WriteLine();


static string GetTemporaryFolder()
{
    var tempPath = Path.GetTempPath();
    var newTempFolder = "DotNetRunScriptingFolder";
    var path = Path.Combine(tempPath, newTempFolder);    
    Directory.CreateDirectory(path);
    return path;
}

static void ReadCSFile(string path, string projectFile, string? args = null)
{    
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
    HashSet<Import> imports = [];
    var sb = new StringBuilder();
    foreach (var line in lines)
    {
        if (line.StartsWith("#:package"))
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
        else if (line.StartsWith("#:import"))
        {
            var import = line.Trim().Replace("#:import ", "");
            if (import.ToLower() == "*.cs")
            {
                var files = Directory.GetFiles(Environment.CurrentDirectory, "*.cs", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (Path.GetFileName(file) != Path.GetFileName(projectFile) && !imports.Any(i => i.File == file))
                    {
                        imports.Add(new(Path.GetFileName(file)));
                    }
                }
            }
            else if (File.Exists(import))
            {
                imports.Add(new(import));
            }
        }
        else
        {
            sb.AppendLine(line);
        }
    }

    if (imports.Count > 0)
    {
        foreach (var import in imports)
        {
            File.Copy(import.File, Path.Combine(path, projectName, import.File), true);
        }
    }

    File.WriteAllText(programFile, sb.ToString());
    var runProject = new ProcessStartInfo
    {
        WorkingDirectory = path,
        FileName = "dotnet",
        Arguments = $"run -v q {args} --project ./{projectName}"
    };
    Process.Start(runProject)?.WaitForExit();
}

record Package(string Name, string Version);
record Import(string File);