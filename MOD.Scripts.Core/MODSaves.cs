using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public enum GlobalSaveType
{
	Vanilla,
	Modded,
	Unknown,
}

class MODSaves
{
	// NOTE: if the global save format is updated, this function must be updated accordingly
	public static GlobalSaveType GetGlobalSaveType(string path)
	{
		try
		{
			byte[] array = File.ReadAllBytes(path);
			MGHelper.KeyEncode(array);
			byte[] buffer = CLZF2.Decompress(array);

			JsonSerializer jsonSerializer = new JsonSerializer();
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using (BsonReader reader = new BsonReader(stream) { CloseInput = false })
				{
					Dictionary<int, int> globalFlags = jsonSerializer.Deserialize<Dictionary<int, int>>(reader);
				}
				using (BsonReader reader = new BsonReader(stream) { CloseInput = false, ReadRootValueAsArray = true })
				{
					List<string> cgFlags = jsonSerializer.Deserialize<List<string>>(reader);
					foreach (string cgName in cgFlags)
					{
						// If the user has seen the 07th-mod logo, then it must be a modded save
						if (cgName.ToLower().StartsWith("07th-mod"))
						{
							return GlobalSaveType.Modded;
						}
					}
				}
				using (BsonReader reader = new BsonReader(stream) { CloseInput = false })
				{
					Dictionary<string, List<int>> readText = jsonSerializer.Deserialize<Dictionary<string, List<int>>>(reader);
				}
				using (BsonReader reader = new BsonReader(stream) { CloseInput = false })
				{
					// If the save has graphics preset data, it must be a modded save
					// This was added around 2021-07 (1e89fd5397f376c44f8c0f9e72b10d29e79aa12e)
					if (jsonSerializer.Deserialize<Dictionary<string, int>>(reader) != null)
					{
						return GlobalSaveType.Modded;
					}
				}
			}
		}
		catch (Exception e)
		{
			Logger.LogError($"An exception occurred while getting save type: {e}");
			return GlobalSaveType.Unknown;
		}

		return GlobalSaveType.Vanilla;
	}
}
