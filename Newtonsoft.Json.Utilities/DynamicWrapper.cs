using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

namespace Newtonsoft.Json.Utilities
{
	internal static class DynamicWrapper
	{
		private static readonly object _lock = new object();

		private static readonly WrapperDictionary _wrapperDictionary = new WrapperDictionary();

		private static ModuleBuilder _moduleBuilder;

		private static ModuleBuilder ModuleBuilder
		{
			get
			{
				Init();
				return _moduleBuilder;
			}
		}

		private static void Init()
		{
			if (_moduleBuilder == null)
			{
				lock (_lock)
				{
					if (_moduleBuilder == null)
					{
						AssemblyName assemblyName = new AssemblyName("Newtonsoft.Json.Dynamic");
						assemblyName.KeyPair = new StrongNameKeyPair(GetStrongKey());
						AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
						_moduleBuilder = assemblyBuilder.DefineDynamicModule("Newtonsoft.Json.DynamicModule", emitSymbolInfo: false);
					}
				}
			}
		}

		private static byte[] GetStrongKey()
		{
			string text = "Newtonsoft.Json.Dynamic.snk";
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text))
			{
				if (stream == null)
				{
					throw new MissingManifestResourceException("Should have " + text + " as an embedded resource.");
				}
				int num = (int)stream.Length;
				byte[] array = new byte[num];
				stream.Read(array, 0, num);
				return array;
			}
		}

		public static Type GetWrapper(Type interfaceType, Type realObjectType)
		{
			Type type = _wrapperDictionary.GetType(interfaceType, realObjectType);
			if (type == null)
			{
				lock (_lock)
				{
					type = _wrapperDictionary.GetType(interfaceType, realObjectType);
					if (type != null)
					{
						return type;
					}
					type = GenerateWrapperType(interfaceType, realObjectType);
					_wrapperDictionary.SetType(interfaceType, realObjectType, type);
					return type;
				}
			}
			return type;
		}

		public static object GetUnderlyingObject(object wrapper)
		{
			DynamicWrapperBase dynamicWrapperBase = wrapper as DynamicWrapperBase;
			if (dynamicWrapperBase == null)
			{
				throw new ArgumentException("Object is not a wrapper.", "wrapper");
			}
			return dynamicWrapperBase.UnderlyingObject;
		}

		private static Type GenerateWrapperType(Type interfaceType, Type underlyingType)
		{
			TypeBuilder typeBuilder = ModuleBuilder.DefineType("{0}_{1}_Wrapper".FormatWith(CultureInfo.InvariantCulture, interfaceType.Name, underlyingType.Name), TypeAttributes.Sealed, typeof(DynamicWrapperBase), new Type[1]
			{
				interfaceType
			});
			WrapperMethodBuilder wrapperMethodBuilder = new WrapperMethodBuilder(underlyingType, typeBuilder);
			foreach (MethodInfo item in interfaceType.AllMethods())
			{
				wrapperMethodBuilder.Generate(item);
			}
			return typeBuilder.CreateType();
		}

		public static T CreateWrapper<T>(object realObject) where T : class
		{
			Type wrapper = GetWrapper(typeof(T), realObject.GetType());
			DynamicWrapperBase dynamicWrapperBase = (DynamicWrapperBase)Activator.CreateInstance(wrapper);
			dynamicWrapperBase.UnderlyingObject = realObject;
			return dynamicWrapperBase as T;
		}
	}
}
