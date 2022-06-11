using Assets.Scripts.Core.AssetManagement;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class FragmentData
{
	public static List<FragmentDataEntry> FragmentDataList;

	public static void Initialize()
	{
		FragmentDataList = JsonConvert.DeserializeObject<List<FragmentDataEntry>>(AssetManager.Instance.LoadTextDataString("fragmentdata.txt"));
	}
}
