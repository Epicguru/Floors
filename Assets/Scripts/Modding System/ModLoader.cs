
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

public static class ModLoader
{
    public static AssemblyMissingBehaviour AssemblyMissingBehaviour = AssemblyMissingBehaviour.ABORT;
    private static Dictionary<string, Mod> mods = new Dictionary<string, Mod>();

    public static bool IsLoaded(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return mods.ContainsKey(name.Trim().ToLower());
    }

    public static Mod GetMod(string modName)
    {
        if (string.IsNullOrWhiteSpace(modName))
            return null;

        modName = modName.Trim().ToLower();

        if (mods.ContainsKey(modName))
        {
            return mods[modName];
        }
        else
        {
            return null;
        }
    }

    public static Mod LoadMod(string modPath)
    {
        const string META_FILE = "Info.txt";
        const string ASSEMBLIES_DIR = "Assemblies";
        const string BUNDLE_FILE = "Bundle";

        if (string.IsNullOrWhiteSpace(modPath))
        {
            Debug.LogError("Mod path cannot be null or whitespace.");
            return null;
        }

        string name = new FileInfo(modPath).Name;

        // Step 1: Validate the folder.
        if (!Directory.Exists(modPath))
        {
            Debug.LogError($"Failed to find mod directory at path {modPath}. Mod will not be loaded.");
            return null;
        }

        /*
         * ModName
         * - Info.txt (json metadata file)
         * - Assemblies
         * -- SomeAssembly.dll
         * -- AnotherAssembly.dll
         * - Content.bundle (the single asset bundle that contains content)
         */

        // Step 2: Read metadata.
        ModInfo info;

        string expectedPath = Path.Combine(modPath, META_FILE);
        if (!File.Exists(expectedPath))
        {
            Debug.LogError($"Could not find the metadata file for the loading mod '{name}'. Expected file {expectedPath}. The mod will not be loaded.");
            return null;
        }

        // 2.1 Read text.
        string text = null;
        try
        {
            text = File.ReadAllText(expectedPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while loading the mod metadata file for '{name}'. Mod will not be loaded.");
            Debug.LogError(e);

            return null;
        }

        // 2.2 Parse text into object.
        try
        {
            info = JsonUtility.FromJson<ModInfo>(text);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while parsing the mod metadata file for '{name}'. Mod will not be loaded.");
            Debug.LogError(e);

            return null;
        }

        // Step 3: Check real name and compare to currently loaded mods.
        string realName = info.Name;
        string id = info.ID;
        if (IsLoaded(id))
        {
            Debug.LogError($"Mod {realName} is already currently loaded. It will not be loaded again.");
            return null;
        }

        // Step 4: Read assembly list, validate files, load assemblies.
        List<Assembly> assemblies = new List<Assembly>();
        if(info.Assemblies != null && info.Assemblies.Length != 0)
        {
            foreach (var partialPath in info.Assemblies)
            {
                string part = partialPath;
                if (!part.EndsWith(".dll"))
                    part = part + ".dll";

                expectedPath = Path.Combine(modPath, ASSEMBLIES_DIR, part);
                bool failed = false;

                if (!File.Exists(expectedPath))
                {
                    Debug.LogError($"Did not find file for assembly {part} for mod {realName}. Expected at {expectedPath}.");
                    failed = true;                   
                }

                Assembly ass = null;
                if (!failed)
                {
                    try
                    {
                        ass = Assembly.LoadFile(expectedPath);
                        assemblies.Add(ass);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception whilst loading assembly {part} from {expectedPath} for mod {realName}.");
                        Debug.LogError(e);
                        failed = true;
                    }
                }                

                if (failed)
                {
                    switch (AssemblyMissingBehaviour)
                    {
                        case AssemblyMissingBehaviour.ABORT:
                            Debug.LogError($"Assembly {part} failed to load for {realName}, loading aborted.");
                            return null;
                        case AssemblyMissingBehaviour.ABORT_IF_SINGLE:
                            if (info.Assemblies.Length == 1)
                            {
                                Debug.LogError($"Assembly {part} failed to load for {realName}, loading aborted.");
                                return null;
                            }
                            else
                            {
                                Debug.LogWarning("Mod loading continuing since it is not the only assembly defined.");
                            }
                            break;
                        case AssemblyMissingBehaviour.CONTINUE:
                            Debug.LogWarning("Mod loading will continue.");
                            break;
                    }

                    continue;
                }
            }
        }

        // Step 5: Load asset bundle.
        string bundlePath = Path.Combine(modPath, BUNDLE_FILE);
        AssetBundle bundle = null;
        if (info.HasBundle)
        {
            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"Failed to find asset bundle file for mod {realName}. Expected at {bundlePath}.");
                Debug.LogError("Mod loading will cancel. Note that the assemblies for this mod have already been loaded.");

                return null;
            }

            try
            {
                bundle = AssetBundle.LoadFromFile(bundlePath);
            }
            catch(Exception e)
            {
                Debug.LogError($"Exception while loading the asset bundle for mod {realName}. Mod loading will cancel. Note that the mod assemblies have already been loaded.");
                Debug.LogError(e);
                return null;
            }
        }

        // Step 6: Find the mod class. Otherwise, use the DefaultModImplementation.
        Mod m = null;
        if (assemblies.Count > 0)
        {
            // Try to find a mod subclass.
            List<Type> modTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.DefinedTypes;
                var mods = from t in types
                           where t.IsClass &&
                                 !t.IsAbstract &&
                                 !t.ContainsGenericParameters &&
                                 t.IsSubclassOf(typeof(Mod))
                           select t;
                modTypes.AddRange(mods);
            }

            if(modTypes.Count == 0)
            {
                Debug.LogError($"Searching {assemblies.Count} assemblies from mod {realName} yeilded no Mod implementations! This is probably a major bug in the mod. Using default mod implementation. Mod may be broken.");
                m = new DefaultModImplementation();
            }
            else
            {
                if(modTypes.Count > 1)
                    Debug.LogError($"Mod {realName} contains {modTypes.Count} mod class implementations, only 1 is expected. {modTypes[0].FullName} will be used.");

                // TODO perhaps scan through all of the options and check if they work. But then again, there should only be one.
                Type t = modTypes[0];

                ConstructorInfo constructor = null;
                try
                {
                    constructor = t.GetConstructor(new Type[] { });
                    if (constructor == null)
                        throw new Exception("Failed to find no-argument public constructor.");
                }
                catch(Exception e)
                {
                    Debug.LogError($"Failed to get no-argument public constructor for mod class {t.FullName} of mod {realName}. The mod will still be loaded, but using the default mod class implementation. Mod will probably be broken.");
                    Debug.LogError(e);
                    m = new DefaultModImplementation();
                }

                if(constructor != null)
                {
                    m = (Mod)constructor.Invoke(new object[] { });
                }
            }
        }
        else
        {
            m = new DefaultModImplementation();
        }

        // Step 7: Put everything together in the mod class.
        m.Info = info;
        m.Bundle = bundle;
        m.Assemblies = assemblies.ToArray();

        mods.Add(id, m);

        Debug.Log($"Loaded mod {realName} ({id}) with {m.Assemblies.Length} assemblies, {(m.Bundle == null ? "no assets." : "one asset bundle.")}");

        return m;
    }
}
