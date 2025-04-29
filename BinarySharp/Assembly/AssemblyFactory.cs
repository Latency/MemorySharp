/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.Assembly.Assembler;
using Binarysharp.Assembly.CallingConvention;
using Binarysharp.Internals;
using Binarysharp.Memory;
using Binarysharp.MemoryManagement;
using Binarysharp.Threading;

namespace Binarysharp.Assembly;

/// <summary>
/// Class providing tools for manipulating assembly code.
/// </summary>
public class AssemblyFactory : IFactory
{
    #region Fields
    /// <summary>
    /// The reference of the <see cref="MemorySharp"/> object.
    /// </summary>
    protected readonly MemorySharp MemorySharp;
    #endregion

    #region Properties
    #region Assembler
    /// <summary>
    /// The assembler used by the factory.
    /// </summary>
    public IAssembler Assembler { get; set; }
    #endregion
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFactory"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    internal AssemblyFactory(MemorySharp memorySharp)
    {
        // Save the parameter
        MemorySharp = memorySharp;
        // Create the tool
        Assembler = new Fasm32Assembler();
    }
    #endregion

    #region Methods
    #region BeginTransaction
    /// <summary>
    /// Begins a new transaction to inject and execute assembly code into the process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    /// <returns>The return value is a new transaction.</returns>
    public AssemblyTransaction BeginTransaction(nint address, bool autoExecute = true) => new(MemorySharp, address, autoExecute);

    /// <summary>
    /// Begins a new transaction to inject and execute assembly code into the process.
    /// </summary>
    /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
    /// <returns>The return value is a new transaction.</returns>
    public AssemblyTransaction BeginTransaction(bool autoExecute = true) => new(MemorySharp, autoExecute);

    #endregion
    #region Dispose (implementation of IFactory)
    /// <summary>
    /// Releases all resources used by the <see cref="AssemblyFactory"/> object.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose... yet
    }
    #endregion

    #region Execute
    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? Execute<T>(nint address)
    {
        // Execute and join the code in a new thread
        var thread = MemorySharp.Threads.CreateAndJoin(address);
        // Return the exit code of the thread
        return thread.GetExitCode<T>();
    }

    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint Execute(nint address) => Execute<nint>(address);

    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? Execute<T>(nint address, dynamic parameter)
    {
        // Execute and join the code in a new thread
        RemoteThread thread = MemorySharp.Threads.CreateAndJoin(address, parameter);
        // Return the exit code of the thread
        return thread.GetExitCode<T>();
    }

    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint Execute(nint address, dynamic parameter) => Execute<nint>(address, parameter);

    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? Execute<T>(nint address, CallingConventions callingConvention, params dynamic[] parameters)
    {
        // Marshal the parameters
        var marshalledParameters = parameters.Select(p => MarshalValue.Marshal(MemorySharp, p)).Cast<IMarshalledValue>().ToArray();
        // Start a transaction
        AssemblyTransaction t;
        using (t = BeginTransaction())
        {
            // Get the object dedicated to create mnemonics for the given calling convention
            var calling = CallingConventionSelector.Get(callingConvention);
            // Push the parameters
            t.AddLine(calling.FormatParameters(marshalledParameters.Select(p => p.Reference).ToArray()));
            // Call the function
            t.AddLine(calling.FormatCalling(address));
            // Clean the parameters
            if(calling.Cleanup == CleanupTypes.Caller)
                t.AddLine(calling.FormatCleaning(marshalledParameters.Length));
            // Add the return mnemonic
            t.AddLine("retn");
        }

        // Clean the marshalled parameters
        foreach (var parameter in marshalledParameters)
            parameter.Dispose();

        // Return the exit code
        return t.GetExitCode<T>();
    }

    /// <summary>
    /// Executes the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint Execute(nint address, CallingConventions callingConvention, params dynamic[] parameters) => Execute<nint>(address, callingConvention, parameters);

    #endregion
    #region ExecuteAsync
    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> ExecuteAsync<T>(nint address) => Task.Run(() => Execute<T>(address));

    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> ExecuteAsync(nint address) => ExecuteAsync<nint>(address);

    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T> ExecuteAsync<T>(nint address, dynamic parameter) => Task.Run(() => (Task<T>)Execute<T>(address, parameter));

    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> ExecuteAsync(nint address, dynamic parameter) => ExecuteAsync<nint>(address, parameter);

    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> ExecuteAsync<T>(nint address, CallingConventions callingConvention, params dynamic[] parameters) => Task.Run(() => Execute<T>(address, callingConvention, parameters));

    /// <summary>
    /// Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="address">The address where the assembly code is located.</param>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> ExecuteAsync(nint address, CallingConventions callingConvention, params dynamic[] parameters) => ExecuteAsync<nint>(address, callingConvention, parameters);

    #endregion
    #region Inject
    /// <summary>
    /// Assembles mnemonics and injects the corresponding assembly code into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    public void Inject(string asm, nint address) => MemorySharp.Write(address, Assembler.Assemble(asm, address), false);

    /// <summary>
    /// Assembles mnemonics and injects the corresponding assembly code into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    public void Inject(string[] asm, nint address) => Inject(string.Join("\n", asm), address);

    /// <summary>
    /// Assembles mnemonics and injects the corresponding assembly code into the remote process.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <returns>The address where the assembly code is injected.</returns>
    public RemoteAllocation Inject(string asm)
    {
        // Assemble the assembly code
        var code = Assembler.Assemble(asm);
        // Allocate a chunk of memory to store the assembly code
        var memory = MemorySharp.Memory.Allocate(code.Length);
        // Inject the code
        Inject(asm, memory.BaseAddress);
        // Return the memory allocated
        return memory;
    }

    /// <summary>
    /// Assembles mnemonics and injects the corresponding assembly code into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The address where the assembly code is injected.</returns>
    public RemoteAllocation Inject(string[] asm) => Inject(string.Join("\n", asm));

    #endregion
    #region InjectAndExecute
    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? InjectAndExecute<T>(string asm, nint address)
    {
        // Inject the assembly code
        Inject(asm, address);
        // Execute the code
        return Execute<T>(address);
    }

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint InjectAndExecute(string asm, nint address) => InjectAndExecute<nint>(asm, address);

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? InjectAndExecute<T>(string[] asm, nint address) => InjectAndExecute<T>(string.Join("\n", asm), address);

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint InjectAndExecute(string[] asm, nint address) => InjectAndExecute<nint>(asm, address);

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? InjectAndExecute<T>(string asm)
    {
        // Inject the assembly code
        using var memory = Inject(asm);
        // Execute the code
        return Execute<T>(memory.BaseAddress);
    }

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint InjectAndExecute(string asm) => InjectAndExecute<nint>(asm);

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T? InjectAndExecute<T>(string[] asm) => InjectAndExecute<T>(string.Join("\n", asm));

    /// <summary>
    /// Assembles, injects and executes the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public nint InjectAndExecute(string[] asm) => InjectAndExecute<nint>(asm);

    #endregion
    #region InjectAndExecuteAsync
    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> InjectAndExecuteAsync<T>(string asm, nint address) => Task.Run(() => InjectAndExecute<T>(asm, address));

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> InjectAndExecuteAsync(string asm, nint address) => InjectAndExecuteAsync<nint>(asm, address);

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> InjectAndExecuteAsync<T>(string[] asm, nint address) => Task.Run(() => InjectAndExecute<T>(asm, address));

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <param name="address">The address where the assembly code is injected.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> InjectAndExecuteAsync(string[] asm, nint address) => InjectAndExecuteAsync<nint>(asm, address);

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> InjectAndExecuteAsync<T>(string asm) => Task.Run(() => InjectAndExecute<T>(asm));

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">The mnemonics to inject.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> InjectAndExecuteAsync(string asm) => InjectAndExecuteAsync<nint>(asm);

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<T?> InjectAndExecuteAsync<T>(string[] asm) => Task.Run(() => InjectAndExecute<T>(asm));

    /// <summary>
    /// Assembles, injects and executes asynchronously the mnemonics into the remote process.
    /// </summary>
    /// <param name="asm">An array containing the mnemonics to inject.</param>
    /// <returns>The return value is an asynchronous operation that return the exit code of the thread created to execute the assembly code.</returns>
    public Task<nint> InjectAndExecuteAsync(string[] asm) => InjectAndExecuteAsync<nint>(asm);

    #endregion
    #endregion
}