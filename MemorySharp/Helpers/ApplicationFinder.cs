﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using Binarysharp.Windows;

namespace Binarysharp.Helpers;

/// <summary>
/// Static helper class providing tools for finding applications.
/// </summary>
public static class ApplicationFinder
{
    /// <summary>
    /// Gets all top-level windows on the screen.
    /// </summary>
    public static IEnumerable<nint> TopLevelWindows => WindowCore.EnumTopLevelWindows();

    /// <summary>
    /// Gets all the windows on the screen.
    /// </summary>
    public static IEnumerable<nint> Windows => WindowCore.EnumAllWindows();

    /// <summary>
    /// Returns a new <see cref="Process"/> component, given the identifier of a process.
    /// </summary>
    /// <param name="processId">The system-unique identifier of a process resource.</param>
    /// <returns>A <see cref="Process"/> component that is associated with the local process resource identified by the processId parameter.</returns>
    public static Process FromProcessId(int processId) => Process.GetProcessById(processId);

    /// <summary>
    /// Creates an collection of new <see cref="Process"/> components and associates them with all the process resources that share the specified process name.
    /// </summary>
    /// <param name="processName">The friendly name of the process.</param>
    /// <returns>A collection of type <see cref="Process"/> that represents the process resources running the specified application or file.</returns>
    public static IEnumerable<Process> FromProcessName(string processName) => Process.GetProcessesByName(processName);

    /// <summary>
    /// Creates a collection of new <see cref="Process"/> components and associates them with all the process resources that share the specified class name.
    /// </summary>
    /// <param name="className">The class name string.</param>
    /// <returns>A collection of type <see cref="Process"/> that represents the process resources running the specified application or file.</returns>
    public static IEnumerable<Process> FromWindowClassName(string className) => Windows.Where(window => WindowCore.GetClassName(window) == className).Select(FromWindowHandle);

    /// <summary>
    /// Retrieves a new <see cref="Process"/> component that created the window. 
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <returns>A <see cref="Process"/>A <see cref="Process"/> component that is associated with the specified window handle.</returns>
    public static Process FromWindowHandle(nint windowHandle) => FromProcessId(WindowCore.GetWindowProcessId(windowHandle));

    /// <summary>
    /// Creates a collection of new <see cref="Process"/> components and associates them with all the process resources that share the specified window title.
    /// </summary>
    /// <param name="windowTitle">The window title string.</param>
    /// <returns>A collection of type <see cref="Process"/> that represents the process resources running the specified application or file.</returns>
    public static IEnumerable<Process> FromWindowTitle(string windowTitle) => Windows.Where(window => WindowCore.GetWindowText(window) == windowTitle).Select(FromWindowHandle);

    /// <summary>
    /// Creates a collection of new <see cref="Process"/> components and associates them with all the process resources that contain the specified window title.
    /// </summary>
    /// <param name="windowTitle">A part a window title string.</param>
    /// <returns>A collection of type <see cref="Process"/> that represents the process resources running the specified application or file.</returns>
    public static IEnumerable<Process> FromWindowTitleContains(string windowTitle) => Windows.Where(window => WindowCore.GetWindowText(window).Contains(windowTitle)).Select(FromWindowHandle);
}