using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
	internal class WrapperDictionary
	{
		private readonly Dictionary<string, Type> _wrapperTypes = new Dictionary<string, Type>();

		private static string GenerateKey(Type interfaceType, Type realObjectType)
		{
			return interfaceType.Name + "_" + realObjectType.Name;
		}

		public Type GetType(Type interfaceType, Type realObjectType)
		{
			string key = GenerateKey(interfaceType, realObjectType);
			if (_wrapperTypes.ContainsKey(key))
			{
				return _wrapperTypes[key];
			}
			return null;
		}

		public void SetType(Type interfaceType, Type realObjectType, Type wrapperType)
		{
			string key = GenerateKey(interfaceType, realObjectType);
			if (_wrapperTypes.ContainsKey(key))
			{
				_wrapperTypes[key] = wrapperType;
			}
			else
			{
				_wrapperTypes.Add(key, wrapperType);
			}
		}
	}
}
