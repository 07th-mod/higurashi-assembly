using Newtonsoft.Json.Utilities;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	internal static class CachedAttributeGetter<T> where T : Attribute
	{
		private static readonly ThreadSafeStore<ICustomAttributeProvider, T> TypeAttributeCache = new ThreadSafeStore<ICustomAttributeProvider, T>(JsonTypeReflector.GetAttribute<T>);

		[CompilerGenerated]
		private static Func<ICustomAttributeProvider, T> _003C_003Ef__mg_0024cache0;

		public static T GetAttribute(ICustomAttributeProvider type)
		{
			return TypeAttributeCache.Get(type);
		}
	}
}
