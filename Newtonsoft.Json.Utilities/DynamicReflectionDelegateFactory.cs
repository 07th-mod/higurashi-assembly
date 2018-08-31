using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Newtonsoft.Json.Utilities
{
	internal class DynamicReflectionDelegateFactory : ReflectionDelegateFactory
	{
		public static DynamicReflectionDelegateFactory Instance = new DynamicReflectionDelegateFactory();

		private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
		{
			return owner.IsInterface ? new DynamicMethod(name, returnType, parameterTypes, owner.Module, skipVisibility: true) : new DynamicMethod(name, returnType, parameterTypes, owner, skipVisibility: true);
		}

		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod(method.ToString(), typeof(object), new Type[2]
			{
				typeof(object),
				typeof(object[])
			}, method.DeclaringType);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateMethodCallIL(method, iLGenerator);
			return (MethodCall<T, object>)dynamicMethod.CreateDelegate(typeof(MethodCall<T, object>));
		}

		private void GenerateCreateMethodCallIL(MethodBase method, ILGenerator generator)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Label label = generator.DefineLabel();
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Ldc_I4, parameters.Length);
			generator.Emit(OpCodes.Beq, label);
			generator.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(Type.EmptyTypes));
			generator.Emit(OpCodes.Throw);
			generator.MarkLabel(label);
			if (!method.IsConstructor && !method.IsStatic)
			{
				generator.PushInstance(method.DeclaringType);
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Ldc_I4, i);
				generator.Emit(OpCodes.Ldelem_Ref);
				generator.UnboxIfNeeded(parameters[i].ParameterType);
			}
			if (method.IsConstructor)
			{
				generator.Emit(OpCodes.Newobj, (ConstructorInfo)method);
			}
			else if (method.IsFinal || !method.IsVirtual)
			{
				generator.CallMethod((MethodInfo)method);
			}
			Type type = (!method.IsConstructor) ? ((MethodInfo)method).ReturnType : method.DeclaringType;
			if (type != typeof(void))
			{
				generator.BoxIfNeeded(type);
			}
			else
			{
				generator.Emit(OpCodes.Ldnull);
			}
			generator.Return();
		}

		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod("Create" + type.FullName, typeof(T), Type.EmptyTypes, type);
			dynamicMethod.InitLocals = true;
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateDefaultConstructorIL(type, iLGenerator);
			return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
		}

		private void GenerateCreateDefaultConstructorIL(Type type, ILGenerator generator)
		{
			if (type.IsValueType)
			{
				generator.DeclareLocal(type);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Box, type);
			}
			else
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				if (constructor == null)
				{
					throw new Exception("Could not get constructor for {0}.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				generator.Emit(OpCodes.Newobj, constructor);
			}
			generator.Return();
		}

		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(T), new Type[1]
			{
				typeof(object)
			}, propertyInfo.DeclaringType);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateGetPropertyIL(propertyInfo, iLGenerator);
			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		private void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
			if (getMethod == null)
			{
				throw new Exception("Property '{0}' does not have a getter.".FormatWith(CultureInfo.InvariantCulture, propertyInfo.Name));
			}
			if (!getMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}
			generator.CallMethod(getMethod);
			generator.BoxIfNeeded(propertyInfo.PropertyType);
			generator.Return();
		}

		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new Type[1]
			{
				typeof(object)
			}, fieldInfo.DeclaringType);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateGetFieldIL(fieldInfo, iLGenerator);
			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		private void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
			}
			generator.Emit(OpCodes.Ldfld, fieldInfo);
			generator.BoxIfNeeded(fieldInfo.FieldType);
			generator.Return();
		}

		public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new Type[2]
			{
				typeof(T),
				typeof(object)
			}, fieldInfo.DeclaringType);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateSetFieldIL(fieldInfo, iLGenerator);
			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		internal static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
			}
			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(fieldInfo.FieldType);
			generator.Emit(OpCodes.Stfld, fieldInfo);
			generator.Return();
		}

		public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, null, new Type[2]
			{
				typeof(T),
				typeof(object)
			}, propertyInfo.DeclaringType);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			GenerateCreateSetPropertyIL(propertyInfo, iLGenerator);
			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		internal static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
			if (!setMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}
			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(propertyInfo.PropertyType);
			generator.CallMethod(setMethod);
			generator.Return();
		}
	}
}
