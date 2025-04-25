// ****************************************************************************
// Project:  MemorySharp
// File:     Manager.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Internals;

/// <summary>
///     Class managing objects implementing <see cref="INamedElement" /> interface.
/// </summary>
public abstract class Manager<T>
    where T : INamedElement
{
    #region Fields

    /// <summary>
    ///     The collection of the elements (writable).
    /// </summary>
    protected Dictionary<string, T> InternalItems = new();

    #endregion

    #region Properties

    /// <summary>
    ///     The collection of the elements.
    /// </summary>
    public IReadOnlyDictionary<string, T> Items => InternalItems;

    #endregion

    #region Methods

    #region DisableAll

    /// <summary>
    ///     Disables all items in the manager.
    /// </summary>
    public void DisableAll()
    {
        foreach (var item in InternalItems)
            item.Value.Disable();
    }

    #endregion

    #region EnableAll

    /// <summary>
    ///     Enables all items in the manager.
    /// </summary>
    public void EnableAll()
    {
        foreach (var item in InternalItems)
            item.Value.Enable();
    }

    #endregion

    #region Remove

    /// <summary>
    ///     Removes an element by its name in the manager.
    /// </summary>
    /// <param name="name">The name of the element to remove.</param>
    public void Remove(string name)
    {
        // Check if the element exists in the dictionary
        if (InternalItems.TryGetValue(name, out T value))
            try
            {
                value.Dispose();
            }
            finally
            {
                // Remove the element from the dictionary
                InternalItems.Remove(name);
            }
    }

    /// <summary>
    ///     Remove a given element.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    public void Remove(T item)
    {
        Remove(item.Name);
    }

    #endregion

    #region RemoveAll

    /// <summary>
    ///     Removes all the elements in the manager.
    /// </summary>
    public void RemoveAll()
    {
        // For each element
        foreach (var item in InternalItems)
            // Dispose it
            item.Value.Dispose();
        // Clear the dictionary
        InternalItems.Clear();
    }

    #endregion

    #endregion
}