
using System.Reflection;
using UnityEngine;
/// <summary>
/// This class represents a loaded mod in memory.
/// </summary>
public abstract class Mod
{
    /// <summary>
    /// The name of the mod. Mod names are always lower case, and can only contain
    /// alphanumerical values and underscores. This is the same value as Info.Name.
    /// </summary>
    public string Name { get { return Info.Name; } }

    /// <summary>
    /// Contains information about this mod, such as a description of its functionality.
    /// </summary>
    public ModInfo Info { get; internal set; }

    /// <summary>
    /// The reference to the asset bundle that this mod is linked to.
    /// This value will be null if the bundle was not loaded correctly, or if the mod does
    /// not have any asset bundle content such as textures or models.
    /// </summary>
    public AssetBundle Bundle { get; internal set; }

    /// <summary>
    /// An array that holds references to all the assemblies that the mod loaded.
    /// There will normally be only 1 or none at all.
    /// This array will not be null but may be empty.
    /// </summary>
    public Assembly[] Assemblies { get; internal set; }

    /// <summary>
    /// Called once after the mod is loaded from the disk into memory.
    /// </summary>
    public abstract void Init();
}
