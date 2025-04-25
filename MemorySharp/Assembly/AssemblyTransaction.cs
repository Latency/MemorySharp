// ****************************************************************************
// Project:  MemorySharp
// File:     AssemblyTransaction.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using MemorySharp.Internals;

namespace MemorySharp.Assembly;

/// <summary>
///     Class representing a transaction where the user can insert mnemonics.
///     The code is then executed when the object is disposed.
/// </summary>
public class AssemblyTransaction : IDisposable
{
    #region Fields

    /// <summary>
    ///     The reference of the <see cref="MemorySharp" /> object.
    /// </summary>
    protected readonly MemorySharp MemorySharp;

    /// <summary>
    ///     The builder contains all the instructions inserted by the user.
    /// </summary>
    public readonly List<string> Instructions;

    /// <summary>
    ///     The exit code of the thread created to execute the assembly code.
    /// </summary>
    protected IntPtr ExitCode;

    #endregion

    #region Properties

    #region Address

    /// <summary>
    ///     The address where to assembly code is assembled.
    /// </summary>
    public IntPtr Address { get; }

    #endregion

    #region IsAutoExecuted

    /// <summary>
    ///     Gets the value indicating whether the assembly code is executed once the object is disposed.
    /// </summary>
    public bool IsAutoExecuted { get; set; }

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AssemblyTransaction" /> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    public AssemblyTransaction(MemorySharp memorySharp, IntPtr address, bool autoExecute)
    {
        // Save the parameters
        MemorySharp    = memorySharp;
        IsAutoExecuted = autoExecute;
        Address        = address;

        // Initialize the string builder
        Instructions = [];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AssemblyTransaction" /> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    public AssemblyTransaction(MemorySharp memorySharp, bool autoExecute) : this(memorySharp, IntPtr.Zero, autoExecute)
    { }

    #endregion

    #region Methods

    #region Assemble

    /// <summary>
    ///     Assemble the assembly code of this transaction.
    /// </summary>
    /// <returns>An array of bytes containing the assembly code.</returns>
    public byte[] Assemble() => MemorySharp.Assembly.Assembler.Assemble(Instructions);

    #endregion

    #region Dispose (implementation of IDisposable)

    /// <summary>
    ///     Releases all resources used by the <see cref="AssemblyTransaction" /> object.
    /// </summary>
    public virtual void Dispose()
    {
        // If a pointer was specified
        if (Address != IntPtr.Zero)
        {
            // If the assembly code must be executed
            if (IsAutoExecuted)
                ExitCode = MemorySharp.Assembly.InjectAndExecute<IntPtr>(Instructions.ToArray(), Address);
            // Else the assembly code is just injected
            else
                MemorySharp.Assembly.Inject(Instructions.ToArray(), Address);
        }

        // If no pointer was specified and the code assembly code must be executed
        if (Address == IntPtr.Zero && IsAutoExecuted)
            ExitCode = MemorySharp.Assembly.InjectAndExecute<IntPtr>(Instructions.ToArray());
    }

    #endregion

    #region GetExitCode

    /// <summary>
    ///     Gets the termination status of the thread.
    /// </summary>
    public T GetExitCode<T>() => MarshalType<T>.PtrToObject(MemorySharp, ExitCode);

    #endregion

    #endregion
}