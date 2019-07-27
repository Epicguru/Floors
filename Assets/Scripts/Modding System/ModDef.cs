
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MyMod", menuName = "Mods")]
public class ModDef : ScriptableObject
{
    public string Name = "My Mod Name";
    [HideInInspector]
    public string ID = "my_mod_name";
    public string Author = "Your Name";
    public string Version = "0.1";
    [TextArea(5, 5)]
    public string Description = "This is the default mod description";

    [HideInInspector]
    public bool HasCreated = false;

#if UNITY_EDITOR
    /// <summary>
    /// Builds the mod. Assumes that the mod def state is valid (name and all that).
    /// </summary>
    public void Build(bool zip)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        Debug.Log($"Building mod {Name} ({ID})...");

        // Step 0: Clear the output.
        string modDir = Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "Mod Builds");
        string outputDir = Path.Combine(modDir, ID);

        if (zip)
        {
            if(File.Exists(Path.Combine(modDir, ID + ".zip")))
            {
                File.Delete(Path.Combine(modDir, ID + ".zip"));
            }
        }
        if (Directory.Exists(outputDir))
        {
            Debug.Log("Clearing output dir.");
            Directory.Delete(outputDir, true);
        }

        // Step 1: Assign all assets in the Content folder to the asset bundle.
        // TODO get all assets in subfolders too.
        string[] guids = UnityEditor.AssetDatabase.FindAssets("", new string[] { "Assets/Mods/" + ID + "/Content" });

        int count = 0;
        foreach (var guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetImporter.GetAtPath(path).assetBundleName = ID;
            count++;
        }
        Debug.Log($"assigned {count} assets to the bundle.");

        // Step 2: Create mod info.
        ModInfo info = new ModInfo
        {
            ID = ID,
            Name = Name,
            Author = Author,
            Version = Version,
            Description = Description,
            HasBundle = count != 0,
            Assemblies = new string[] { ID + ".dll" } // TODO allow more that one assembly to exist. Such as packaging Json.NET.
        };

        // Step 3: Write info file to the output directory.
        string infoPath = Path.Combine(outputDir, "Info.txt");
        string tempBundle = Path.Combine(outputDir, "TempBundleDir");
        string assembliesPath = Path.Combine(outputDir, "Assemblies");

        string json = JsonUtility.ToJson(info, true);
        GameIO.EnsureParentDir(infoPath);
        File.WriteAllText(infoPath, json);
        Debug.Log("Written mod info.");

        // Step 4: Write asset bundle. Takes ages.
        BuildBundleTo(tempBundle);
        File.Copy(Path.Combine(tempBundle, ID), Path.Combine(outputDir, "Bundle"));
        File.Copy(Path.Combine(tempBundle, ID + ".manifest"), Path.Combine(outputDir, "Bundle.manifest"));
        Directory.Delete(tempBundle, true);
        Debug.Log("Built asset bundle.");
        
        // Step 5: Write assemblies. They just need to be copied, they are automatically compiled by unity.
        if(!Directory.Exists(assembliesPath))
            Directory.CreateDirectory(assembliesPath);

        // TODO allow custom assemblies.
        CopyAssembliesTo(assembliesPath, new string[] { ID });
        Debug.Log("Copied assemblies to output.");

        if (zip)
        {
            Debug.Log("Zipping...");
            System.IO.Compression.ZipFile.CreateFromDirectory(outputDir, Path.Combine(modDir, ID + ".zip"));
            Debug.Log("Deleting uncompressed directory...");
            Directory.Delete(outputDir, true);
        }

        watch.Stop();
        Debug.Log($"Finished build in {watch.Elapsed.TotalMilliseconds:n1} ms.");
    }

    private bool CopyAssembliesTo(string outputPath, string[] assemblies)
    {
        
        Debug.Log("Copying assemblies...");

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        bool codeless = false;

        foreach (var item in assemblies)
        {
            string dllName = item + ".dll";
            string source = Path.Combine(new DirectoryInfo(Application.dataPath).Parent.ToString(), "Library", "ScriptAssemblies", dllName);
            string dest = Path.Combine(outputPath, dllName);

            if (File.Exists(source))
            {
                File.Copy(source, dest);
                Debug.Log($"Copied {dllName}");
            }
            else
            {
                Debug.LogWarning($"Assembly file not found, so not copied to output! {source}");
            }

        }
        Debug.Log("Done!");
        return codeless;
    }

    private void BuildBundleTo(string outputPath)
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var built = UnityEditor.BuildPipeline.BuildAssetBundles(outputPath, UnityEditor.BuildAssetBundleOptions.None, UnityEditor.BuildTarget.StandaloneWindows64);
        //Debug.Log("Done!");

        string A = ID;
        string B = ID + ".manifest";

        // Clean up.
        foreach (var item in Directory.EnumerateFiles(outputPath))
        {
            string i = new FileInfo(item).Name;
            if (i != A && i != B)
            {
                File.Delete(item);
            }
        }
    }

#endif
}
