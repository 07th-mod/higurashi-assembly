using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Linq
{
	public class JArray : JContainer, IList<JToken>, IEnumerable, ICollection<JToken>, IEnumerable<JToken>
	{
		private IList<JToken> _values = new List<JToken>();

		bool ICollection<JToken>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		protected override IList<JToken> ChildrenTokens => _values;

		public override JTokenType Type => JTokenType.Array;

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				if (!(key is int))
				{
					throw new ArgumentException("Set JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				SetItem((int)key, value);
			}
		}

		public JToken this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		public JArray()
		{
		}

		public JArray(JArray other)
			: base(other)
		{
		}

		public JArray(params object[] content)
			: this((object)content)
		{
		}

		public JArray(object content)
		{
			Add(content);
		}

		internal override bool DeepEquals(JToken node)
		{
			JArray jArray = node as JArray;
			return jArray != null && ContentsEqual(jArray);
		}

		internal override JToken CloneToken()
		{
			return new JArray(this);
		}

		public new static JArray Load(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JArray from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JArray jArray = new JArray();
			jArray.SetLineInfo(reader as IJsonLineInfo);
			jArray.ReadTokenFrom(reader);
			return jArray;
		}

		public new static JArray Parse(string json)
		{
			JsonReader reader = new JsonTextReader(new StringReader(json));
			return Load(reader);
		}

		public new static JArray FromObject(object o)
		{
			return FromObject(o, new JsonSerializer());
		}

		public new static JArray FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jToken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jToken.Type != JTokenType.Array)
			{
				throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, jToken.Type));
			}
			return (JArray)jToken;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartArray();
			foreach (JToken childrenToken in ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndArray();
		}

		public int IndexOf(JToken item)
		{
			return IndexOfItem(item);
		}

		public void Insert(int index, JToken item)
		{
			InsertItem(index, item);
		}

		public void RemoveAt(int index)
		{
			RemoveItemAt(index);
		}

		public void Add(JToken item)
		{
			Add((object)item);
		}

		public void Clear()
		{
			ClearItems();
		}

		public bool Contains(JToken item)
		{
			return ContainsItem(item);
		}

		void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
		{
			CopyItemsTo(array, arrayIndex);
		}

		public bool Remove(JToken item)
		{
			return RemoveItem(item);
		}

		internal override int GetDeepHashCode()
		{
			return ContentsHashCode();
		}
	}
}
