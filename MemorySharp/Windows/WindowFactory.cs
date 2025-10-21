﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using MemorySharp.Internals;

namespace MemorySharp.Windows;

/// <summary>
/// Class providing tools for manipulating windows.
/// </summary>
public class WindowFactory : IFactory
{
    /// <summary>
    /// The reference of the <see cref="MemorySharp"/> object.
    /// </summary>
    private readonly MemoryManagement.MemorySharp _memorySharp;

    /// <summary>
    /// Gets all the child windows that belong to the application.
    /// </summary>
    public IEnumerable<RemoteWindow> ChildWindows => ChildWindowHandles.Select(handle => new RemoteWindow(_memorySharp, handle));

    /// <summary>
    /// Gets all the child window handles that belong to the application.
    /// </summary>
    internal IEnumerable<nint> ChildWindowHandles => WindowCore.EnumChildWindows(_memorySharp.Native.MainWindowHandle);

    /// <summary>
    /// Gets the main window of the application.
    /// </summary>
    public RemoteWindow MainWindow => new(_memorySharp, MainWindowHandle);

    /// <summary>
    /// Gets the main window handle of the application.
    /// </summary>
    public nint MainWindowHandle => _memorySharp.Native.MainWindowHandle;

    /// <summary>
    /// Gets all the windows that have the same specified title.
    /// </summary>
    /// <param name="windowTitle">The window title string.</param>
    /// <returns>A collection of <see cref="RemoteWindow"/>.</returns>
    public IEnumerable<RemoteWindow> this[string windowTitle] => GetWindowsByTitle(windowTitle);

    /// <summary>
    /// Gets all the windows that belong to the application.
    /// </summary>
    public IEnumerable<RemoteWindow> RemoteWindows => WindowHandles.Select(handle => new RemoteWindow(_memorySharp, handle));

    /// <summary>
    /// Gets all the window handles that belong to the application.
    /// </summary>
    internal IEnumerable<nint> WindowHandles => new List<nint>(ChildWindowHandles) {MainWindowHandle};

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFactory"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    internal WindowFactory(MemoryManagement.MemorySharp memorySharp) => _memorySharp = memorySharp;

    /// <summary>
    /// Releases all resources used by the <see cref="WindowFactory"/> object.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose... yet
    }

    /// <summary>
    /// Gets all the windows that have the specified class name.
    /// </summary>
    /// <param name="className">The class name string.</param>
    /// <returns>A collection of <see cref="RemoteWindow"/>.</returns>
    public IEnumerable<RemoteWindow> GetWindowsByClassName(string className) => WindowHandles
                                                                                .Where(handle => WindowCore.GetClassName(handle) == className)
                                                                                .Select(handle => new RemoteWindow(_memorySharp, handle));

    /// <summary>
    /// Gets all the windows that have the same specified title.
    /// </summary>
    /// <param name="windowTitle">The window title string.</param>
    /// <returns>A collection of <see cref="RemoteWindow"/>.</returns>
    public IEnumerable<RemoteWindow> GetWindowsByTitle(string windowTitle) => WindowHandles
                                                                              .Where(handle => WindowCore.GetWindowText(handle) == windowTitle)
                                                                              .Select(handle => new RemoteWindow(_memorySharp, handle));

    /// <summary>
    /// Gets all the windows that contain the specified title.
    /// </summary>
    /// <param name="windowTitle">A part a window title string.</param>
    /// <returns>A collection of <see cref="RemoteWindow"/>.</returns>
    public IEnumerable<RemoteWindow> GetWindowsByTitleContains(string windowTitle) => WindowHandles
                                                                                      .Where(handle => WindowCore.GetWindowText(handle).Contains(windowTitle))
                                                                                      .Select(handle => new RemoteWindow(_memorySharp, handle));
}