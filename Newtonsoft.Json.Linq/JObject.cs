using Newtonsoft.Json.ObservableSupport;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Newtonsoft.Json.Linq
{
	public class JObject : JContainer, IDictionary<string, JToken>, INotifyPropertyChanged, ICustomTypeDescriptor, Newtonsoft.Json.ObservableSupport.INotifyPropertyChanging, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable
	{
		private class JPropertKeyedCollection : KeyedCollection<string, JToken>
		{
			public new IDictionary<string, JToken> Dictionary => base.Dictionary;

			public JPropertKeyedCollection(IEqualityComparer<string> comparer)
				: base(comparer)
			{
			}

			protected override string GetKeyForItem(JToken item)
			{
				return ((JProperty)item).Name;
			}

			protected override void InsertItem(int index, JToken item)
			{
				if (Dictionary == null)
				{
					base.InsertItem(index, item);
				}
				else
				{
					string keyForItem = GetKeyForItem(item);
					Dictionary[keyForItem] = item;
					base.Items.Insert(index, item);
				}
			}
		}

		private JPropertKeyedCollection _properties = new JPropertKeyedCollection(StringComparer.Ordinal);

		ICollection<string> IDictionary<string, JToken>.Keys
		{
			get
			{
				return _properties.Dictionary.Keys;
			}
		}

		ICollection<JToken> IDictionary<string, JToken>.Values
		{
			get
			{
				return _properties.Dictionary.Values;
			}
		}

		bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		protected override IList<JToken> ChildrenTokens => _properties;

		public override JTokenType Type => JTokenType.Object;

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this[text];
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this[text] = value;
			}
		}

		public JToken this[string propertyName]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				return Property(propertyName)?.Value;
			}
			set
			{
				JProperty jProperty = Property(propertyName);
				if (jProperty != null)
				{
					jProperty.Value = value;
				}
				else
				{
					OnPropertyChanging(propertyName);
					Add(new JProperty(propertyName, value));
					OnPropertyChanged(propertyName);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler PropertyChanging;

		public JObject()
		{
		}

		public JObject(JObject other)
			: base(other)
		{
		}

		public JObject(params object[] content)
			: this((object)content)
		{
		}

		public JObject(object content)
		{
			Add(content);
		}

		internal override bool DeepEquals(JToken node)
		{
			JObject jObject = node as JObject;
			return jObject != null && ContentsEqual(jObject);
		}

		internal override void InsertItem(int index, JToken item)
		{
			if (item == null || item.Type != JTokenType.Comment)
			{
				base.InsertItem(index, item);
			}
		}

		internal override void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type != JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
			}
			JProperty jProperty = (JProperty)o;
			if (existing != null)
			{
				JProperty jProperty2 = (JProperty)existing;
				if (jProperty.Name == jProperty2.Name)
				{
					return;
				}
			}
			if (_properties.Dictionary != null && _properties.Dictionary.TryGetValue(jProperty.Name, out existing))
			{
				throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, jProperty.Name, GetType()));
			}
		}

		internal void InternalPropertyChanged(JProperty childProperty)
		{
			OnPropertyChanged(childProperty.Name);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOfItem(childProperty)));
		}

		internal void InternalPropertyChanging(JProperty childProperty)
		{
			OnPropertyChanging(childProperty.Name);
		}

		internal override JToken CloneToken()
		{
			return new JObject(this);
		}

		public IEnumerable<JProperty> Properties()
		{
			return ChildrenTokens.Cast<JProperty>();
		}

		public JProperty Property(string name)
		{
			if (_properties.Dictionary == null)
			{
				return null;
			}
			if (name == null)
			{
				return null;
			}
			_properties.Dictionary.TryGetValue(name, out JToken value);
			return (JProperty)value;
		}

		public JEnumerable<JToken> PropertyValues()
		{
			return new JEnumerable<JToken>(from p in Properties()
			select p.Value);
		}

		public new static JObject Load(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JObject from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JObject jObject = new JObject();
			jObject.SetLineInfo(reader as IJsonLineInfo);
			jObject.ReadTokenFrom(reader);
			return jObject;
		}

		public new static JObject Parse(string json)
		{
			JsonReader reader = new JsonTextReader(new StringReader(json));
			return Load(reader);
		}

		public new static JObject FromObject(object o)
		{
			return FromObject(o, new JsonSerializer());
		}

		public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jToken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jToken != null && jToken.Type != JTokenType.Object)
			{
				throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, jToken.Type));
			}
			return (JObject)jToken;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartObject();
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndObject();
		}

		public void Add(string propertyName, JToken value)
		{
			Add(new JProperty(propertyName, value));
		}

		bool IDictionary<string, JToken>.ContainsKey(string key)
		{
			if (_properties.Dictionary == null)
			{
				return false;
			}
			return _properties.Dictionary.ContainsKey(key);
		}

		public bool Remove(string propertyName)
		{
			JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				return false;
			}
			jProperty.Remove();
			return true;
		}

		public bool TryGetValue(string propertyName, out JToken value)
		{
			JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				value = null;
				return false;
			}
			value = jProperty.Value;
			return true;
		}

		void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
		{
			Add(new JProperty(item.Key, item.Value));
		}

		void ICollection<KeyValuePair<string, JToken>>.Clear()
		{
			RemoveAll();
		}

		bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
		{
			JProperty jProperty = Property(item.Key);
			if (jProperty == null)
			{
				return false;
			}
			return jProperty.Value == item.Value;
		}

		void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (base.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				array[arrayIndex + num] = new KeyValuePair<string, JToken>(childrenToken.Name, childrenToken.Value);
				num++;
			}
		}

		bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
		{
			if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
			{
				return false;
			}
			((IDictionary<string, JToken>)this).Remove(item.Key);
			return true;
		}

		internal override int GetDeepHashCode()
		{
			return ContentsHashCode();
		}

		public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
		{
			foreach (JToken jtoken in this.ChildrenTokens)
			{
				JProperty property = (JProperty)jtoken;
				yield return new KeyValuePair<string, JToken>(property.Name, property.Value);
			}
			yield break;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected virtual void OnPropertyChanging(string propertyName)
		{
			if (this.PropertyChanging != null)
			{
				this.PropertyChanging(this, new Newtonsoft.Json.ObservableSupport.PropertyChangingEventArgs(propertyName));
			}
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties((Attribute[])null);
		}

		private static Type GetTokenPropertyType(JToken token)
		{
			if (token is JValue)
			{
				JValue jValue = (JValue)token;
				return (jValue.Value == null) ? typeof(object) : jValue.Value.GetType();
			}
			return token.GetType();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			using (IEnumerator<KeyValuePair<string, JToken>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JToken> current = enumerator.Current;
					propertyDescriptorCollection.Add(new JPropertyDescriptor(current.Key, GetTokenPropertyType(current.Value)));
				}
				return propertyDescriptorCollection;
			}
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new TypeConverter();
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return null;
		}
	}
}
