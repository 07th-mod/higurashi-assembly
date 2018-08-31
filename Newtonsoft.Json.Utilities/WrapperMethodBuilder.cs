using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Newtonsoft.Json.Utilities
{
	internal class WrapperMethodBuilder
	{
		private readonly Type _realObjectType;

		private readonly TypeBuilder _wrapperBuilder;

		public WrapperMethodBuilder(Type realObjectType, TypeBuilder proxyBuilder)
		{
			_realObjectType = realObjectType;
			_wrapperBuilder = proxyBuilder;
		}

		public void Generate(MethodInfo newMethod)
		{
			if (newMethod.IsGenericMethod)
			{
				newMethod = newMethod.GetGenericMethodDefinition();
			}
			FieldInfo field = typeof(DynamicWrapperBase).GetField("UnderlyingObject", BindingFlags.Instance | BindingFlags.NonPublic);
			ParameterInfo[] parameters = newMethod.GetParameters();
			Type[] parameterTypes = (from parameter in parameters
			select parameter.ParameterType).ToArray();
			MethodBuilder methodBuilder = _wrapperBuilder.DefineMethod(newMethod.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, newMethod.ReturnType, parameterTypes);
			if (newMethod.IsGenericMethod)
			{
				methodBuilder.DefineGenericParameters((from arg in newMethod.GetGenericArguments()
				select arg.Name).ToArray());
			}
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			LoadUnderlyingObject(iLGenerator, field);
			PushParameters(parameters, iLGenerator);
			ExecuteMethod(newMethod, parameterTypes, iLGenerator);
			Return(iLGenerator);
		}

		private static void Return(ILGenerator ilGenerator)
		{
			ilGenerator.Emit(OpCodes.Ret);
		}

		private void ExecuteMethod(MethodBase newMethod, Type[] parameterTypes, ILGenerator ilGenerator)
		{
			MethodInfo method = GetMethod(newMethod, parameterTypes);
			if (method == null)
			{
				throw new MissingMethodException("Unable to find method " + newMethod.Name + " on " + _realObjectType.FullName);
			}
			ilGenerator.Emit(OpCodes.Call, method);
		}

		private MethodInfo GetMethod(MethodBase realMethod, Type[] parameterTypes)
		{
			if (realMethod.IsGenericMethod)
			{
				return _realObjectType.GetGenericMethod(realMethod.Name, parameterTypes);
			}
			return _realObjectType.GetMethod(realMethod.Name, parameterTypes);
		}

		private static void PushParameters(ICollection<ParameterInfo> parameters, ILGenerator ilGenerator)
		{
			for (int i = 1; i < parameters.Count + 1; i++)
			{
				ilGenerator.Emit(OpCodes.Ldarg, i);
			}
		}

		private static void LoadUnderlyingObject(ILGenerator ilGenerator, FieldInfo srcField)
		{
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldfld, srcField);
		}
	}
}
