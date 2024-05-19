using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
	public class PlayerOptions
	{
		public enum States
		{
			Default = -1,
			Disable,
			Enable
		}

		private const string FIXED_SIZE_WIDTH_KEY = "fixed-size-width";

		private const string FIXED_SIZE_HEIGHT_KEY = "fixed-size-height";

		public const int DEFAULT_CACHING_VALUE = 300;

		private Dictionary<string, string> _options;

		public Vector2 FixedVideoSize
		{
			get
			{
				int value = GetValue<int>("fixed-size-width");
				int value2 = GetValue<int>("fixed-size-height");
				return new Vector2(value, value2);
			}
			set
			{
				float num = (!(value.x > 0f)) ? 0f : value.x;
				float num2 = (!(value.y > 0f)) ? 0f : value.y;
				SetValue("fixed-size-width", num.ToString());
				SetValue("fixed-size-height", num2.ToString());
			}
		}

		public PlayerOptions(string[] options)
		{
			_options = new Dictionary<string, string>();
			if (options == null)
			{
				return;
			}
			foreach (string text in options)
			{
				string[] array = text.Split(new char[1]
				{
					'='
				}, 2);
				if (array.Length > 1)
				{
					SetValue(array[0], array[1]);
				}
				else
				{
					SetValue(array[0], null);
				}
			}
		}

		public void SetValue(string key, string value)
		{
			if (!_options.ContainsKey(key))
			{
				_options.Add(key, value);
			}
			else
			{
				_options[key] = value;
			}
		}

		public string GetValue(string key)
		{
			if (_options.ContainsKey(key))
			{
				return _options[key];
			}
			return null;
		}

		public T GetValue<T>(string key)
		{
			string value = GetValue(key);
			string name = typeof(T).Name;
			if (name == typeof(bool).Name)
			{
				return (T)Convert.ChangeType(value != null, typeof(T));
			}
			if (!string.IsNullOrEmpty(value))
			{
				if (name == typeof(int).Name)
				{
					int result = 0;
					int.TryParse(value, out result);
					return (T)Convert.ChangeType(result, typeof(T));
				}
				if (name == typeof(float).Name)
				{
					float result2 = 0f;
					float.TryParse(value, out result2);
					return (T)Convert.ChangeType(result2, typeof(T));
				}
				if (name == typeof(string).Name)
				{
					return (T)Convert.ChangeType(value, typeof(T));
				}
			}
			return default(T);
		}

		public void RemoveOption(string key)
		{
			if (_options.ContainsKey(key))
			{
				_options.Remove(key);
			}
		}

		public void ClearOptions()
		{
			_options.Clear();
		}

		public string GetOptions(char separator)
		{
			string text = string.Empty;
			foreach (KeyValuePair<string, string> option in _options)
			{
				if (option.Key.StartsWith(":") || option.Key.StartsWith("-") || option.Key.StartsWith("--"))
				{
					text = (string.IsNullOrEmpty(option.Value) ? (text + option.Key) : (text + string.Format(option.Key + "={0}", option.Value)));
					text += separator;
				}
			}
			return text.Trim();
		}
	}
}
