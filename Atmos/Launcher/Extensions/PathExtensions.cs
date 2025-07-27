namespace Launcher.Extensions;

public static class PathExtensions
{
    public static string CombinePlatformExe(string path1, string exeName)
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(path1, $"{exeName}.exe");
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return Path.Combine(path1, exeName);
        }

        throw new PlatformNotSupportedException("Unsupported operating system for path combination.");
    }
}
