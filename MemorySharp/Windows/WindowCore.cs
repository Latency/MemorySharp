/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MemorySharp.Helpers;
using MemorySharp.Internals;
using MemorySharp.MemoryManagement.Native;

namespace MemorySharp.Windows;

/// <summary>
/// Static core class providing tools for managing windows.
/// </summary>
public static class WindowCore
{
    /// <summary>
    /// Retrieves the name of the class to which the specified window belongs.
    /// </summary>
    /// <param name="windowHandle">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <returns>The return values is the class name string.</returns>
    public static string GetClassName(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Get the window class name
        var buffer = new char[char.MaxValue];
        if (NativeMethods.GetClassName(windowHandle, buffer, char.MaxValue) == 0)
            throw new Win32Exception("Couldn't get the class name of the window or the window has no class name.");

        return new string(buffer);
    }

    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently working).
    /// </summary>
    /// <returns>A handle to the foreground window. The foreground window can be <c>nint.Zero</c> in certain circumstances, such as when a window is losing activation.</returns>
    public static nint GetForegroundWindow() => NativeMethods.GetForegroundWindow();

    /// <summary>
    /// Retrieves the specified system metric or system configuration setting.
    /// </summary>
    /// <param name="metric">The system metric or configuration setting to be retrieved.</param>
    /// <returns>The return value is the requested system metric or configuration setting.</returns>
    public static int GetSystemMetrics(SystemMetrics metric)
    {
        var ret = NativeMethods.GetSystemMetrics(metric);

        if (ret != 0)
            return ret;

        throw new Win32Exception("The call of GetSystemMetrics failed. Unfortunately, GetLastError code doesn't provide more information.");
    }

    /// <summary>
    /// Gets the text of the specified window's title bar.
    /// </summary>
    /// <param name="windowHandle">A handle to the window containing the text.</param>
    /// <returns>The return value is the window's title bar.</returns>
    public static string GetWindowText(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Get the size of the window's title
        var capacity = NativeMethods.GetWindowTextLength(windowHandle);
        // If the window doesn't contain any title
        if (capacity == 0)
            return string.Empty;

        // Get the text of the window's title bar text
        var len    = capacity + 1;
        var buffer = new char[len];
        if (NativeMethods.GetWindowText(windowHandle, buffer, len) == 0)
            throw new Win32Exception("Couldn't get the text of the window's title bar or the window has no title.");

        return new string(buffer);
    }

    /// <summary>
    /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <returns>The return value is a <see cref="WindowPlacement"/> structure that receives the show state and position information.</returns>
    public static WindowPlacement GetWindowPlacement(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Allocate a WindowPlacement structure
        WindowPlacement placement;
        placement.Length = Marshal.SizeOf(typeof(WindowPlacement));

        // Get the window placement
        if (!NativeMethods.GetWindowPlacement(windowHandle, out placement))
            throw new Win32Exception("Couldn't get the window placement.");

        return placement;
    }

    /// <summary>
    /// Retrieves the identifier of the process that created the window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <returns>The return value is the identifier of the process that created the window.</returns>
    public static int GetWindowProcessId(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Get the process id
        NativeMethods.GetWindowThreadProcessId(windowHandle, out var processId);

        return processId;
    }

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <returns>The return value is the identifier of the thread that created the window.</returns>
    public static int GetWindowThreadId(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Get the thread id
        return NativeMethods.GetWindowThreadProcessId(windowHandle, out _);
    }

    /// <summary>
    /// Enumerates all the windows on the screen.
    /// </summary>
    /// <returns>A collection of handles of all the windows.</returns>
    public static IEnumerable<nint> EnumAllWindows()
    {
        // Create the list of windows
        var list = new List<nint>();

        // For each top-level windows
        foreach (var topWindow in EnumTopLevelWindows())
        {
            // Add this window to the list
            list.Add(topWindow);
            // Enumerate and add the children of this window
            list.AddRange(EnumChildWindows(topWindow));
        }

        // Return the list of windows
        return list;
    }

    /// <summary>
    /// Enumerates recursively all the child windows that belong to the specified parent window.
    /// </summary>
    /// <param name="parentHandle">The parent window handle.</param>
    /// <returns>A collection of handles of the child windows.</returns>
    public static IEnumerable<nint> EnumChildWindows(nint parentHandle)
    {
        var childHandles = new List<nint>();

        var gcChildhandlesList      = GCHandle.Alloc(childHandles);
        var pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

        try
        {
            var childProc = new EnumWindowsProc(Callback);
            NativeMethods.EnumChildWindows(parentHandle, childProc, pointerChildHandlesList);
        }
        finally
        {
            gcChildhandlesList.Free();
        }

        return childHandles;

        // Create the callback
        static bool Callback(nint windowHandle, nint lParam)
        {
            var gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
                return false;

            var childHandles = gcChildhandlesList.Target as List<nint>;
            childHandles?.Add(windowHandle);

            return true;
        }
    }


    /// <summary>
    /// Enumerates all top-level windows on the screen. This function does not search child windows.
    /// </summary>
    /// <returns>A collection of handles of top-level windows.</returns>
    public static IEnumerable<nint> EnumTopLevelWindows() => EnumChildWindows(nint.Zero);

    /// <summary>
    /// Flashes the specified window one time. It does not change the active state of the window.
    /// To flash the window a specified number of times, use the <see cref="FlashWindowEx(nint, FlashWindowFlags, uint, TimeSpan)"/> function.
    /// </summary>
    /// <param name="windowHandle">A handle to the window to be flashed. The window can be either open or minimized.</param>
    /// <returns>
    /// The return value specifies the window's state before the call to the <see cref="FlashWindow"/> function.
    /// If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value is zero.
    /// </returns>
    public static bool FlashWindow(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Flash the window
        return NativeMethods.FlashWindow(windowHandle, true);
    }


    /// <summary>
    /// Flashes the specified window. It does not change the active state of the window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
    /// <param name="flags">The flash status.</param>
    /// <param name="count">The number of times to flash the window.</param>
    /// <param name="timeout">The rate at which the window is to be flashed.</param>
    public static void FlashWindowEx(nint windowHandle, FlashWindowFlags flags, uint count, TimeSpan timeout)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Create the data structure
        var flashInfo = new FlashInfo
        {
            Size    = Marshal.SizeOf(typeof(FlashInfo)),
            Hwnd    = windowHandle,
            Flags   = flags,
            Count   = count,
            Timeout = Convert.ToInt32(timeout.TotalMilliseconds)
        };

        // Flash the window
        NativeMethods.FlashWindowEx(ref flashInfo);
    }

    /// <summary>
    /// Flashes the specified window. It does not change the active state of the window. The function uses the default cursor blink rate.
    /// </summary>
    /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
    /// <param name="flags">The flash status.</param>
    /// <param name="count">The number of times to flash the window.</param>
    public static void FlashWindowEx(nint windowHandle, FlashWindowFlags flags, uint count) => FlashWindowEx(windowHandle, flags, count, TimeSpan.FromMilliseconds(0));

    /// <summary>
    /// Flashes the specified window. It does not change the active state of the window. The function uses the default cursor blink rate.
    /// </summary>
    /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
    /// <param name="flags">The flash status.</param>
    public static void FlashWindowEx(nint windowHandle, FlashWindowFlags flags) => FlashWindowEx(windowHandle, flags, 0);

    /// <summary>
    /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
    /// To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx function.
    /// </summary>
    /// <param name="key">
    /// The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter.
    /// </param>
    /// <param name="translation">
    /// The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
    /// </param>
    /// <returns>
    /// The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType.
    /// If there is no translation, the return value is zero.
    /// </returns>
    public static uint MapVirtualKey(uint key, TranslationTypes translation) => NativeMethods.MapVirtualKey(key, translation);

    /// <summary>
    /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
    /// To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx function.
    /// </summary>
    /// <param name="key">
    /// The virtual key code for a key. How this value is interpreted depends on the value of the uMapType parameter.
    /// </param>
    /// <param name="translation">
    /// The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
    /// </param>
    /// <returns>
    /// The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType.
    /// If there is no translation, the return value is zero.
    /// </returns>
    public static uint MapVirtualKey(Keys key, TranslationTypes translation) => MapVirtualKey((uint)key, translation);

    /// <summary>
    /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
    /// </summary>
    /// <param name="windowHandle">A handle to the window whose window procedure is to receive the message. The following values have special meanings.</param>
    /// <param name="message">The message to be posted.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    public static void PostMessage(nint windowHandle, uint message, nint wParam, nint lParam)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Post the message
        if (!NativeMethods.PostMessage(windowHandle, message, wParam, lParam))
            throw new Win32Exception($"Couldn't post the message '{message}'.");
    }

    /// <summary>
    /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
    /// </summary>
    /// <param name="windowHandle">A handle to the window whose window procedure is to receive the message. The following values have special meanings.</param>
    /// <param name="message">The message to be posted.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    public static void PostMessage(nint windowHandle, WindowsMessages message, nint wParam, nint lParam) => PostMessage(windowHandle, (uint)message, wParam, lParam);

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="inputs">An array of <see cref="Input"/> structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
    public static void SendInput(Input[]? inputs)
    {
        // Check if the array passed in parameter is not empty
        if (inputs != null && inputs.Length != 0)
        {
            if (NativeMethods.SendInput(inputs.Length, inputs, MarshalType<Input>.Size) == 0)
                throw new Win32Exception("Couldn't send the inputs.");
        }
        else
            throw new ArgumentException("The parameter cannot be null or empty.", nameof(inputs));
    }

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="input">A structure represents an event to be inserted into the keyboard or mouse input stream.</param>
    public static void SendInput(Input input) => SendInput([input]);

    /// <summary>
    /// Sends the specified message to a window or windows.
    /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    /// </summary>
    /// <param name="windowHandle">A handle to the window whose window procedure will receive the message.</param>
    /// <param name="message">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    public static nint SendMessage(nint windowHandle, uint message, nint wParam, nint lParam)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Send the message
        return NativeMethods.SendMessage(windowHandle, message, wParam, lParam);
    }

    /// <summary>
    /// Sends the specified message to a window or windows.
    /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    /// </summary>
    /// <param name="windowHandle">A handle to the window whose window procedure will receive the message.</param>
    /// <param name="message">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    public static nint SendMessage(nint windowHandle, WindowsMessages message, nint wParam, nint lParam) => SendMessage(windowHandle, (uint)message, wParam, lParam);

    /// <summary>
    /// Brings the thread that created the specified window into the foreground and activates the window.
    /// The window is restored if minimized. Performs no action if the window is already activated.
    /// </summary>
    /// <param name="windowHandle">A handle to the window that should be activated and brought to the foreground.</param>
    /// <returns>
    /// If the window was brought to the foreground, the return value is <c>true</c>, otherwise the return value is <c>false</c>.
    /// </returns>
    public static void SetForegroundWindow(nint windowHandle)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // If the window is already activated, do nothing
        if (GetForegroundWindow() == windowHandle)
            return;

        // Restore the window if minimized
        ShowWindow(windowHandle, WindowStates.Restore);

        // Activate the window
        if(!NativeMethods.SetForegroundWindow(windowHandle))
            throw new ApplicationException("Couldn't set the window to foreground.");
    }

    /// <summary>
    /// Sets the current position and size of the specified window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <param name="left">The x-coordinate of the upper-left corner of the window.</param>
    /// <param name="top">The y-coordinate of the upper-left corner of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <param name="width">The width of the window.</param>
    public static void SetWindowPlacement(nint windowHandle, int left, int top, int height, int width)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Get a WindowPlacement structure of the current window
        var placement = GetWindowPlacement(windowHandle);

        // Set the values
        placement.NormalPosition.Left   = left;
        placement.NormalPosition.Top    = top;
        placement.NormalPosition.Height = height;
        placement.NormalPosition.Width  = width;

        // Set the window placement
        SetWindowPlacement(windowHandle, placement);
    }

    /// <summary>
    /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <param name="placement">A pointer to the <see cref="WindowPlacement"/> structure that specifies the new show state and window positions.</param>
    public static void SetWindowPlacement(nint windowHandle, WindowPlacement placement)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // If the debugger is attached and the state of the window is ShowDefault, there's an issue where the window disappears
        if (Debugger.IsAttached && placement.ShowCmd == WindowStates.ShowNormal)
            placement.ShowCmd = WindowStates.Restore;

        // Set the window placement
        if (!NativeMethods.SetWindowPlacement(windowHandle, ref placement))
            throw new Win32Exception("Couldn't set the window placement.");
    }

    /// <summary>
    /// Sets the text of the specified window's title bar.
    /// </summary>
    /// <param name="windowHandle">A handle to the window whose text is to be changed.</param>
    /// <param name="title">The new title text.</param>
    public static void SetWindowText(nint windowHandle, string title)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Set the text of the window's title bar
        if (!NativeMethods.SetWindowText(windowHandle, title))
            throw new Win32Exception("Couldn't set the text of the window's title bar.");
    }

    /// <summary>
    /// Sets the specified window's show state.
    /// </summary>
    /// <param name="windowHandle">A handle to the window.</param>
    /// <param name="state">Controls how the window is to be shown.</param>
    /// <returns>If the window was previously visible, the return value is <c>true</c>, otherwise the return value is <c>false</c>.</returns>
    public static bool ShowWindow(nint windowHandle, WindowStates state)
    {
        // Check if the handle is valid
        HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

        // Change the state of the window
        return NativeMethods.ShowWindow(windowHandle, state);
    }
}