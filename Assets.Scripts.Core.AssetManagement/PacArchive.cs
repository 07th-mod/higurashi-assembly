using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.AssetManagement
{
	public class PacArchive
	{
		public string Name;

		private string path;

		public Dictionary<string, PacEntity> EntityList = new Dictionary<string, PacEntity>();

		public byte[] GetPacFile(string filename, bool encode, bool compress)
		{
			Debug.Log(filename);
			EntityList.TryGetValue(filename.ToLower(), out PacEntity value);
			if (value == null)
			{
				Logger.LogError("Could not find archive " + filename + " in pac file " + Name);
				return new byte[0];
			}
			byte[] array;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					fileStream.Seek(value.Offset, SeekOrigin.Begin);
					array = binaryReader.ReadBytes(value.Size);
				}
			}
			Debug.Log($"GetPacFile {filename} size {array.Length}");
			if (encode)
			{
				MGHelper.KeyEncode(array);
			}
			if (compress)
			{
				return CLZF2.Decompress(array);
			}
			return array;
		}

		public string[] GetPacFileList()
		{
			return EntityList.Keys.ToArray();
		}

		public PacArchive(string filename)
		{
			Name = filename;
			path = Path.Combine(MGHelper.GetDataPath(), filename);
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						if (new string(binaryReader.ReadChars(4)) != "MGPK")
						{
							throw new FileLoadException("File is not a valid pac file.");
						}
						if (binaryReader.ReadInt32() != 1)
						{
							throw new FileLoadException("Cannot read from pac archive! Incorrect archive version.");
						}
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							fileStream.Seek(12 + 48 * i, SeekOrigin.Begin);
							string text = binaryReader.ReadString();
							fileStream.Seek(12 + 48 * i + 32, SeekOrigin.Begin);
							int offset = binaryReader.ReadInt32();
							int size = binaryReader.ReadInt32();
							PacEntity value = new PacEntity(text.ToLower(), offset, size);
							EntityList.Add(text.ToLower(), value);
						}
					}
				}
			}
			catch (Exception arg)
			{
				Logger.LogError($"Failed to open PacArchive {filename}!\nException: {arg}");
			}
		}
	}
}
