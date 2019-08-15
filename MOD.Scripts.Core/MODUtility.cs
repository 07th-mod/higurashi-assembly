using System.Collections.Generic;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

/// <summary>
/// Just a bunch of convenience functions
/// </summary>
public static class MODUtility
{
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
}
