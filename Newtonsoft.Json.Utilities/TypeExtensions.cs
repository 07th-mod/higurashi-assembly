using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	internal static class TypeExtensions
	{
		public static MethodInfo GetGenericMethod(this Type type, string name, params Type[] parameterTypes)
		{
			IEnumerable<MethodInfo> enumerable = from method in type.GetMethods()
			where method.Name == name
			select method;
			foreach (MethodInfo item in enumerable)
			{
				if (item.HasParameters(parameterTypes))
				{
					return item;
				}
			}
			return null;
		}

		public static bool HasParameters(this MethodInfo method, params Type[] parameterTypes)
		{
			Type[] array = (from parameter in method.GetParameters()
			select parameter.ParameterType).ToArray();
			if (array.Length != parameterTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ToString() != parameterTypes[i].ToString())
				{
					return false;
				}
			}
			return true;
		}

		public static IEnumerable<Type> AllInterfaces(this Type target)
		{
			Type[] interfaces = target.GetInterfaces();
			foreach (Type IF in interfaces)
			{
				yield return IF;
				foreach (Type item in IF.AllInterfaces())
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<MethodInfo> AllMethods(this Type target)
		{
			List<Type> list = target.AllInterfaces().ToList();
			list.Add(target);
			return from type in list
			from method in type.GetMethods()
			select method;
		}
	}
}
