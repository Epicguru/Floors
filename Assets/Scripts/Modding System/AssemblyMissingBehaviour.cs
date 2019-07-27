
public enum AssemblyMissingBehaviour
{
    /// <summary>
    /// The mod loading will be cancelled.
    /// </summary>
    ABORT,
    /// <summary>
    /// The mod loading will be cancelled if the assembly is the only one defined by the mod.
    /// If it is one of multiple assemblies that are defined, the mod loading continues regardless of whether
    /// those other asemblies are loaded correctly. Can result in broken mods.
    /// </summary>
    ABORT_IF_SINGLE,
    /// <summary>
    /// Continue loading the mod regardless. Can result in broken mods.
    /// </summary>
    CONTINUE
}
