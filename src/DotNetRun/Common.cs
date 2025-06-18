namespace DotNetRun;

internal static class Common
{
    internal static string GetTemporaryFolder()
    {
        var tempPath = Path.GetTempPath();
        var newTempFolder = "DotNetRunScriptingFolder";
        var path = Path.Combine(tempPath, newTempFolder);
        Directory.CreateDirectory(path);
        return path;
    }
}
