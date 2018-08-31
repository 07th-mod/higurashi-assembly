using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Serialization
{
	public class DynamicValueProvider : IValueProvider
	{
		private readonly MemberInfo _memberInfo;

		private Func<object, object> _getter;

		private Action<object, object> _setter;

		public DynamicValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
			_memberInfo = memberInfo;
		}

		public void SetValue(object target, object value)
		{
			try
			{
				if (_setter == null)
				{
					_setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(_memberInfo);
				}
				_setter(target, value);
			}
			catch (Exception innerException)
			{
				throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), innerException);
				IL_0067:;
			}
		}

		public object GetValue(object target)
		{
			try
			{
				if (_getter == null)
				{
					_getter = DynamicReflectionDelegateFactory.Instance.CreateGet<object>(_memberInfo);
				}
				return _getter(target);
				IL_0033:
				object result;
				return result;
			}
			catch (Exception innerException)
			{
				throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), innerException);
				IL_006c:
				object result;
				return result;
			}
		}
	}
}
