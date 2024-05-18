using System;

namespace UMP.Wrappers
{
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false)]
	internal sealed class NativeFunctionAttribute : Attribute
	{
		public string FunctionName
		{
			get;
			private set;
		}

		public NativeFunctionAttribute(string functionName)
		{
			FunctionName = functionName;
		}
	}
}
