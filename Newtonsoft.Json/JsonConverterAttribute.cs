using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonConverterAttribute : Attribute
	{
		private readonly Type _converterType;

		public Type ConverterType => _converterType;

		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			_converterType = converterType;
		}

		internal static JsonConverter CreateJsonConverterInstance(Type converterType)
		{
			try
			{
				return (JsonConverter)Activator.CreateInstance(converterType);
			}
			catch (Exception innerException)
			{
				throw new Exception("Error creating {0}".FormatWith(CultureInfo.InvariantCulture, converterType), innerException);
			}
		}
	}
}
