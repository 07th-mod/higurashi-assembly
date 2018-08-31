using Assets.Scripts.Core.Buriko.Util;
using System;
using System.Globalization;
using System.IO;

namespace Assets.Scripts.Core.Buriko
{
	public class BurikoVariable
	{
		private string valueString;

		private int valueInt;

		private BurikoReference valueReference;

		private BurikoVariable valueVariable;

		public BurikoValueType Type
		{
			get;
			private set;
		}

		public static BurikoVariable Null => new BurikoVariable();

		public BurikoVariable()
		{
			Type = BurikoValueType.Null;
		}

		public BurikoVariable(string s)
		{
			Type = BurikoValueType.String;
			valueString = s;
		}

		public BurikoVariable(int i)
		{
			Type = BurikoValueType.Int;
			valueInt = i;
		}

		public BurikoVariable(BurikoReference reference, BurikoVariable variable)
		{
			Type = BurikoValueType.Variable;
			valueReference = reference;
			valueVariable = variable;
		}

		public BurikoVariable(BinaryReader stream)
		{
			Type = (BurikoValueType)stream.ReadInt16();
			switch (Type)
			{
			case BurikoValueType.Null:
				valueInt = -1;
				break;
			case BurikoValueType.Bool:
				valueInt = stream.ReadByte();
				break;
			case BurikoValueType.Int:
				valueInt = stream.ReadInt32();
				break;
			case BurikoValueType.String:
				valueString = stream.ReadString();
				break;
			case BurikoValueType.Variable:
				stream.BaseStream.Seek(-2L, SeekOrigin.Current);
				valueReference = ReadReference(stream);
				break;
			case BurikoValueType.Math:
			{
				BurikoMathType type = (BurikoMathType)stream.ReadInt16();
				BurikoVariable a = new BurikoVariable(stream);
				BurikoVariable b = new BurikoVariable(stream);
				Type = BurikoValueType.Int;
				valueInt = PerformMath(type, a, b);
				break;
			}
			case BurikoValueType.Operation:
				valueVariable = BurikoScriptSystem.Instance.GetCurrentScript().ExecuteOperation((BurikoOperations)stream.ReadInt16());
				break;
			default:
				throw new NotImplementedException("BurikoVariable: Unhandled BurikoValueType " + Type);
			}
		}

		public static BurikoReference ReadReference(BinaryReader dataReader)
		{
			short num = dataReader.ReadInt16();
			if (num != 5)
			{
				throw new Exception("Cannot perform assignment to an object that is not a declared variable.");
			}
			string property = dataReader.ReadString();
			int member = new BurikoVariable(dataReader).IntValue();
			bool flag = dataReader.ReadBoolean();
			BurikoReference burikoReference = new BurikoReference(property, member);
			if (flag)
			{
				burikoReference.Reference = ReadReference(dataReader);
			}
			return burikoReference;
		}

		private static int PerformMath(BurikoMathType type, BurikoVariable a, BurikoVariable b)
		{
			int num = a.IntValue();
			int num2 = b.IntValue();
			switch (type)
			{
			case BurikoMathType.Equals:
				return (num == num2) ? 1 : 0;
			case BurikoMathType.Add:
				return num + num2;
			case BurikoMathType.Divide:
				return num / num2;
			case BurikoMathType.Multiply:
				return num * num2;
			case BurikoMathType.Subtract:
				return num - num2;
			case BurikoMathType.GreaterThan:
				return (num > num2) ? 1 : 0;
			case BurikoMathType.GreaterThanOrEquals:
				return (num >= num2) ? 1 : 0;
			case BurikoMathType.LessThan:
				return (num < num2) ? 1 : 0;
			case BurikoMathType.LessThanOrEquals:
				return (num <= num2) ? 1 : 0;
			case BurikoMathType.Modulus:
				return num % num2;
			case BurikoMathType.NotEquals:
				return (num != num2) ? 1 : 0;
			default:
				throw new Exception("Cannot find a handler for math type " + type);
			}
		}

		public bool BoolValue()
		{
			switch (Type)
			{
			case BurikoValueType.Int:
			case BurikoValueType.Bool:
				if (valueInt != 0)
				{
					return true;
				}
				return false;
			case BurikoValueType.String:
				if (valueString == "0" || valueString == string.Empty)
				{
					return false;
				}
				return true;
			default:
				throw new NotImplementedException($"BurikoValue: Cannot cast type {Type} into type Bool.");
			}
		}

		public int IntValue()
		{
			switch (Type)
			{
			case BurikoValueType.Int:
			case BurikoValueType.Bool:
				return valueInt;
			case BurikoValueType.String:
				if (int.TryParse(valueString, out int result))
				{
					return result;
				}
				throw new Exception("BurikoValue: Cannot parse type string into type int.");
			case BurikoValueType.Variable:
			{
				IBurikoObject memory = BurikoMemory.Instance.GetMemory(valueReference.Property);
				return memory.IntValue(valueReference);
			}
			case BurikoValueType.Operation:
				return valueVariable.IntValue();
			default:
				throw new NotImplementedException($"BurikoValue: Cannot cast type {Type} into type Int.");
			}
		}

		public string StringValue()
		{
			switch (Type)
			{
			case BurikoValueType.Null:
				return string.Empty;
			case BurikoValueType.Int:
			case BurikoValueType.Bool:
				return valueInt.ToString(CultureInfo.InvariantCulture);
			case BurikoValueType.String:
				return valueString;
			case BurikoValueType.Variable:
				if (BurikoMemory.Instance.IsMemory(valueReference.Property))
				{
					IBurikoObject memory = BurikoMemory.Instance.GetMemory(valueReference.Property);
					return memory.GetObject(valueReference).StringValue();
				}
				if (BurikoMemory.Instance.IsFlag(valueReference.Property))
				{
					return BurikoMemory.Instance.GetFlag(valueReference.Property).StringValue();
				}
				throw new Exception("Unable to convert variable type to string!");
			case BurikoValueType.Operation:
				return valueVariable.StringValue();
			default:
				throw new NotImplementedException($"BurikoValue: Cannot cast type {Type} into type String.");
			}
		}

		public BurikoReference VariableValue()
		{
			if (Type != BurikoValueType.Variable)
			{
				throw new Exception("Cannot convert variable type " + Type + " to variable type Variable!");
			}
			return valueReference;
		}

		public string VariableName()
		{
			if (Type != BurikoValueType.Variable)
			{
				throw new Exception("Expected type variable, cannot accept type " + Type);
			}
			return valueReference.Property;
		}
	}
}
