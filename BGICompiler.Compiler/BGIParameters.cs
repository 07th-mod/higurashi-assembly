using Antlr.Runtime.Tree;
using Assets.Scripts.Core.Buriko;
using System.Collections.Generic;
using UnityEngine;

namespace BGICompiler.Compiler
{
	public class BGIParameters
	{
		private readonly List<BGIValue> values = new List<BGIValue>();

		public int Count => values.Count;

		public BGIParameters()
		{
		}

		public BGIParameters(ITree tree)
		{
			for (int i = 0; i < tree.ChildCount; i++)
			{
				values.Add(new BGIValue(tree.GetChild(i)));
			}
		}

		public BGIValue GetParam(int i)
		{
			return values[i];
		}

		public void OutputParam(int i)
		{
			values[i].Output();
		}

		public void OutputAllParams()
		{
			foreach (BGIValue value in values)
			{
				value.Output();
			}
		}

		public bool CheckParamSig(string sig)
		{
			int num = 0;
			if (sig.Length != values.Count)
			{
				return false;
			}
			foreach (char c in sig)
			{
				if (values[num].Type == BurikoValueType.Null)
				{
					num++;
					continue;
				}
				switch (c)
				{
				case 'i':
					if (values[num].Type != BurikoValueType.Int && values[num].Type != BurikoValueType.Bool && values[num].Type != BurikoValueType.Variable && values[num].Type != BurikoValueType.Operation && values[num].Type != BurikoValueType.Math && values[num].Type != BurikoValueType.Unary)
					{
						Debug.LogWarning(values[num].Type);
						return false;
					}
					break;
				case 's':
					if (values[num].Type != BurikoValueType.String && values[num].Type != BurikoValueType.Variable)
					{
						return false;
					}
					break;
				case 'b':
					if (values[num].Type != BurikoValueType.Bool)
					{
						return false;
					}
					break;
				case 'v':
					if (values[num].Type != BurikoValueType.Variable)
					{
						return false;
					}
					break;
				default:
					return false;
				}
				num++;
			}
			return true;
		}
	}
}
