namespace Heavens.Core.Extension.Helper;

public class RootPathHelper
{
    static RootPathHelper()
    {
        CurrentDir = Directory.GetCurrentDirectory();
        TempDir = Path.Combine(CurrentDir, "Temp");
    }

    private RootPathHelper()
    {
    }
    public static string CurrentDir { get; }
    public static string TempDir { get; }
}
