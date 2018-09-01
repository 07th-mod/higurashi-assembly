using Assets.Scripts.Core.Buriko.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Core.Buriko.VarTypes
{
	[Serializable]
	internal class BurikoString : IBurikoObject
	{
		public List<string> Stringlist;

		public void Create(int members)
		{
			Stringlist = new List<string>();
			for (int i = 0; i < members; i++)
			{
				Stringlist.Add(string.Empty);
			}
		}

		public int MemberCount()
		{
			return Stringlist.Count;
		}

		public IEnumerator<BurikoVariable> GetObjects()
		{
			return (from s in Stringlist
			select new BurikoVariable(s)).GetEnumerator();
		}

		public BurikoVariable GetObject(BurikoReference reference)
		{
			int num = reference.Member;
			if (num == -1)
			{
				num = 0;
			}
			return new BurikoVariable(Stringlist[num]);
		}

		public void SetValue(BurikoReference reference, BurikoVariable var)
		{
			if (reference.Member <= 0)
			{
				Stringlist[0] = var.StringValue();
			}
			else
			{
				Stringlist[reference.Member] = var.StringValue();
			}
		}

		public string GetObjectType()
		{
			return "String";
		}

		public void Serialize(MemoryStream ms)
		{
			if (Stringlist == null)
			{
				throw new Exception("Cannot serialize while Stringlist is null!");
			}
			BsonWriter bsonWriter = new BsonWriter(ms);
			bsonWriter.CloseOutput = false;
			using (BsonWriter jsonWriter = bsonWriter)
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				jsonSerializer.Serialize(jsonWriter, Stringlist);
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
				Stringlist = jsonSerializer.Deserialize<List<string>>(reader);
			}
		}

		public string StringValue(BurikoReference reference)
		{
			if (reference.Member <= 0)
			{
				return Stringlist[0];
			}
			return Stringlist[reference.Member];
		}

		public int IntValue(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public List<string> GetStringList()
		{
			return Stringlist;
		}
	}
}
