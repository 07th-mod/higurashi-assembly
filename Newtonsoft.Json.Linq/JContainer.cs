using Newtonsoft.Json.ObservableSupport;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Newtonsoft.Json.Linq
{
	public abstract class JContainer : JToken, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable, ITypedList, IBindingList, IList, ICollection
	{
		private class JTokenReferenceEqualityComparer : IEqualityComparer<JToken>
		{
			public static readonly JTokenReferenceEqualityComparer Instance = new JTokenReferenceEqualityComparer();

			public bool Equals(JToken x, JToken y)
			{
				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(JToken obj)
			{
				return obj?.GetHashCode() ?? 0;
			}
		}

		private object _syncRoot;

		private bool _busy;

		JToken IList<JToken>.this[int index]
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

		bool ICollection<JToken>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, EnsureValue(value));
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return false;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				return ListSortDirection.Ascending;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return null;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		protected abstract IList<JToken> ChildrenTokens
		{
			get;
		}

		public override bool HasValues => ChildrenTokens.Count > 0;

		public override JToken First => ChildrenTokens.FirstOrDefault();

		public override JToken Last => ChildrenTokens.LastOrDefault();

		public int Count => ChildrenTokens.Count;

		public event ListChangedEventHandler ListChanged;

		public event Newtonsoft.Json.ObservableSupport.AddingNewEventHandler AddingNew;

		internal JContainer()
		{
		}

		internal JContainer(JContainer other)
		{
			ValidationUtils.ArgumentNotNull(other, "c");
			foreach (JToken item in (IEnumerable<JToken>)other)
			{
				Add(item);
			}
		}

		internal void CheckReentrancy()
		{
			if (_busy)
			{
				throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, GetType()));
			}
		}

		protected virtual void OnAddingNew(Newtonsoft.Json.ObservableSupport.AddingNewEventArgs e)
		{
			this.AddingNew?.Invoke(this, e);
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			ListChangedEventHandler listChanged = this.ListChanged;
			if (listChanged != null)
			{
				_busy = true;
				try
				{
					listChanged(this, e);
				}
				finally
				{
					_busy = false;
				}
			}
		}

		internal bool ContentsEqual(JContainer container)
		{
			JToken jToken = First;
			JToken jToken2 = container.First;
			if (jToken == jToken2)
			{
				return true;
			}
			while (true)
			{
				if (jToken == null && jToken2 == null)
				{
					return true;
				}
				if (jToken == null || jToken2 == null || !jToken.DeepEquals(jToken2))
				{
					break;
				}
				jToken = ((jToken == Last) ? null : jToken.Next);
				jToken2 = ((jToken2 == container.Last) ? null : jToken2.Next);
			}
			return false;
		}

		public override JEnumerable<JToken> Children()
		{
			return new JEnumerable<JToken>(ChildrenTokens);
		}

		public override IEnumerable<T> Values<T>()
		{
			return ChildrenTokens.Convert<JToken, T>();
		}

		public IEnumerable<JToken> Descendants()
		{
			foreach (JToken o in this.ChildrenTokens)
			{
				yield return o;
				JContainer c = o as JContainer;
				if (c != null)
				{
					foreach (JToken d in c.Descendants())
					{
						yield return d;
					}
				}
			}
			yield break;
		}

		internal bool IsMultiContent(object content)
		{
			return content is IEnumerable && !(content is string) && !(content is JToken) && !(content is byte[]);
		}

		internal JToken EnsureParentToken(JToken item)
		{
			if (item == null)
			{
				return new JValue((object)null);
			}
			if (item.Parent != null)
			{
				item = item.CloneToken();
			}
			else
			{
				JContainer jContainer = this;
				while (jContainer.Parent != null)
				{
					jContainer = jContainer.Parent;
				}
				if (item == jContainer)
				{
					item = item.CloneToken();
				}
			}
			return item;
		}

		internal int IndexOfItem(JToken item)
		{
			return ChildrenTokens.IndexOf(item, JTokenReferenceEqualityComparer.Instance);
		}

		internal virtual void InsertItem(int index, JToken item)
		{
			if (index > ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
			}
			CheckReentrancy();
			item = EnsureParentToken(item);
			JToken jToken = (index != 0) ? ChildrenTokens[index - 1] : null;
			JToken jToken2 = (index != ChildrenTokens.Count) ? ChildrenTokens[index] : null;
			ValidateToken(item, null);
			item.Parent = this;
			item.Previous = jToken;
			if (jToken != null)
			{
				jToken.Next = item;
			}
			item.Next = jToken2;
			if (jToken2 != null)
			{
				jToken2.Previous = item;
			}
			ChildrenTokens.Insert(index, item);
			if (this.ListChanged != null)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			}
		}

		internal virtual void RemoveItemAt(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			CheckReentrancy();
			JToken jToken = ChildrenTokens[index];
			JToken jToken2 = (index != 0) ? ChildrenTokens[index - 1] : null;
			JToken jToken3 = (index != ChildrenTokens.Count - 1) ? ChildrenTokens[index + 1] : null;
			if (jToken2 != null)
			{
				jToken2.Next = jToken3;
			}
			if (jToken3 != null)
			{
				jToken3.Previous = jToken2;
			}
			jToken.Parent = null;
			jToken.Previous = null;
			jToken.Next = null;
			ChildrenTokens.RemoveAt(index);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		internal virtual bool RemoveItem(JToken item)
		{
			int num = IndexOfItem(item);
			if (num >= 0)
			{
				RemoveItemAt(num);
				return true;
			}
			return false;
		}

		internal virtual JToken GetItem(int index)
		{
			return ChildrenTokens[index];
		}

		internal virtual void SetItem(int index, JToken item)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			JToken jToken = ChildrenTokens[index];
			if (!IsTokenUnchanged(jToken, item))
			{
				CheckReentrancy();
				item = EnsureParentToken(item);
				ValidateToken(item, jToken);
				JToken jToken2 = (index != 0) ? ChildrenTokens[index - 1] : null;
				JToken jToken3 = (index != ChildrenTokens.Count - 1) ? ChildrenTokens[index + 1] : null;
				item.Parent = this;
				item.Previous = jToken2;
				if (jToken2 != null)
				{
					jToken2.Next = item;
				}
				item.Next = jToken3;
				if (jToken3 != null)
				{
					jToken3.Previous = item;
				}
				ChildrenTokens[index] = item;
				jToken.Parent = null;
				jToken.Previous = null;
				jToken.Next = null;
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		internal virtual void ClearItems()
		{
			CheckReentrancy();
			foreach (JToken childrenToken in ChildrenTokens)
			{
				childrenToken.Parent = null;
				childrenToken.Previous = null;
				childrenToken.Next = null;
			}
			ChildrenTokens.Clear();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		internal virtual void ReplaceItem(JToken existing, JToken replacement)
		{
			if (existing != null && existing.Parent == this)
			{
				int index = IndexOfItem(existing);
				SetItem(index, replacement);
			}
		}

		internal virtual bool ContainsItem(JToken item)
		{
			return IndexOfItem(item) != -1;
		}

		internal virtual void CopyItemsTo(Array array, int arrayIndex)
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
			if (Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JToken childrenToken in ChildrenTokens)
			{
				array.SetValue(childrenToken, arrayIndex + num);
				num++;
			}
		}

		internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
		{
			JValue jValue = currentValue as JValue;
			if (jValue != null)
			{
				if (jValue.Type == JTokenType.Null && newValue == null)
				{
					return true;
				}
				return jValue.Equals(newValue);
			}
			return false;
		}

		internal virtual void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type == JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
			}
		}

		public virtual void Add(object content)
		{
			AddInternal(ChildrenTokens.Count, content);
		}

		public void AddFirst(object content)
		{
			AddInternal(0, content);
		}

		internal void AddInternal(int index, object content)
		{
			if (IsMultiContent(content))
			{
				IEnumerable enumerable = (IEnumerable)content;
				int num = index;
				IEnumerator enumerator = enumerable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						AddInternal(num, current);
						num++;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			else
			{
				JToken item = CreateFromContent(content);
				InsertItem(index, item);
			}
		}

		internal JToken CreateFromContent(object content)
		{
			if (content is JToken)
			{
				return (JToken)content;
			}
			return new JValue(content);
		}

		public JsonWriter CreateWriter()
		{
			return new JTokenWriter(this);
		}

		public void ReplaceAll(object content)
		{
			ClearItems();
			Add(content);
		}

		public void RemoveAll()
		{
			ClearItems();
		}

		internal void ReadTokenFrom(JsonReader r)
		{
			int depth = r.Depth;
			if (!r.Read())
			{
				throw new Exception("Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
			}
			ReadContentFrom(r);
			int depth2 = r.Depth;
			if (depth2 > depth)
			{
				throw new Exception("Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
			}
		}

		internal void ReadContentFrom(JsonReader r)
		{
			ValidationUtils.ArgumentNotNull(r, "r");
			IJsonLineInfo lineInfo = r as IJsonLineInfo;
			JContainer jContainer = this;
			do
			{
				if (jContainer is JProperty && ((JProperty)jContainer).Value != null)
				{
					if (jContainer == this)
					{
						break;
					}
					jContainer = jContainer.Parent;
				}
				switch (r.TokenType)
				{
				case JsonToken.StartArray:
				{
					JArray jArray = new JArray();
					jArray.SetLineInfo(lineInfo);
					jContainer.Add(jArray);
					jContainer = jArray;
					break;
				}
				case JsonToken.EndArray:
					if (jContainer == this)
					{
						return;
					}
					jContainer = jContainer.Parent;
					break;
				case JsonToken.StartObject:
				{
					JObject jObject2 = new JObject();
					jObject2.SetLineInfo(lineInfo);
					jContainer.Add(jObject2);
					jContainer = jObject2;
					break;
				}
				case JsonToken.EndObject:
					if (jContainer == this)
					{
						return;
					}
					jContainer = jContainer.Parent;
					break;
				case JsonToken.StartConstructor:
				{
					JConstructor jConstructor = new JConstructor(r.Value.ToString());
					jConstructor.SetLineInfo(jConstructor);
					jContainer.Add(jConstructor);
					jContainer = jConstructor;
					break;
				}
				case JsonToken.EndConstructor:
					if (jContainer == this)
					{
						return;
					}
					jContainer = jContainer.Parent;
					break;
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					JValue jValue = new JValue(r.Value);
					jValue.SetLineInfo(lineInfo);
					jContainer.Add(jValue);
					break;
				}
				case JsonToken.Comment:
				{
					JValue jValue = JValue.CreateComment(r.Value.ToString());
					jValue.SetLineInfo(lineInfo);
					jContainer.Add(jValue);
					break;
				}
				case JsonToken.Null:
				{
					JValue jValue = new JValue(null, JTokenType.Null);
					jValue.SetLineInfo(lineInfo);
					jContainer.Add(jValue);
					break;
				}
				case JsonToken.Undefined:
				{
					JValue jValue = new JValue(null, JTokenType.Undefined);
					jValue.SetLineInfo(lineInfo);
					jContainer.Add(jValue);
					break;
				}
				case JsonToken.PropertyName:
				{
					string name = r.Value.ToString();
					JProperty jProperty = new JProperty(name);
					jProperty.SetLineInfo(lineInfo);
					JObject jObject = (JObject)jContainer;
					JProperty jProperty2 = jObject.Property(name);
					if (jProperty2 == null)
					{
						jContainer.Add(jProperty);
					}
					else
					{
						jProperty2.Replace(jProperty);
					}
					jContainer = jProperty;
					break;
				}
				default:
					throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
				case JsonToken.None:
					break;
				}
			}
			while (r.Read());
		}

		internal int ContentsHashCode()
		{
			int num = 0;
			foreach (JToken childrenToken in ChildrenTokens)
			{
				num ^= childrenToken.GetDeepHashCode();
			}
			return num;
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			return string.Empty;
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return (First as ICustomTypeDescriptor)?.GetProperties();
		}

		int IList<JToken>.IndexOf(JToken item)
		{
			return IndexOfItem(item);
		}

		void IList<JToken>.Insert(int index, JToken item)
		{
			InsertItem(index, item);
		}

		void IList<JToken>.RemoveAt(int index)
		{
			RemoveItemAt(index);
		}

		void ICollection<JToken>.Add(JToken item)
		{
			Add(item);
		}

		void ICollection<JToken>.Clear()
		{
			ClearItems();
		}

		bool ICollection<JToken>.Contains(JToken item)
		{
			return ContainsItem(item);
		}

		void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
		{
			CopyItemsTo(array, arrayIndex);
		}

		bool ICollection<JToken>.Remove(JToken item)
		{
			return RemoveItem(item);
		}

		private JToken EnsureValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is JToken)
			{
				return (JToken)value;
			}
			throw new ArgumentException("Argument is not a JToken.");
		}

		int IList.Add(object value)
		{
			Add(EnsureValue(value));
			return Count - 1;
		}

		void IList.Clear()
		{
			ClearItems();
		}

		bool IList.Contains(object value)
		{
			return ContainsItem(EnsureValue(value));
		}

		int IList.IndexOf(object value)
		{
			return IndexOfItem(EnsureValue(value));
		}

		void IList.Insert(int index, object value)
		{
			InsertItem(index, EnsureValue(value));
		}

		void IList.Remove(object value)
		{
			RemoveItem(EnsureValue(value));
		}

		void IList.RemoveAt(int index)
		{
			RemoveItemAt(index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyItemsTo(array, index);
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		object IBindingList.AddNew()
		{
			Newtonsoft.Json.ObservableSupport.AddingNewEventArgs addingNewEventArgs = new Newtonsoft.Json.ObservableSupport.AddingNewEventArgs();
			OnAddingNew(addingNewEventArgs);
			if (addingNewEventArgs.NewObject == null)
			{
				throw new Exception("Could not determine new value to add to '{0}'.".FormatWith(CultureInfo.InvariantCulture, GetType()));
			}
			if (!(addingNewEventArgs.NewObject is JToken))
			{
				throw new Exception("New item to be added to collection must be compatible with {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JToken)));
			}
			JToken jToken = (JToken)addingNewEventArgs.NewObject;
			Add(jToken);
			return jToken;
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}
	}
}
