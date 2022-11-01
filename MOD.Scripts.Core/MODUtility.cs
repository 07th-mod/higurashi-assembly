using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

/// <summary>
/// Just a bunch of convenience functions
/// </summary>
public static class MODUtility
{
	public enum Platform
	{
		Windows,
		MacOS,
		Linux
	}

	/// <summary>
	/// Merges another dictionary into self, overwriting any duplicate keys
	/// </summary>
	public static void MergeOverwrite<Key, Value>(this Dictionary<Key, Value> self, IEnumerable<KeyValuePair<Key, Value>> other)
	{
		foreach (var item in other)
		{
			self[item.Key] = item.Value;
		}
	}

	public static void FlagMonitorOnlyLog(string str)
	{
		if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() != 0)
		{
			Debug.Log(str);
		}
	}

	[DllImport("Retinaizer", EntryPoint = "goRetina")]
	private static extern void CGoRetina();

	public static void GoRetina()
	{
		try
		{
			CGoRetina();
		}
		catch (System.DllNotFoundException)
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				Debug.Log("Not attempting Retina, no library");
			}
		}
	}

	public static string CombinePaths(params string[] paths)
	{
		return paths.Aggregate((lhs, rhs) => System.IO.Path.Combine(lhs, rhs));
	}

	/// <returns>True if is 2000 series unity, False if is old unity versioning (5.x, 4.x etc)</returns>
	public static bool IsUnity2000()
	{
		if (GetUnityVersionMajor() is int version)
		{
			return version > 2000;
		}

		return false;
	}

	/// <summary>
	/// Gets the major part of the unity version, eg 2018, 2017, 5, 4, 3, as an integer
	/// </summary>
	/// <returns>Returns the major unity version as an int - returns null if version can't be determined</returns>
	public static int? GetUnityVersionMajor()
	{
		string[] versions = Application.unityVersion.Split('.');
		if (versions.Length > 0 && int.TryParse(versions[0], out int version))
		{
			return version;
		}

		return null;
	}

	public static Platform GetPlatform()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.IPhonePlayer:
				return Platform.MacOS;

			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				return Platform.Windows;

			case RuntimePlatform.LinuxPlayer:
				return Platform.Linux;

			// All other platforms are not possible for Higurashi, just assume windows
			default:
				return Platform.Windows;
		}
	}

	public static string StripTabs(string s) => s.Replace("\t", "");

	public static string InformationalVersion()
	{
		try
		{
			return Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
		catch(Exception e)
		{
			Debug.Log(e);
			return "Unknown Version";
		}
	}

	/// <summary>
	/// On Windows, will open a new Explorer window with the file highlighted
	/// On Other OS, *might* show the folder containing the file, but also might not work
	/// </summary>
	public static void ShowInFolder(string pathToshow)
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			// Explorer doesn't like it if you use forward slashes in path
			string backslashOnlyPath = pathToshow.Replace("/", "\\");
			string arguments = $"/select, \"{backslashOnlyPath}\"";
			string executable = "explorer.exe";
			Debug.Log($"Executing [{executable} {arguments}]");
			System.Diagnostics.Process.Start(executable, arguments);
		}
		else
		{
			string folderToOpen = Path.GetDirectoryName(pathToshow);
			Debug.Log($"Opening Folder [{folderToOpen}]");
			Application.OpenURL(folderToOpen);
		}
	}

	/// <summary>
	/// Some of the linux players have a bug where the window resize function passes uninitialized stack data to X11
	/// </summary>
	/// <returns>Whether this unity version is has a broken resize function</returns>
	public static bool HasBrokenWindowResize()
	{
		if (Application.platform != RuntimePlatform.LinuxPlayer)
		{
			return false;
		}
		string version_string = new string(Application.unityVersion.TakeWhile(x => (x >= '0' && x <= '9') || x == '.').ToArray());
		Version version = new Version(version_string);

		// 5.5.3 is broken, 5.6.7 is not.
		// There are no higurashi games with versions between those, but the patch code should do nothing if it can't find anything.
		bool is_broken = version < new Version(5, 6, 7);
		Debug.Log($"Detected Unity {version}, which has {(is_broken ? "broken" : "working")} window resize");
		return is_broken;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct XSizeHints
	{
		public static readonly long FlagMinSize = 1 << 4;
		public static readonly long FlagMaxSize = 1 << 5;
		public IntPtr flags;
		public int x, y;
		public int width, height;
		public int min_width, min_height;
		public int max_width, max_height;
		public int width_inc, height_inc;
		public int min_aspect_x, min_aspect_y;
		public int max_aspect_x, max_aspect_y;
		public int base_width, base_height;
		public int win_gravity;
	}

	private enum XReturn : int
	{
		Success = 0,
		BadRequest = 1,
		BadValue = 2,
		BadWindow = 3,
	}

	[DllImport("libX11")]
	private unsafe static extern void XGetWMNormalHints(IntPtr display, IntPtr window, out XSizeHints hints, out IntPtr flags);
	[DllImport("libX11")]
	private static extern void XSetWMNormalHints(IntPtr display, IntPtr window, ref XSizeHints hints);
	[DllImport("libX11")]
	private static extern IntPtr XAllocSizeHints();
	[DllImport("libX11")]
	private static extern void XFree(IntPtr ptr);
	[DllImport("libX11")]
	private static extern IntPtr XOpenDisplay(IntPtr name);
	[DllImport("libX11")]
	private static extern void XCloseDisplay(IntPtr display);
	[DllImport("libX11")]
	private static extern void XFlush(IntPtr display);
	[DllImport("libX11")]
	private static extern int XScreenCount(IntPtr display);
	[DllImport("libX11")]
	private static extern IntPtr XRootWindow(IntPtr display, int screen);
	[DllImport("libX11")]
	private static extern XReturn XResizeWindow(IntPtr display, IntPtr window, int width, int height);
	[DllImport("libX11")]
	private static extern void XQueryTree(IntPtr display, IntPtr window, out IntPtr window_out, out IntPtr parent, out IntPtr children, out uint nchildren);

	private static void X11GetChildWindows(IntPtr display, IntPtr window, ref List<IntPtr> output)
	{
		XQueryTree(display, window, out _, out _, out IntPtr ichildren, out uint nchildren);
		unsafe
		{
			IntPtr* children = (IntPtr*)ichildren;
			for (uint j = 0; j < nchildren; j++)
			{
				output.Add(children[j]);
				X11GetChildWindows(display, children[j], ref output);
			}
			XFree(ichildren);
		}
	}

	private static IntPtr[] X11GetWindows(IntPtr display)
	{
		List<IntPtr> windows = new List<IntPtr>();
		int nscreens = XScreenCount(display);
		for (int i = 0; i < nscreens; i++)
		{
			IntPtr root = XRootWindow(display, i);
			X11GetChildWindows(display, root, ref windows);
		}
		return windows.ToArray();
	}

	/// <summary>
	/// Unity won't tell us what the main window is, so we have to search for ourselves
	/// </summary>
	private static IntPtr? X11GetLikelyMainWindow(IntPtr display)
	{
		IntPtr[] windows = X11GetWindows(display);
		if (windows.Length == 0)
		{
			return null;
		}
		foreach (IntPtr window in windows)
		{
			XGetWMNormalHints(display, window, out XSizeHints hints, out IntPtr flags);
			if (((int)hints.flags & (XSizeHints.FlagMaxSize | XSizeHints.FlagMinSize)) != 0)
			{
				// Unity will set a minsize and maxsize, so expect the main window to have them
				return window;
			}
		}
		Debug.Log($"X11: Couldn't find any windows with MinSize or MaxSize (returning window 0 of {windows.Length})");
		return windows[0];
	}

	public static void X11ManualSetWindowSize(int width, int height)
	{
		Debug.Log($"X11: Resizing to {width}x{height}");
		IntPtr display = XOpenDisplay(IntPtr.Zero);
		if (X11GetLikelyMainWindow(display) is IntPtr window)
		{
			XGetWMNormalHints(display, window, out XSizeHints hints, out _);
			hints.flags = (IntPtr)((long)hints.flags | XSizeHints.FlagMinSize | XSizeHints.FlagMaxSize);
			hints.min_width = width;
			hints.max_width = width;
			hints.min_height = height;
			hints.max_height = height;
			XSetWMNormalHints(display, window, ref hints);
			XResizeWindow(display, window, width, height);
			XFlush(display);
		}
		XCloseDisplay(display);
	}
}
