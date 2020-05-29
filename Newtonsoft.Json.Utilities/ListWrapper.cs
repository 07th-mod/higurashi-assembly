using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
	internal class ListWrapper<T> : CollectionWrapper<T>, IList<T>, IWrappedList, IEnumerable, ICollection<T>, IEnumerable<T>, IList, ICollection
	{
		private readonly IList<T> _genericList;

		public T this[int index]
		{
			get
			{
				if (_genericList != null)
				{
					return _genericList[index];
				}
				return (T)((IList)this)[index];
			}
			set
			{
				if (_genericList != null)
				{
					_genericList[index] = value;
				}
				else
				{
					((IList)this)[index] = value;
				}
			}
		}

		public override int Count
		{
			get
			{
				if (_genericList != null)
				{
					return _genericList.Count;
				}
				return base.Count;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				if (_genericList != null)
				{
					return _genericList.IsReadOnly;
				}
				return base.IsReadOnly;
			}
		}

		public object UnderlyingList
		{
			get
			{
				if (_genericList != null)
				{
					return _genericList;
				}
				return base.UnderlyingCollection;
			}
		}

		public ListWrapper(IList list)
			: base(list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			if (list is IList<T>)
			{
				_genericList = (IList<T>)list;
			}
		}

		public ListWrapper(IList<T> list)
			: base((ICollection<T>)list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			_genericList = list;
		}

		public int IndexOf(T item)
		{
			if (_genericList != null)
			{
				return _genericList.IndexOf(item);
			}
			return ((IList)this).IndexOf((object)item);
		}

		public void Insert(int index, T item)
		{
			if (_genericList != null)
			{
				_genericList.Insert(index, item);
			}
			else
			{
				((IList)this).Insert(index, (object)item);
			}
		}

		public void RemoveAt(int index)
		{
			if (_genericList != null)
			{
				_genericList.RemoveAt(index);
			}
			else
			{
				((IList)this).RemoveAt(index);
			}
		}

		public override void Add(T item)
		{
			if (_genericList != null)
			{
				_genericList.Add(item);
			}
			else
			{
				base.Add(item);
			}
		}

		public override void Clear()
		{
			if (_genericList != null)
			{
				_genericList.Clear();
			}
			else
			{
				base.Clear();
			}
		}

		public override bool Contains(T item)
		{
			if (_genericList != null)
			{
				return _genericList.Contains(item);
			}
			return base.Contains(item);
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
			if (_genericList != null)
			{
				_genericList.CopyTo(array, arrayIndex);
			}
			else
			{
				base.CopyTo(array, arrayIndex);
			}
		}

		public override bool Remove(T item)
		{
			if (_genericList != null)
			{
				return _genericList.Remove(item);
			}
			bool flag = base.Contains(item);
			if (flag)
			{
				base.Remove(item);
			}
			return flag;
		}

		public override IEnumerator<T> GetEnumerator()
		{
			if (_genericList != null)
			{
				return _genericList.GetEnumerator();
			}
			return base.GetEnumerator();
		}
	}
}
