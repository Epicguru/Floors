
/// <summary>
/// Contains info about a mod that may or may not be loaded. This info is saved into a metadata
/// file that can be quickly loaded without loading the whole mod.
/// </summary>
public struct ModInfo
{
    public string ID;
    public string Name;
    public string Author;
    public string Version;
    public string Description;
    public bool HasBundle;
    public string[] Assemblies;
}
