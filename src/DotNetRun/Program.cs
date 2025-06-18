using DotNetRun;

if (args.Length == 0) return;

var file = args[0];

if (Path.GetExtension(file).Equals(".sql", StringComparison.CurrentCultureIgnoreCase) && File.Exists(file))
{
    await SqlProjectBuilder.Build(args);
}
else if (Path.GetExtension(file).Equals(".cs", StringComparison.CurrentCultureIgnoreCase) && File.Exists(file))
{
    CSProjectBuilder.Build(args);
}
else
{
    Console.WriteLine($"Unsupported file type: {Path.GetExtension(file)}");
}

Console.WriteLine();