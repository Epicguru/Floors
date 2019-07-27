using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModDef))]
public class NetObjectEditor : Editor
{
    private static StringBuilder str = new StringBuilder();

    private static string idInput = string.Empty;
    private static bool zip = false;

    public override void OnInspectorGUI()
    {
        ModDef m = target as ModDef;

        if (!m.HasCreated)
        {
            EditorGUILayout.HelpBox("You need to give a mod ID. This ID cannot change after the mod is created.", MessageType.Info);
            idInput = idInput.Trim().ToLower();
            idInput = GUILayout.TextField(idInput);

            bool valid = IsValidID(idInput, out string error);

            if (valid && GUILayout.Button("Create mod files"))
            {
                // Assign id.
                m.ID = idInput;

                // Create a folder next to the mod def file that has the same name as the mod def file.
                string modFolder = "Assets/Mods/" + m.ID;
                AssetDatabase.CreateFolder("Assets/Mods", m.ID);
                AssetDatabase.CreateFolder(modFolder, "Scripts");
                AssetDatabase.CreateFolder(modFolder, "Content");

                CreateAssemblyDef(m.ID);

                m.HasCreated = true;
            }
            else if (!valid)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
            return;
        }

        GUI.enabled = false;
        GUILayout.Label($"ID: {m.ID}");
        GUI.enabled = true;

        DrawDefaultInspector();
        EditorGUILayout.Space();
        bool canBuild = true;

        if (canBuild)
        {
            GUILayout.BeginHorizontal();
            zip = GUILayout.Toggle(zip, "Zip Compress", GUILayout.ExpandWidth(false));
            bool build = GUILayout.Button("Build Mod", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            if (build)
            {
                m.Build(zip);
            }
        }
        else
        {
            GUILayout.Label("Fix errors to build the mod.");
        }
    }

    public static bool IsValidID(string id, out string error)
    {
        bool canBuild = true;
        str.Clear();
        str.AppendLine("Invalid character(s) in mod ID:");
        bool f = true;
        foreach (char c in id)
        {
            bool notAlpha = (c < 'a' || c > 'z');
            bool notNum = (c < '0' || c > '9');
            bool isUnderscore = c == '_';
            if (notAlpha && notNum && !isUnderscore)
            {
                if (!f)
                {
                    str.Append(", ");
                }
                str.Append('\'');
                str.Append(c);
                str.Append('\'');
                f = false;
                canBuild = false;
            }
        }
        str.AppendLine();
        str.Append("ID must be all lowercase, and have only letters, numbers and underscores.");

        error = str.ToString();
        return canBuild;
    }

    private void CreateAssemblyDef(string id)
    {
        string str = "{\n" +
                     "\"name\": \"" + id + "\",\n" +
                     "\"references\": [\n" +
                     "\n" +
                     "],\n" +
                     "\"optionalUnityReferences\": [],\n" +
                     "\"includePlatforms\": [],\n" +
                     "\"excludePlatforms\": [],\n" +
                     "\"allowUnsafeCode\": false,\n" +
                     "\"overrideReferences\": false,\n" +
                     "\"precompiledReferences\": [],\n" +
                     "\"autoReferenced\": true,\n" +
                     "\"defineConstraints\": [],\n" +
                     "\"versionDefines\": []\n" +
                     "}";

        string path = Path.Combine(Application.dataPath, "Mods", id, "Scripts", id + ".asmdef");

        //var stream = File.CreateText(path);
        //stream.Write(str);
        //stream.Close();
        //stream.Dispose();

        File.WriteAllText(path, str);

        AssetDatabase.ImportAsset("Assets/Mods/" + id + "/Scripts/" + id + ".asmdef");
    }
}