namespace Newtonsoft.Json.ObservableSupport
{
	public class AddingNewEventArgs
	{
		public object NewObject
		{
			get;
			set;
		}

		public AddingNewEventArgs()
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			NewObject = newObject;
		}
	}
}
