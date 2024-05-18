using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UMP.Wrappers;
using UnityEngine;

namespace UMP
{
	internal class NativeInterop
	{
		private static class WindowsInterop
		{
			[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool SetDllDirectory([MarshalAs(UnmanagedType.LPStr)] string lpPathName);

			[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
			internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

			[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
			internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

			[DllImport("kernel32", SetLastError = true)]
			internal static extern bool FreeLibrary(IntPtr hModule);

			[DllImport("advapi32", SetLastError = true)]
			internal static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, int samDesired, out UIntPtr hkResult);

			[DllImport("advapi32", CharSet = CharSet.Ansi, SetLastError = true)]
			internal static extern uint RegQueryValueEx(UIntPtr hKey, [MarshalAs(UnmanagedType.LPStr)] string lpValueName, int lpReserved, out uint lpType, StringBuilder lpData, ref uint lpcbData);

			[DllImport("advapi32", SetLastError = true)]
			internal static extern int RegCloseKey(UIntPtr hKey);
		}

		private static class MacInterop
		{
			[DllImport("libdl", SetLastError = true)]
			internal static extern IntPtr dlopen(string fileName, int flags);

			[DllImport("libdl", SetLastError = true)]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);

			[DllImport("libdl")]
			internal static extern int dlclose(IntPtr handle);
		}

		private static class LinuxInterop
		{
			[DllImport("__Internal", SetLastError = true)]
			internal static extern IntPtr dlopen(string fileName, int flags);

			[DllImport("__Internal", SetLastError = true)]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);

			[DllImport("__Internal")]
			internal static extern int dlclose(IntPtr handle);
		}

		private const string LIB_WIN_KERNEL = "kernel32";

		private const string LIB_WIN_ADVAPI = "advapi32";

		private const string LIB_UNX = "libdl";

		private const string LIB_LIN = "__Internal";

		private const int LIN_RTLD_NOW = 2;

		private const string EXT_PLUGINS_FOLDER_NAME = "/vlc/plugins";

		private const string MAC_APPS_FOLDER_NAME = "/Applications";

		private const string LIN_86_APPS_FOLDER_NAME = "/usr/lib";

		private const string LIN_64_APPS_FOLDER_NAME = "/usr/lib64";

		private const string MAC_BUNDLE_NAME = "/libvlc.bundle";

		private const string MAC_PACKAGE_NAME = "/vlc.app";

		private const string MAC_PACKAGE_LIB_PATH = "/Contents/MacOS/lib";

		private const string VLC_EXT_ENV = "VLC_PLUGIN_PATH";

		private static readonly Dictionary<string, Delegate> _interopDelegates = new Dictionary<string, Delegate>();

		private static bool SetLibraryDirectory(string path)
		{
			UMPSettings.Platforms runtimePlatform = UMPSettings.RuntimePlatform;
			switch (runtimePlatform)
			{
			case UMPSettings.Platforms.Win:
				return WindowsInterop.SetDllDirectory(path);
			case UMPSettings.Platforms.Mac:
			{
				string fullName = Directory.GetParent(path.TrimEnd(Path.DirectorySeparatorChar)).FullName;
				fullName = Path.Combine(fullName, "plugins");
				if (Directory.Exists(fullName))
				{
					Environment.SetEnvironmentVariable("VLC_PLUGIN_PATH", fullName);
					return true;
				}
				break;
			}
			}
			if (runtimePlatform == UMPSettings.Platforms.Linux)
			{
				Environment.SetEnvironmentVariable("VLC_PLUGIN_PATH", path);
				Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", path);
				return true;
			}
			return false;
		}

		public static IntPtr LoadLibrary(string libName, string libraryPath)
		{
			IntPtr intPtr = IntPtr.Zero;
			string text = string.Empty;
			if (string.IsNullOrEmpty(libName) || string.IsNullOrEmpty(libraryPath))
			{
				return intPtr;
			}
			SetLibraryDirectory(libraryPath);
			string[] files = Directory.GetFiles(libraryPath);
			string[] array = files;
			foreach (string text2 in array)
			{
				if (!text2.EndsWith(".meta"))
				{
					string fileName = Path.GetFileName(text2);
					if (fileName.StartsWith(libName + ".") && (string.IsNullOrEmpty(text) || fileName.Any(char.IsDigit)))
					{
						text = fileName;
					}
				}
			}
			switch (UMPSettings.RuntimePlatform)
			{
			case UMPSettings.Platforms.Win:
				intPtr = WindowsInterop.LoadLibrary(Path.Combine(libraryPath, text));
				break;
			case UMPSettings.Platforms.Mac:
				intPtr = MacInterop.dlopen(Path.Combine(libraryPath, text), 2);
				break;
			case UMPSettings.Platforms.Linux:
				intPtr = LinuxInterop.dlopen(Path.Combine(libraryPath, text), 2);
				break;
			}
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception($"[LoadLibrary] Can't load '{libName}' library");
			}
			return intPtr;
		}

		public static T GetLibraryDelegate<T>(IntPtr handler)
		{
			//Discarded unreachable code: IL_0133, IL_0153
			string text = null;
			IntPtr intPtr = IntPtr.Zero;
			UMPSettings.Platforms runtimePlatform = UMPSettings.RuntimePlatform;
			try
			{
				object[] customAttributes = typeof(T).GetCustomAttributes(typeof(NativeFunctionAttribute), inherit: false);
				if (customAttributes.Length == 0)
				{
					throw new Exception("[GetLibraryDelegate] Could not find the native attribute type.");
				}
				NativeFunctionAttribute nativeFunctionAttribute = (NativeFunctionAttribute)customAttributes[0];
				text = nativeFunctionAttribute.FunctionName;
				if (_interopDelegates.ContainsKey(text))
				{
					return (T)Convert.ChangeType(_interopDelegates[nativeFunctionAttribute.FunctionName], typeof(T), null);
				}
				if (runtimePlatform == UMPSettings.Platforms.Win)
				{
					intPtr = WindowsInterop.GetProcAddress(handler, nativeFunctionAttribute.FunctionName);
				}
				if (runtimePlatform == UMPSettings.Platforms.Mac)
				{
					intPtr = MacInterop.dlsym(handler, nativeFunctionAttribute.FunctionName);
				}
				if (runtimePlatform == UMPSettings.Platforms.Linux)
				{
					intPtr = LinuxInterop.dlsym(handler, nativeFunctionAttribute.FunctionName);
				}
				if (intPtr == IntPtr.Zero)
				{
					throw new Exception($"[GetLibraryDelegate] Can't get process address from '{handler}'");
				}
				Delegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(intPtr, typeof(T));
				_interopDelegates[nativeFunctionAttribute.FunctionName] = delegateForFunctionPointer;
				return (T)Convert.ChangeType(delegateForFunctionPointer, typeof(T), null);
			}
			catch (Exception inner)
			{
				throw new MissingMethodException($"[GetLibraryDelegate] The address of the function '{text}' does not exist in '{handler}' library.", inner);
			}
		}

		public static bool FreeLibrary(IntPtr handler)
		{
			switch (UMPSettings.RuntimePlatform)
			{
			case UMPSettings.Platforms.Win:
				return WindowsInterop.FreeLibrary(handler);
			case UMPSettings.Platforms.Mac:
				return MacInterop.dlclose(handler) == 0;
			case UMPSettings.Platforms.Linux:
				return LinuxInterop.dlclose(handler) == 0;
			default:
				return false;
			}
		}

		public static string ReadLocalRegKey(string keyPath, string valueName)
		{
			UMPSettings.Platforms runtimePlatform = UMPSettings.RuntimePlatform;
			string result = string.Empty;
			if (runtimePlatform == UMPSettings.Platforms.Win)
			{
				UIntPtr hKey = new UIntPtr(2147483650u);
				int samDesired = 131097;
				UIntPtr hkResult = UIntPtr.Zero;
				if (WindowsInterop.RegOpenKeyEx(hKey, keyPath, 0, samDesired, out hkResult) == 0)
				{
					uint lpcbData = 1024u;
					StringBuilder stringBuilder = new StringBuilder((int)lpcbData);
					if (WindowsInterop.RegQueryValueEx(hkResult, valueName, 0, out uint _, stringBuilder, ref lpcbData) == 0)
					{
						result = stringBuilder.ToString();
					}
					else
					{
						Debug.LogWarning($"[ReadLocalRegKey] Can't read local reg key value: '{valueName}'");
					}
					WindowsInterop.RegCloseKey(hkResult);
				}
			}
			return result;
		}
	}
}
