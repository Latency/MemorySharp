// ****************************************************************************
// Project:  MemorySharp
// File:     RemoteModule.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************
// ReSharper disable UnusedMember.Global

using dnlib.DotNet;
using System.Diagnostics;
using MemorySharp.Memory;

namespace MemorySharp.Modules;
/// <summary>
///     Class repesenting a module in the remote process.
/// </summary>
public class RemoteModule(ProcessModule processModule, ModuleFactory moduleFactory)
{
    #region Fields

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Lazy<ModuleDefMD> _singletonModuleDef = new(() => ModuleDefMD.Load(processModule.FileName) ?? throw new InvalidOperationException($"Unable to load module '{processModule.ModuleName}'."));

    #endregion Fields

    #region Properties

    #region Region

    public required RemoteRegion Region { get; init; }

    #endregion Region

    #region IsValid

    public virtual bool IsValid => BaseAddress != IntPtr.Zero;

    #endregion IsValid

    #region ModuleName

    /// <devdoc>
    ///     Returns the name of the Module.
    /// </devdoc>
    public string ModuleName => processModule.ModuleName;

    #endregion ModuleName

    #region FileName

    /// <devdoc>
    ///     Returns the full file path for the location of the module.
    /// </devdoc>
    public string FileName => processModule.FileName;

    #endregion FileName

    #region BaseAddress

    /// <devdoc>
    ///     Returns the memory address that the module was loaded at.
    /// </devdoc>
    public IntPtr BaseAddress => processModule.BaseAddress;

    #endregion BaseAddress

    #region ModuleMemorySize

    /// <devdoc>
    ///     Returns the amount of memory required to load the module.  This does
    ///     not include any additional memory allocations made by the module once
    ///     it is running; it only includes the size of the static code and data
    ///     in the module file.
    /// </devdoc>
    public int ModuleMemorySize => processModule.ModuleMemorySize;

    #endregion ModuleMemorySize

    #region EntryPointAddress

    /// <devdoc>
    ///     Returns the memory address for function that runs when the module is
    ///     loaded and run.
    /// </devdoc>
    public IntPtr EntryPointAddress => processModule.EntryPointAddress;

    #endregion EntryPointAddress

    #region FileVersionInfo

    /// <devdoc>
    ///     Returns version information about the module.
    /// </devdoc>
    public FileVersionInfo FileVersionInfo => field ??= FileVersionInfo.GetVersionInfo(processModule.FileName);

    #endregion FileVersionInfo

    #region IsMainModule

    /// <summary>
    ///     State if this is the main module of the remote process.
    /// </summary>
    public bool IsMainModule => moduleFactory.MainModule.BaseAddress == BaseAddress;

    #endregion IsMainModule

    #region ModuleDef

    /// <summary>
    ///     The definition of the remote module.
    /// </summary>
    public ModuleDefMD ModuleDef => _singletonModuleDef.Value;

    #endregion ModuleDef

    #region This

    /// <summary>
    ///     Gets the specified function in the remote module.
    /// </summary>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>A new instance of a <see cref="TypeDef" /> class.</returns>
    public TypeDef? this[string functionName] => FindFunction(functionName);

    #endregion This

    #endregion Properties

    #region Methods

    #region FindFunction

    /// <summary>
    ///     Finds the specified function in the remote module.
    /// </summary>
    /// <param name="functionName">The name of the function (case sensitive).</param>
    /// <returns>A new instance of a <see cref="TypeDef" /> class.</returns>
    /// <remarks>
    ///     Interesting article on how DLL loading works: http://msdn.microsoft.com/en-us/magazine/bb985014.aspx
    /// </remarks>
    public TypeDef? FindFunction(string functionName) => ModuleDef.Find(functionName, false);

    #endregion FindFunction

    #region InternalEject (internal)

    /// <summary>
    ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
    /// <param name="module">The module to eject.</param>
    internal static void InternalEject(MemorySharp memorySharp, RemoteModule module)
    {
        var def = memorySharp["kernel32"]["FreeLibrary"];
        if (def is null)
            throw new NullReferenceException("Unable to find FreeLibrary function.");

        // Call FreeLibrary remotely
        memorySharp.Threads.CreateAndJoin((nint)def.Rid, module.BaseAddress);
    }

    #endregion InternalEject

    #region ToString (override)

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {ModuleName}";

    #endregion ToString (override)

    #endregion Methods
}