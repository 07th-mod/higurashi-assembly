using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq
{
	internal class JPath
	{
		private readonly string _expression;

		private int _currentIndex;

		public List<object> Parts
		{
			get;
			private set;
		}

		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			_expression = expression;
			Parts = new List<object>();
			ParseMain();
		}

		private void ParseMain()
		{
			int num = _currentIndex;
			bool flag = false;
			while (_currentIndex < _expression.Length)
			{
				char c = _expression[_currentIndex];
				switch (c)
				{
				case '(':
				case '[':
					if (_currentIndex > num)
					{
						string item2 = _expression.Substring(num, _currentIndex - num);
						Parts.Add(item2);
					}
					ParseIndexer(c);
					num = _currentIndex + 1;
					flag = true;
					break;
				case ')':
				case ']':
					throw new Exception("Unexpected character while parsing path: " + c);
				case '.':
					if (_currentIndex > num)
					{
						string item = _expression.Substring(num, _currentIndex - num);
						Parts.Add(item);
					}
					num = _currentIndex + 1;
					flag = false;
					break;
				default:
					if (flag)
					{
						throw new Exception("Unexpected character following indexer: " + c);
					}
					break;
				}
				_currentIndex++;
			}
			if (_currentIndex > num)
			{
				string item3 = _expression.Substring(num, _currentIndex - num);
				Parts.Add(item3);
			}
		}

		private void ParseIndexer(char indexerOpenChar)
		{
			_currentIndex++;
			char c = (indexerOpenChar != '[') ? ')' : ']';
			int currentIndex = _currentIndex;
			int num = 0;
			bool flag = false;
			while (_currentIndex < _expression.Length)
			{
				char c2 = _expression[_currentIndex];
				if (char.IsDigit(c2))
				{
					num++;
					_currentIndex++;
					continue;
				}
				if (c2 == c)
				{
					flag = true;
					break;
				}
				throw new Exception("Unexpected character while parsing path indexer: " + c2);
			}
			if (!flag)
			{
				throw new Exception("Path ended with open indexer. Expected " + c);
			}
			if (num == 0)
			{
				throw new Exception("Empty path indexer.");
			}
			string value = _expression.Substring(currentIndex, num);
			Parts.Add(Convert.ToInt32(value, CultureInfo.InvariantCulture));
		}

		internal JToken Evaluate(JToken root, bool errorWhenNoMatch)
		{
			JToken jToken = root;
			foreach (object part in Parts)
			{
				string text = part as string;
				if (text != null)
				{
					JObject jObject = jToken as JObject;
					if (jObject == null)
					{
						if (errorWhenNoMatch)
						{
							throw new Exception("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, text, jToken.GetType().Name));
						}
						return null;
					}
					jToken = jObject[text];
					if (jToken == null && errorWhenNoMatch)
					{
						throw new Exception("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, text));
					}
				}
				else
				{
					int num = (int)part;
					JArray jArray = jToken as JArray;
					if (jArray == null)
					{
						if (errorWhenNoMatch)
						{
							throw new Exception("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, num, jToken.GetType().Name));
						}
						return null;
					}
					if (jArray.Count <= num)
					{
						if (errorWhenNoMatch)
						{
							throw new IndexOutOfRangeException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, num));
						}
						return null;
					}
					jToken = jArray[num];
				}
			}
			return jToken;
		}
	}
}
