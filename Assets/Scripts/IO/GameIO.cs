
using System.IO;

public static class GameIO
{
    public static void EnsureParentDir(string filePath)
    {
        if (File.Exists(filePath))
            return;

        string dirPath = new FileInfo(filePath).Directory.FullName;

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }
}
