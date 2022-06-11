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
	internal class BurikoInt : IBurikoObject
	{
		public List<int> intlist;

		public void Create(int members)
		{
			intlist = new List<int>();
			for (int i = 0; i < members; i++)
			{
				intlist.Add(0);
			}
		}

		public int MemberCount()
		{
			return intlist.Count;
		}

		public IEnumerator<BurikoVariable> GetObjects()
		{
			return intlist.Select((int s) => new BurikoVariable(s)).GetEnumerator();
		}

		public BurikoVariable GetObject(BurikoReference reference)
		{
			return new BurikoVariable(intlist[reference.Member]);
		}

		public void SetValue(BurikoReference reference, BurikoVariable var)
		{
			if (reference.Member <= 0)
			{
				intlist[0] = var.IntValue();
			}
			else
			{
				intlist[reference.Member] = var.IntValue();
			}
		}

		public string GetObjectType()
		{
			return "Integer";
		}

		public void Serialize(MemoryStream ms)
		{
			if (intlist == null)
			{
				throw new Exception("Cannot serialize while Stringlist is null!");
			}
			using (BsonWriter jsonWriter = new BsonWriter(ms)
			{
				CloseOutput = false
			})
			{
				new JsonSerializer().Serialize(jsonWriter, intlist);
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
				intlist = jsonSerializer.Deserialize<List<int>>(reader);
			}
		}

		public string StringValue(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public int IntValue(BurikoReference reference)
		{
			if (reference.Member <= 0)
			{
				return intlist[0];
			}
			return intlist[reference.Member];
		}
	}
}
