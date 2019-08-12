using System.Collections.Generic;

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
}
