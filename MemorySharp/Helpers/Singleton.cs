﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
 */
namespace Binarysharp.Helpers;

/// <summary>
/// Static helper used to create or get a singleton from another class.
/// </summary>
/// <typeparam name="T">The type to create or get a singleton.</typeparam>
public static class Singleton<T> where T : new()
{
    private static readonly Lazy<T> SingletonInstance = new(() => new T());

    /// <summary>
    /// Gets the singleton of the given type.
    /// </summary>
    public static T Instance => SingletonInstance.Value;
}