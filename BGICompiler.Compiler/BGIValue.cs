using Antlr.Runtime.Tree;
using Assets.Scripts.Core.Buriko;
using System;
using System.Globalization;
using System.IO;

namespace BGICompiler.Compiler
{
	public class BGIValue
	{
		public BurikoValueType Type;

		private ITree baseTree;

		private ITree tree;

		private BurikoMathType mathType;

		public override string ToString()
		{
			return tree.Text;
		}

		public bool GetBool()
		{
			if (Type != BurikoValueType.Bool)
			{
				throw new Exception("GetBool called for variable type " + Type);
			}
			if (tree.Text == "TRUE")
			{
				return true;
			}
			if (tree.Text == "FALSE")
			{
				return false;
			}
			throw new Exception("Unexpected type found for variable type Bool!");
		}

		public bool IsMath(string s)
		{
			switch (s)
			{
			case "==":
				mathType = BurikoMathType.Equals;
				return true;
			case "!=":
				mathType = BurikoMathType.NotEquals;
				return true;
			case "<=":
				mathType = BurikoMathType.LessThanOrEquals;
				return true;
			case ">=":
				mathType = BurikoMathType.GreaterThanOrEquals;
				return true;
			case ">":
				mathType = BurikoMathType.GreaterThan;
				return true;
			case "<":
				mathType = BurikoMathType.LessThan;
				return true;
			case "+":
				mathType = BurikoMathType.Add;
				return true;
			case "-":
				mathType = BurikoMathType.Subtract;
				return true;
			case "*":
				mathType = BurikoMathType.Multiply;
				return true;
			case "/":
				mathType = BurikoMathType.Divide;
				return true;
			case "%":
				mathType = BurikoMathType.Modulus;
				return true;
			case "&&":
				mathType = BurikoMathType.And;
				return true;
			case "||":
				mathType = BurikoMathType.Or;
				return true;
			default:
				return false;
			}
		}

		public void OutputMath(BinaryWriter output)
		{
			BGIValue bGIValue = new BGIValue(baseTree.GetChild(0));
			BGIValue bGIValue2 = new BGIValue(baseTree.GetChild(1));
			IsMath(baseTree.Text);
			output.Write((short)Type);
			output.Write((short)mathType);
			bGIValue.Output();
			bGIValue2.Output();
		}

		public void OutputUnary(BinaryWriter output)
		{
			BGIValue bGIValue = new BGIValue(baseTree.GetChild(0));
			output.Write((short)Type);
			bGIValue.Output();
		}

		public void OutputVar(BinaryWriter output)
		{
			output.Write((short)Type);
			output.Write(tree.Text);
			int num = 1;
			if (baseTree.ChildCount <= 1)
			{
				output.Write((short)2);
				output.Write(-1);
				output.Write(value: false);
				return;
			}
			ITree child = baseTree.GetChild(num);
			if (child.Text == "INDEX")
			{
				new BGIValue(child.GetChild(0)).Output();
				num++;
			}
			else
			{
				output.Write((short)2);
				output.Write(-1);
			}
			if (baseTree.ChildCount <= num)
			{
				output.Write(value: false);
				return;
			}
			ITree child2 = baseTree.GetChild(num);
			if (child2.Text == "MEMBER")
			{
				output.Write(value: true);
				new BGIValue(child2.GetChild(0)).Output();
			}
			else
			{
				output.Write(value: false);
			}
		}

		public void Output()
		{
			BinaryWriter output = BGItoMG.Instance.Output;
			switch (Type)
			{
			case BurikoValueType.Int:
			{
				int num = (!tree.Text.StartsWith("0x")) ? int.Parse(tree.Text) : int.Parse(tree.Text.Substring(2), NumberStyles.HexNumber);
				output.Write((short)Type);
				output.Write(num);
				break;
			}
			case BurikoValueType.Unary:
			{
				int num = (!tree.Text.StartsWith("0x")) ? int.Parse(tree.Text) : int.Parse(tree.Text.Substring(2), NumberStyles.HexNumber);
				output.Write((short)2);
				output.Write(-num);
				break;
			}
			case BurikoValueType.String:
			{
				string text = tree.Text;
				output.Write((short)Type);
				output.Write(text.Substring(1, text.Length - 2).Replace("\\\"", "\""));
				break;
			}
			case BurikoValueType.Bool:
				output.Write((short)Type);
				output.Write(GetBool());
				break;
			case BurikoValueType.Null:
				output.Write((short)Type);
				break;
			case BurikoValueType.Math:
				OutputMath(output);
				break;
			case BurikoValueType.Operation:
				output.Write((short)Type);
				new OperationHandler().ParseOperation(tree);
				break;
			case BurikoValueType.Variable:
				OutputVar(output);
				break;
			default:
				if (tree != null)
				{
					throw new Exception($"Unhandled Variable Type: {Type} Value: {tree.Text}");
				}
				throw new Exception($"Unhandled Variable Type: {Type} Value: NO VALUE");
			}
		}

		public BGIValue(ITree treein)
		{
			baseTree = treein;
			tree = treein.GetChild(0);
			switch (treein.Text)
			{
			case "TYPEINT":
				Type = BurikoValueType.Int;
				break;
			case "TYPEHEX":
				Type = BurikoValueType.Int;
				break;
			case "TYPESTRING":
				Type = BurikoValueType.String;
				break;
			case "TYPEBOOL":
				Type = BurikoValueType.Bool;
				break;
			case "TYPENULL":
				Type = BurikoValueType.Null;
				break;
			case "TYPEFUNCTION":
				Type = BurikoValueType.Operation;
				break;
			case "TYPEVARIABLE":
				Type = BurikoValueType.Variable;
				baseTree = tree;
				tree = baseTree.GetChild(0);
				break;
			case "TYPEMATH":
				Type = BurikoValueType.Math;
				baseTree = tree;
				tree = baseTree.GetChild(0);
				break;
			case "TYPEUNARY":
				Type = BurikoValueType.Unary;
				break;
			case "VAR":
				Type = BurikoValueType.Variable;
				break;
			}
			if (Type == BurikoValueType.None && IsMath(treein.Text))
			{
				Type = BurikoValueType.Math;
			}
		}
	}
}
