﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
 */

namespace MemorySharp.Internals;

/// <summary>
/// Class managing objects implementing <see cref="INamedElement"/> interface.
/// </summary>
public abstract class Manager<T> where T : INamedElement
{
    /// <summary>
    /// The collection of the elements (writable).
    /// </summary>
    protected Dictionary<string, T> InternalItems = new();

    /// <summary>
    /// The collection of the elements.
    /// </summary>
    public IReadOnlyDictionary<string, T> Items => InternalItems;

    /// <summary>
    /// Disables all items in the manager.
    /// </summary>
    public void DisableAll()
    {
        foreach (var item in InternalItems)
            item.Value.Disable();
    }

    /// <summary>
    /// Enables all items in the manager.
    /// </summary>
    public void EnableAll()
    {
        foreach (var item in InternalItems)
            item.Value.Enable();
    }

    /// <summary>
    /// Removes an element by its name in the manager.
    /// </summary>
    /// <param name="name">The name of the element to remove.</param>
    public void Remove(string name)
    {
        // Check if the element exists in the dictionary
        if (InternalItems.TryGetValue(name, out _))
        {
            try
            {
                // Dispose the element
                InternalItems[name].Dispose();
            }
            finally
            {
                // Remove the element from the dictionary
                InternalItems.Remove(name);
            }
        }
    }

    /// <summary>
    /// Remove a given element.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    public void Remove(T item) => Remove(item.Name);

    /// <summary>
    /// Removes all the elements in the manager.
    /// </summary>
    public void RemoveAll()
    {
        // For each element
        foreach (var item in InternalItems)
            item.Value.Dispose();

        // Clear the dictionary
        InternalItems.Clear();
    }
}