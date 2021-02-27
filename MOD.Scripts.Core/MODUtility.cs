using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

/// <summary>
/// Just a bunch of convenience functions
/// </summary>
public static class MODUtility
{
	/// <summary>
	/// Some flags use "0" to indicate the flag has not been chosen by the user, and values 1 onwards as the actual flag value.
	/// This class helps ensure 1-based flags are used correctly though type-checking and reusable conversion functions
	/// </summary>
	public class OneBasedFlag
	{
		public int OneBased { get; }

		public OneBasedFlag(int OneBased)
		{
			this.OneBased = OneBased;
		}

		/// <summary>
		/// Get the value of the flag if it were a 0-based value
		/// If the 1-based value is 0, this function just returns 0
		/// </summary>
		public int ZeroBased => OneBased > 0 ? OneBased - 1 : 0;

		public static OneBasedFlag FromZeroBased(int zeroBased) => new OneBasedFlag(zeroBased + 1);
	}

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
}
