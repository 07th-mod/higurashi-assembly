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

		// Extra check to see if we can actually call any X11 functions, as in some cases you will get a DllNotFoundException: libX11.so
		if (is_broken)
		{
			try
			{
				IntPtr display = XOpenDisplay(IntPtr.Zero);
				if (display != IntPtr.Zero)
				{
					XCloseDisplay(display);
				}
			}
			catch (DllNotFoundException e)
			{
				Debug.Log($"X11 not available - assuming window resize is not broken: {e}");
				return false;
			}
		}

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

	[DllImport("libc", SetLastError = true)]
	private static extern IntPtr readlink([MarshalAs(UnmanagedType.LPStr)] string file, IntPtr buffer, IntPtr size);
	[DllImport("libc")]
	private static extern int memcmp(IntPtr a, IntPtr b, IntPtr size);
	[DllImport("libc", SetLastError = true)]
	private static extern int mprotect(IntPtr addr, IntPtr len, int prot);
	[DllImport("libc")]
	private static extern int getpagesize();

	private static readonly byte[] BAD_FUNCTION_HEADER_X64 = new byte[]
	{
		0x48, 0x89, 0x5c, 0x24, 0xe0,             // mov qword ptr [rsp - 0x20], rbx
		0x48, 0x89, 0x6c, 0x24, 0xe8,             // mov qword ptr [rsp - 0x18], rbp
		0x48, 0x89, 0xfb,                         // mov rbx, rdi
		0x4c, 0x89, 0x64, 0x24, 0xf0,             // mov qword ptr [rsp - 0x10], r12
		0x4c, 0x89, 0x6c, 0x24, 0xf8,             // mov qword ptr [rsp - 0x08], r13
		0x48, 0x81, 0xec, 0x88, 0x00, 0x00, 0x00, // sub rsp, 0x88
		0x48, 0x83, 0x3d,                         // cmp qword ptr [rip+???], 0
	};

	private static readonly byte[] BAD_FUNCTION_HEADER_X86 = new byte[]
	{
		// x86 starts with a reference to the global variable, which changes between versions
		// We'll search for some later code instead
		// Annoyingly, different unity versions have slightly different compilations of the second instruction:
		// 0x81, 0xec, 0x8c, 0x00, 0x00, 0x00,       sub esp, 0x8c
		// 0x8b, 0x0d, 0x??, 0x??, 0x??, 0x??,       mov ecx, dword ptr [???]
		//  - or -
		// 0xa1, 0x??, 0x??, 0x??, 0x??,             mov eax, dword ptr [???]
		0x89, 0x5c, 0x24, 0x7c,                   // mov dword ptr [esp + 0x7c], ebx
		0x8b, 0x94, 0x24, 0x98, 0x00, 0x00, 0x00, // mov edx, dword ptr [esp + 0x98]
		0x8d, 0x5c, 0x24, 0x24,                   // lea ebx, [esp + 0x24]
	};

	private static string GetNameOfMainExecutable()
	{
		byte[] namebuf = new byte[256];
		unsafe
		{
			while (true)
			{
				fixed (byte* nameptr = namebuf)
				{
					IntPtr sz = readlink("/proc/self/exe", (IntPtr)nameptr, (IntPtr)namebuf.Length);
					if (sz == (IntPtr)namebuf.Length)
					{
						namebuf = new byte[namebuf.Length * 2];
						continue;
					}
					if (sz.ToInt64() < 0)
					{
						Debug.Log($"PatchBrokenResize: Failed to readlink /proc/self/exe: {Marshal.GetLastWin32Error()}");
						return null;
					}
					return Encoding.UTF8.GetString(namebuf, 0, (int)sz);
				}
			}
		}
	}

	private static bool FindMainExecutableMap(out IntPtr begin, out IntPtr end)
	{
		begin = IntPtr.Zero;
		end = IntPtr.Zero;
		string name = GetNameOfMainExecutable();
		if (name == null) { return false; }
		Debug.Log("PatchBrokenResize: Main executable is " + name);
		foreach (string line in File.ReadAllLines("/proc/self/maps"))
		{
			string[] sections = line.Split(new char[] {' '}, 6, StringSplitOptions.RemoveEmptyEntries);
			if (sections.Length != 6 || name != sections[5]) { continue; }
			Debug.Log($"PatchBrokenResize: Found map for main executable, address {sections[0]} perms {sections[1]}");
			long[] address = sections[0].Split('-').Select(x => long.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
			if (address.Length != 2) { continue; }
			if (!sections[1].Contains('x')) { continue; } // Looking for executable sections
			begin = (IntPtr)(begin == IntPtr.Zero ? address[0] : Math.Min(begin.ToInt64(), address[0]));
			end   = (IntPtr)(end   == IntPtr.Zero ? address[1] : Math.Max(end  .ToInt64(), address[1]));
		}
		return begin != IntPtr.Zero && end != IntPtr.Zero;
	}

	private static IntPtr? FindBadWindowResizeFunction()
	{
		unsafe
		{
			byte[] search = sizeof(IntPtr) == 8 ? BAD_FUNCTION_HEADER_X64 : BAD_FUNCTION_HEADER_X86;
			if (!FindMainExecutableMap(out IntPtr ibegin, out IntPtr iend)) { return null; }
			Debug.Log($"PatchBrokenResize: Main executable goes from {ibegin.ToInt64():x} to {iend.ToInt64():x}.");
			fixed (byte* searchp = search)
			{
				byte* begin = (byte*)ibegin;
				byte* end = (byte*)iend - search.Length;
				long fastsearch = *(long*)searchp;
				for (byte* cur = begin; cur < end; cur++)
				{
					if (fastsearch != *(long*)cur) { continue; }
					if (memcmp((IntPtr)cur, (IntPtr)searchp, (IntPtr)search.Length) == 0)
					{
						if (sizeof(IntPtr) != 8)
						{
							// Search is for data that's either 11 or 12 bytes into the function, so back up the pointer
							cur -= 11;
							if (*cur != 0x81) { cur--; } // For the 12 case
							if (*cur != 0x81)
							{
								Debug.Log($"PatchBrokenResize: resize function started with {*cur:x} instead of 81!");
								return null;
							}
						}
						Debug.Log($"PatchBrokenResize: Found broken resize function at {(long)cur:x}!");
						return (IntPtr)cur;
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Finds and patches Unity's broken window resize function to do nothing
	/// (We'll use our reimplementation of it above instead)
	/// </summary>
	public static bool PatchWindowResizeFunction()
	{
		if (FindBadWindowResizeFunction() is IntPtr func)
		{
			unsafe
			{
				long pagesize = getpagesize();
				long pagemask = pagesize - 1;
				long fp = func.ToInt64();
				long funcpage_start = fp & ~pagemask;
				long funcpage_end = (fp + 1 /* ret */ + pagemask) & ~pagemask;
				const int PROT_READ = 1;
				const int PROT_WRITE = 2;
				const int PROT_EXEC = 4;
				if (mprotect((IntPtr)funcpage_start, (IntPtr)(funcpage_end - funcpage_start), PROT_READ | PROT_WRITE) != 0)
				{
					Debug.Log($"PatchBrokenResize: Failed to mprotect window resize function: {Marshal.GetLastWin32Error()}");
					return false;
				}
				*(byte*)func = 0xc3; // replace first instruction with `ret` to prevent function from doing anything
				mprotect((IntPtr)funcpage_start, (IntPtr)(funcpage_end - funcpage_start), PROT_READ | PROT_EXEC);
				Debug.Log("PatchBrokenResize: Successfully patched Unity's broken window resize function!");
				return true;
			}
		}
		else
		{
			Debug.Log("PatchBrokenResize: Couldn't find broken window resize function!");
			return false;
		}
	}
}
