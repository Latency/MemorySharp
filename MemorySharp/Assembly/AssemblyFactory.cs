// ****************************************************************************
// Project:  MemorySharp
// File:     AssemblyFactory.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using MemorySharp.Assembly.Assembler;
using MemorySharp.Assembly.CallingConvention;
using MemorySharp.Internals;
using MemorySharp.Memory;
using MemorySharp.Threading;
// ReSharper disable UnusedMember.Global

namespace MemorySharp.Assembly;

/// <summary>
///     Class providing tools for manipulating assembly code.
/// </summary>
public class AssemblyFactory(MemorySharp memorySharp) : IFactory
{
    #region Properties

    #region Assembler

    /// <summary>
    ///     Gets or sets the assembler used by the factory.
    /// </summary>
    public KeystoneAssembler Assembler { get; init; } = new(memorySharp.Is64Process? InstructionSet.X64 : InstructionSet.X86);

    #endregion Assembler

    #endregion Properties

    #region Public methods

    #region BeginTransaction

    /// <summary>
    ///     Begins a new transaction to inject and execute assembly code into the process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    /// <returns>The return value is a new transaction.</returns>
    public AssemblyTransaction BeginTransaction(IntPtr address, bool autoExecute = true) => new(memorySharp, address, autoExecute);

    /// <summary>
    ///     Begins a new transaction to inject and execute assembly code into the process.
    /// </summary>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    /// <returns>The return value is a new transaction.</returns>
    public AssemblyTransaction BeginTransaction(bool autoExecute = true) => new(memorySharp, autoExecute);

    #endregion

    #region Dispose (implementation of IFactory)

    /// <summary>
    ///     Releases all resources used by the <see cref="AssemblyFactory" /> object.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose... yet
    }

    #endregion

    #region Execute

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>(IntPtr address)
    {
        // Execute and join the code in a new thread
        var thread = memorySharp.Threads.CreateAndJoin(address);
        // Return the exit code of the thread
        return thread.GetExitCode<T>();
    }

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute(IntPtr address) => Execute<IntPtr>(address);

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>(IntPtr address, dynamic parameter)
    {
        // Execute and join the code in a new thread
        RemoteThread thread = memorySharp.Threads.CreateAndJoin(address, parameter);
        // Return the exit code of the thread
        return thread.GetExitCode<T>();
    }

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute(IntPtr address, dynamic parameter) => Execute<IntPtr>(address, parameter);

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>(IntPtr address, CallingConventions callingConvention, params dynamic[] parameters)
    {
        // Marshal the parameters
        var marshalledParameters = parameters.Select(p => MarshalValue.Marshal(memorySharp, p)).Cast<IMarshalledValue>().ToArray();
        // Start a transaction
        AssemblyTransaction t;
        using (t = BeginTransaction())
        {
            // Get the object dedicated to create mnemonics for the given calling convention
            var calling    = CallingConventionSelector.Get(callingConvention);
            var references = marshalledParameters.Select(p => p.Reference).ToArray();

            calling.FormatCall(address, references, t.Instructions);
            t.Instructions.Add("ret");
        }

        // Clean the marshalled parameters
        foreach (var parameter in marshalledParameters)
            parameter.Dispose();
        // Return the exit code
        return t.GetExitCode<T>();
    }

    /// <summary>
    ///     Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute(IntPtr address, CallingConventions callingConvention, params dynamic[] parameters) => Execute<IntPtr>(address, callingConvention, parameters);

    #endregion

    #region ExecuteAsync

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>(IntPtr address)
    {
        return Task.Run(() => Execute<T>(address));
    }

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync(IntPtr address) => ExecuteAsync<IntPtr>(address);

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>(IntPtr address, dynamic parameter)
    {
        return Task.Run(() => (Task<T>)Execute<T>(address, parameter));
    }

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync(IntPtr address, dynamic parameter) => ExecuteAsync<IntPtr>(address, parameter);

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>(IntPtr address, CallingConventions callingConvention, params dynamic[] parameters)
    {
        return Task.Run(() => Execute<T>(address, callingConvention, parameters));
    }

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync(IntPtr address, CallingConventions callingConvention, params dynamic[] parameters) => ExecuteAsync<IntPtr>(address, callingConvention, parameters);

    #endregion

    #region Inject

    /// <summary>
    ///     Assembles mnemonics and injects the corresponding assembly code into the remote process at the specified address.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    public void Inject(string[] instructions, IntPtr address) => memorySharp.Write(address, Assembler.Assemble(instructions, address), false);

    /// <summary>
    ///     Assembles mnemonics and injects the corresponding assembly code into the remote process.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <returns>The address where the assembly code is injected.</returns>
    public RemoteAllocation Inject(string[] instructions)
    {
        // Assemble the assembly code
        var code = Assembler.Assemble(instructions);
        // Allocate a chunk of memory to store the assembly code
        var memory = memorySharp.Memory.Allocate(code.Length);
        // Inject the code
        Inject(instructions, memory.BaseAddress);
        // Return the memory allocated
        return memory;
    }

    #endregion

    #region InjectAndExecute

    /// <summary>
    ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T InjectAndExecute<T>(string[] instructions, IntPtr address)
    {
        // Inject the assembly code
        Inject(instructions, address);
        // Execute the code
        return Execute<T>(address);
    }

    /// <summary>
    ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr InjectAndExecute(string[] asm, IntPtr address) => InjectAndExecute<IntPtr>(asm, address);

    /// <summary>
    ///     Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T InjectAndExecute<T>(string[] instructions)
    {
        // Inject the assembly code
        using var memory = Inject(instructions);
        // Execute the code
        return Execute<T>(memory.BaseAddress);
    }

    /// <summary>
    ///     Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr InjectAndExecute(string[] asm) => InjectAndExecute<IntPtr>(asm);

    #endregion

    #region InjectAndExecuteAsync

    /// <summary>
    ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> InjectAndExecuteAsync<T>(string[] instructions, IntPtr address)
    {
        return Task.Run(() => InjectAndExecute<T>(instructions, address));
    }

    /// <summary>
    ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> InjectAndExecuteAsync(string[] instructions, IntPtr address) => InjectAndExecuteAsync<IntPtr>(instructions, address);

    /// <summary>
    ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> InjectAndExecuteAsync<T>(string[] instructions)
    {
        return Task.Run(() => InjectAndExecute<T>(instructions));
    }

    /// <summary>
    ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> InjectAndExecuteAsync(string[] instructions) => InjectAndExecuteAsync<IntPtr>(instructions);

    #endregion

    #endregion
}