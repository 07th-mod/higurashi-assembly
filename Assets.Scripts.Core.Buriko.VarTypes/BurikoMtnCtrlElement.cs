using Assets.Scripts.Core.Buriko.Util;
using Assets.Scripts.Core.Scene;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Core.Buriko.VarTypes
{
	internal class BurikoMtnCtrlElement : IBurikoObject
	{
		public List<MtnCtrlElement> Elements;

		public void Create(int members)
		{
			Elements = new List<MtnCtrlElement>();
			for (int i = 0; i < members; i++)
			{
				Elements.Add(new MtnCtrlElement());
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
			MtnCtrlElement mtnCtrlElement = Elements[num];
			if (reference.Reference == null)
			{
				BurikoReference burikoReference = var.VariableValue();
				BurikoMtnCtrlElement burikoMtnCtrlElement = BurikoMemory.Instance.GetMemory(burikoReference.Property) as BurikoMtnCtrlElement;
				if (burikoMtnCtrlElement == null)
				{
					throw new Exception("Attempting to set MtnCtrlElement with a variable that is not also a MtnCtrlElement!");
				}
				int num2 = burikoReference.Member;
				if (num2 == -1)
				{
					num2 = 0;
				}
				Elements[num].CopyFrom(burikoMtnCtrlElement.GetElement(num2));
				return;
			}
			BurikoReference reference2 = reference.Reference;
			switch (reference2.Property)
			{
			case "lPoints":
				mtnCtrlElement.Points = var.IntValue();
				break;
			case "lAngle":
				mtnCtrlElement.Angle = var.IntValue();
				break;
			case "lTransparency":
				mtnCtrlElement.Transparancy = var.IntValue();
				break;
			case "lStyleOfMovements":
				mtnCtrlElement.StyleOfMovement = var.IntValue();
				break;
			case "lStyleOfRotating":
				mtnCtrlElement.StyleOfRotation = var.IntValue();
				break;
			case "lCount":
				mtnCtrlElement.Time = var.IntValue();
				break;
			case "astvRoute":
			{
				int member = reference2.Member;
				BurikoReference reference3 = reference2.Reference;
				switch (reference3.Property)
				{
				case "lX":
					mtnCtrlElement.Route[member].x = var.IntValue();
					break;
				case "lY":
					mtnCtrlElement.Route[member].y = var.IntValue();
					break;
				case "lZ":
					mtnCtrlElement.Route[member].z = var.IntValue();
					break;
				default:
					Logger.LogError("Invalid property of MtnCtrlElement.astvRoute " + reference3.Property);
					break;
				}
				break;
			}
			default:
				Logger.LogError("Invalid property of MtnCtrlElement " + reference2.Property);
				break;
			}
		}

		public string GetObjectType()
		{
			return "MtnCtrlElement";
		}

		public void Serialize(MemoryStream ms)
		{
			using (BsonWriter bsonWriter = new BsonWriter(ms)
			{
				CloseOutput = false
			})
			{
				bsonWriter.CloseOutput = false;
				new JsonSerializer().Serialize(bsonWriter, Elements);
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
				Elements = jsonSerializer.Deserialize<List<MtnCtrlElement>>(reader);
			}
		}

		public string StringValue(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public int IntValue(BurikoReference reference)
		{
			throw new NotImplementedException();
		}

		public MtnCtrlElement GetElement(int id)
		{
			return Elements[id];
		}

		public MtnCtrlElement[] GetAllElements()
		{
			return Elements.ToArray();
		}
	}
}
