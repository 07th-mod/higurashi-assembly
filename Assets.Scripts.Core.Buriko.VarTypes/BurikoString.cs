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
				Stringlist.Add("");
			}
		}

		public int MemberCount()
		{
			return Stringlist.Count;
		}

		public IEnumerator<BurikoVariable> GetObjects()
		{
			return Stringlist.Select((string s) => new BurikoVariable(s)).GetEnumerator();
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
			using (BsonWriter jsonWriter = new BsonWriter(ms)
			{
				CloseOutput = false
			})
			{
				new JsonSerializer().Serialize(jsonWriter, Stringlist);
			}
		}

		public void DeSerialize(MemoryStream ms)
		{
			using (BsonReader reader = new BsonReader(ms)
			{
				CloseInput = false,
				ReadRootValueAsArray = true
			})
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
