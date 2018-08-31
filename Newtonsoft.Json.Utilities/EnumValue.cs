namespace Newtonsoft.Json.Utilities
{
	internal class EnumValue<T> where T : struct
	{
		private string _name;

		private T _value;

		public string Name => _name;

		public T Value => _value;

		public EnumValue(string name, T value)
		{
			_name = name;
			_value = value;
		}
	}
}
