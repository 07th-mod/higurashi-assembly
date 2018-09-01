using Assets.Scripts.Core.Buriko.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Buriko.VarTypes
{
	[Serializable]
	internal class BurikoVector : IBurikoObject
	{
		public List<Vector3> Elements;

		public void Create(int members)
		{
			Elements = new List<Vector3>();
			for (int i = 0; i < members; i++)
			{
				Elements.Add(Vector3.zero);
			}
		}

		public int MemberCount()
		{
			return Elements.Count;
		}

		public IEnumerator<BurikoVariable> GetObjects()
		{
			throw new NotImplementedException();
		}

		public BurikoVariable GetObject(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public void SetValue(BurikoReference reference, BurikoVariable var)
		{
			int num = reference.Member;
			if (num == -1)
			{
				num = 0;
			}
			Vector3 value = Elements[num];
			BurikoReference reference2 = reference.Reference;
			switch (reference2.Property)
			{
			case "lX":
				value.x = (float)var.IntValue();
				break;
			case "lY":
				value.y = (float)var.IntValue();
				break;
			case "lZ":
				value.z = (float)var.IntValue();
				break;
			default:
				Logger.LogError("Cannot set propertly " + reference2.Property + " on a ST_Vector object!");
				break;
			}
			Elements[num] = value;
		}

		public string GetObjectType()
		{
			return "Vector";
		}

		public void Serialize(MemoryStream ms)
		{
			BsonWriter bsonWriter = new BsonWriter(ms);
			bsonWriter.CloseOutput = false;
			using (BsonWriter bsonWriter2 = bsonWriter)
			{
				bsonWriter2.CloseOutput = false;
				JsonSerializer jsonSerializer = new JsonSerializer();
				jsonSerializer.Serialize(bsonWriter2, Elements);
			}
		}

		public void DeSerialize(MemoryStream ms)
		{
			BsonReader bsonReader = new BsonReader(ms);
			bsonReader.CloseInput = false;
			bsonReader.ReadRootValueAsArray = true;
			using (BsonReader reader = bsonReader)
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				Elements = jsonSerializer.Deserialize<List<Vector3>>(reader);
			}
		}

		public string StringValue(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public int IntValue(BurikoReference reference)
		{
			int num = reference.Member;
			if (num == -1)
			{
				num = 0;
			}
			Vector3 vector = Elements[num];
			BurikoReference reference2 = reference.Reference;
			switch (reference2.Property)
			{
			case "lX":
				return (int)vector.x;
			case "lY":
				return -(int)vector.y;
			case "lZ":
				return (int)vector.z;
			default:
				Logger.LogError("Cannot set propertly " + reference2.Property + " on a ST_Vector object!");
				return 0;
			}
		}

		public Vector3[] GetElements(int count)
		{
			Vector3[] array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = Elements[i];
			}
			return array;
		}
	}
}
