﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Diagnostics;

//using System.Runtime.Serialization;
//using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Collections
{
	public class ArrayList : IList, ICloneable
	{
		private Object[] _items;

		[ContractPublicPropertyName("Count")]
		private int _size;

		private int _version;
		private readonly Object _syncRoot;

		private const int _defaultCapacity = 4;
		private static readonly Object[] emptyArray = EmptyArray<Object>.Value;

		// Note: this constructor is a bogus constructor that does nothing
		// and is for use only with SyncArrayList.
		internal ArrayList(bool trash)
		{
		}

		// Constructs a ArrayList. The list is initially empty and has a capacity
		// of zero. Upon adding the first element to the list the capacity is
		// increased to _defaultCapacity, and then increased in multiples of two as required.
		public ArrayList()
		{
			_items = emptyArray;
		}

		// Constructs a ArrayList with a given initial capacity. The list is
		// initially empty, but will have room for the given number of elements
		// before any reallocations are required.
		//
		public ArrayList(int capacity)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException();
			Contract.EndContractBlock();

			if (capacity == 0)
				_items = emptyArray;
			else
				_items = new Object[capacity];
		}

		// Constructs a ArrayList, copying the contents of the given collection. The
		// size and capacity of the new list will both be equal to the size of the
		// given collection.
		//
		public ArrayList(ICollection c)
		{
			if (c == null)
				throw new ArgumentNullException();//(nameof(c), Environment.GetResourceString("ArgumentNull_Collection"));
			Contract.EndContractBlock();

			int count = c.Count;
			if (count == 0)
			{
				_items = emptyArray;
			}
			else
			{
				_items = new Object[count];
				AddRange(c);
			}
		}

		// Gets and sets the capacity of this list.  The capacity is the size of
		// the internal array used to hold items.  When set, the internal
		// array of the list is reallocated to the given capacity.
		//
		public virtual int Capacity
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() >= Count);
				return _items.Length;
			}
			set
			{
				if (value < _size)
				{
					throw new ArgumentOutOfRangeException();//(nameof(value), Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
				}
				Contract.Ensures(Capacity >= 0);
				Contract.EndContractBlock();

				// We don't want to update the version number when we change the capacity.
				// Some existing applications have dependency on this.
				if (value != _items.Length)
				{
					if (value > 0)
					{
						Object[] newItems = new Object[value];
						if (_size > 0)
						{
							Array.Copy(_items, 0, newItems, 0, _size);
						}
						_items = newItems;
					}
					else
					{
						_items = new Object[_defaultCapacity];
					}
				}
			}
		}

		// Read-only property describing how many elements are in the List.
		public virtual int Count
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() >= 0);
				return _size;
			}
		}

		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		// Is this ArrayList read-only?
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		// Is this ArrayList synchronized (thread-safe)?
		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		// Synchronization root for this object.
		public virtual Object SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					//TODO System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
				}
				return _syncRoot;
			}
		}

		// Sets or Gets the element at the given index.
		//
		public virtual Object this[int index]
		{
			get
			{
				if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				Contract.EndContractBlock();
				return _items[index];
			}
			set
			{
				if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				Contract.EndContractBlock();
				_items[index] = value;
				_version++;
			}
		}

		// Creates a ArrayList wrapper for a particular IList.  This does not
		// copy the contents of the IList, but only wraps the ILIst.  So any
		// changes to the underlying list will affect the ArrayList.  This would
		// be useful if you want to Reverse a subrange of an IList, or want to
		// use a generic BinarySearch or Sort method without implementing one yourself.
		// However, since these methods are generic, the performance may not be
		// nearly as good for some operations as they would be on the IList itself.
		//
		public static ArrayList Adapter(IList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();
			return new IListWrapper(list);
		}

		// Adds the given object to the end of this list. The size of the list is
		// increased by one. If required, the capacity of the list is doubled
		// before adding the new element.
		//
		public virtual int Add(Object value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0);
			if (_size == _items.Length) EnsureCapacity(_size + 1);
			_items[_size] = value;
			_version++;
			return _size++;
		}

		// Adds the elements of the given collection to the end of this list. If
		// required, the capacity of the list is increased to twice the previous
		// capacity or the new size, whichever is larger.
		//
		public virtual void AddRange(ICollection c)
		{
			InsertRange(_size, c);
		}

		// Searches a section of the list for a given element using a binary search
		// algorithm. Elements of the list are compared to the search value using
		// the given IComparer interface. If comparer is null, elements of
		// the list are compared to the search value using the IComparable
		// interface, which in that case must be implemented by all elements of the
		// list and the given search value. This method assumes that the given
		// section of the list is already sorted; if this is not the case, the
		// result will be incorrect.
		//
		// The method returns the index of the given value in the list. If the
		// list does not contain the given value, the method returns a negative
		// integer. The bitwise complement operator (~) can be applied to a
		// negative result to produce the index of the first element (if any) that
		// is larger than the given search value. This is also the index at which
		// the search value should be inserted into the list in order for the list
		// to remain sorted.
		//
		// The method uses the Array.BinarySearch method to perform the
		// search.
		//
		public virtual int BinarySearch(int index, int count, Object value, IComparer comparer)
		{
			throw new Exception("unimplemented");
		}

		public virtual int BinarySearch(Object value)
		{
			Contract.Ensures(Contract.Result<int>() < Count);
			return BinarySearch(0, Count, value, null);
		}

		public virtual int BinarySearch(Object value, IComparer comparer)
		{
			Contract.Ensures(Contract.Result<int>() < Count);
			return BinarySearch(0, Count, value, comparer);
		}

		// Clears the contents of ArrayList.
		public virtual void Clear()
		{
			throw new Exception("unimplemented");
		}

		// Clones this ArrayList, doing a shallow copy.  (A copy is made of all
		// Object references in the ArrayList, but the Objects pointed to
		// are not cloned).
		public virtual Object Clone()
		{
			Contract.Ensures(Contract.Result<Object>() != null);
			ArrayList la = new ArrayList(_size);
			la._size = _size;
			la._version = _version;
			Array.Copy(_items, 0, la._items, 0, _size);
			return la;
		}

		// Contains returns true if the specified element is in the ArrayList.
		// It does a linear, O(n) search.  Equality is determined by calling
		// item.Equals().
		//
		public virtual bool Contains(Object item)
		{
			if (item == null)
			{
				for (int i = 0; i < _size; i++)
					if (_items[i] == null)
						return true;
				return false;
			}
			else
			{
				for (int i = 0; i < _size; i++)
					if ((_items[i] != null) && (_items[i].Equals(item)))
						return true;
				return false;
			}
		}

		// Copies this ArrayList into array, which must be of a
		// compatible array type.
		//
		public virtual void CopyTo(Array array)
		{
			CopyTo(array, 0);
		}

		// Copies this ArrayList into array, which must be of a
		// compatible array type.
		//
		public virtual void CopyTo(Array array, int arrayIndex)
		{
			if ((array != null) && (array.Rank != 1))
				throw new ArgumentException();//(Environment.GetResourceString("Arg_RankMultiDimNotSupported"));
			Contract.EndContractBlock();

			// Delegate rest of error checking to Array.Copy.
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}

		// Copies a section of this list to the given array at the given index.
		//
		// The method uses the Array.Copy method to copy the elements.
		//
		public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if (_size - index < count)
				throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));
			if ((array != null) && (array.Rank != 1))
				throw new ArgumentException();//(Environment.GetResourceString("Arg_RankMultiDimNotSupported"));
			Contract.EndContractBlock();

			// Delegate rest of error checking to Array.Copy.
			Array.Copy(_items, index, array, arrayIndex, count);
		}

		// Ensures that the capacity of this list is at least the given minimum
		// value. If the currect capacity of the list is less than min, the
		// capacity is increased to twice the current capacity or to min,
		// whichever is larger.
		private void EnsureCapacity(int min)
		{
			if (_items.Length < min)
			{
				int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;

				// Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
				// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
				if ((uint)newCapacity > Array.MaxArrayLength) newCapacity = Array.MaxArrayLength;
				if (newCapacity < min) newCapacity = min;
				Capacity = newCapacity;
			}
		}

		// Returns a list wrapper that is fixed at the current size.  Operations
		// that add or remove items will fail, however, replacing items is allowed.
		//
		public static IList FixedSize(IList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<IList>() != null);
			Contract.EndContractBlock();
			return new FixedSizeList(list);
		}

		// Returns a list wrapper that is fixed at the current size.  Operations
		// that add or remove items will fail, however, replacing items is allowed.
		//
		public static ArrayList FixedSize(ArrayList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();
			return new FixedSizeArrayList(list);
		}

		// Returns an enumerator for this list with the given
		// permission for removal of elements. If modifications made to the list
		// while an enumeration is in progress, the MoveNext and
		// GetObject methods of the enumerator will throw an exception.
		//
		public virtual IEnumerator GetEnumerator()
		{
			Contract.Ensures(Contract.Result<IEnumerator>() != null);
			return new ArrayListEnumeratorSimple(this);
		}

		// Returns an enumerator for a section of this list with the given
		// permission for removal of elements. If modifications made to the list
		// while an enumeration is in progress, the MoveNext and
		// GetObject methods of the enumerator will throw an exception.
		//
		public virtual IEnumerator GetEnumerator(int index, int count)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			if (count < 0)
				throw new ArgumentOutOfRangeException();//(nameof(count), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			if (_size - index < count)
				throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));
			Contract.Ensures(Contract.Result<IEnumerator>() != null);
			Contract.EndContractBlock();

			return new ArrayListEnumerator(this, index, count);
		}

		// Returns the index of the first occurrence of a given value in a range of
		// this list. The list is searched forwards from beginning to end.
		// The elements of the list are compared to the given value using the
		// Object.Equals method.
		//
		// This method uses the Array.IndexOf method to perform the
		// search.
		//
		public virtual int IndexOf(Object value)
		{
			throw new Exception("unimplemented");
		}

		// Returns the index of the first occurrence of a given value in a range of
		// this list. The list is searched forwards, starting at index
		// startIndex and ending at count number of elements. The
		// elements of the list are compared to the given value using the
		// Object.Equals method.
		//
		// This method uses the Array.IndexOf method to perform the
		// search.
		//
		public virtual int IndexOf(Object value, int startIndex)
		{
			throw new Exception("unimplemented");
		}

		// Returns the index of the first occurrence of a given value in a range of
		// this list. The list is searched forwards, starting at index
		// startIndex and upto count number of elements. The
		// elements of the list are compared to the given value using the
		// Object.Equals method.
		//
		// This method uses the Array.IndexOf method to perform the
		// search.
		//
		public virtual int IndexOf(Object value, int startIndex, int count)
		{
			throw new Exception("unimplemented");
		}

		// Inserts an element into this list at a given index. The size of the list
		// is increased by one. If required, the capacity of the list is doubled
		// before inserting the new element.
		//
		public virtual void Insert(int index, Object value)
		{
			// Note that insertions at the end are legal.
			if (index < 0 || index > _size) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_ArrayListInsert"));

			//Contract.Ensures(Count == Contract.OldValue(Count) + 1);
			Contract.EndContractBlock();

			if (_size == _items.Length) EnsureCapacity(_size + 1);
			if (index < _size)
			{
				Array.Copy(_items, index, _items, index + 1, _size - index);
			}
			_items[index] = value;
			_size++;
			_version++;
		}

		// Inserts the elements of the given collection at a given index. If
		// required, the capacity of the list is increased to twice the previous
		// capacity or the new size, whichever is larger.  Ranges may be added
		// to the end of the list by setting index to the ArrayList's size.
		//
		public virtual void InsertRange(int index, ICollection c)
		{
			if (c == null)
				throw new ArgumentNullException();//(nameof(c), Environment.GetResourceString("ArgumentNull_Collection"));
			if (index < 0 || index > _size) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));

			//Contract.Ensures(Count == Contract.OldValue(Count) + c.Count);
			Contract.EndContractBlock();

			int count = c.Count;
			if (count > 0)
			{
				EnsureCapacity(_size + count);

				// shift existing items
				if (index < _size)
				{
					Array.Copy(_items, index, _items, index + count, _size - index);
				}

				Object[] itemsToInsert = new Object[count];
				c.CopyTo(itemsToInsert, 0);
				itemsToInsert.CopyTo(_items, index);
				_size += count;
				_version++;
			}
		}

		// Returns the index of the last occurrence of a given value in a range of
		// this list. The list is searched backwards, starting at the end
		// and ending at the first element in the list. The elements of the list
		// are compared to the given value using the Object.Equals method.
		//
		// This method uses the Array.LastIndexOf method to perform the
		// search.
		//
		public virtual int LastIndexOf(Object value)
		{
			Contract.Ensures(Contract.Result<int>() < _size);
			return LastIndexOf(value, _size - 1, _size);
		}

		// Returns the index of the last occurrence of a given value in a range of
		// this list. The list is searched backwards, starting at index
		// startIndex and ending at the first element in the list. The
		// elements of the list are compared to the given value using the
		// Object.Equals method.
		//
		// This method uses the Array.LastIndexOf method to perform the
		// search.
		//
		public virtual int LastIndexOf(Object value, int startIndex)
		{
			if (startIndex >= _size)
				throw new ArgumentOutOfRangeException();//(nameof(startIndex), Environment.GetResourceString("ArgumentOutOfRange_Index"));
			Contract.Ensures(Contract.Result<int>() < Count);
			Contract.EndContractBlock();
			return LastIndexOf(value, startIndex, startIndex + 1);
		}

		// Returns the index of the last occurrence of a given value in a range of
		// this list. The list is searched backwards, starting at index
		// startIndex and upto count elements. The elements of
		// the list are compared to the given value using the Object.Equals
		// method.
		//
		// This method uses the Array.LastIndexOf method to perform the
		// search.
		//
		public virtual int LastIndexOf(Object value, int startIndex, int count)
		{
			throw new Exception("unimplemented");
		}

		// Returns a read-only IList wrapper for the given IList.
		//
#if FEATURE_CORECLR
        [FriendAccessAllowed]
#endif

		public static IList ReadOnly(IList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<IList>() != null);
			Contract.EndContractBlock();
			return new ReadOnlyList(list);
		}

		// Returns a read-only ArrayList wrapper for the given ArrayList.
		//
		public static ArrayList ReadOnly(ArrayList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();
			return new ReadOnlyArrayList(list);
		}

		// Removes the element at the given index. The size of the list is
		// decreased by one.
		//
		public virtual void Remove(Object obj)
		{
			Contract.Ensures(Count >= 0);

			int index = IndexOf(obj);

			//TODO WTF-
			//BCLDebug.Correctness(index >= 0 || !( obj is Int32 ), "You passed an Int32 to Remove that wasn't in the ArrayList." + Environment.NewLine + "Did you mean RemoveAt?  int: " + obj + "  Count: " + Count);
			if (index >= 0)
				RemoveAt(index);
		}

		// Removes the element at the given index. The size of the list is
		// decreased by one.
		//
		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));
			Contract.Ensures(Count >= 0);

			//Contract.Ensures(Count == Contract.OldValue(Count) - 1);
			Contract.EndContractBlock();

			_size--;
			if (index < _size)
			{
				Array.Copy(_items, index + 1, _items, index, _size - index);
			}
			_items[_size] = null;
			_version++;
		}

		// Removes a range of elements from this list.
		//
		public virtual void RemoveRange(int index, int count)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			if (count < 0)
				throw new ArgumentOutOfRangeException();//(nameof(count), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			if (_size - index < count)
				throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));
			Contract.Ensures(Count >= 0);

			//Contract.Ensures(Count == Contract.OldValue(Count) - count);
			Contract.EndContractBlock();

			if (count > 0)
			{
				int i = _size;
				_size -= count;
				if (index < _size)
				{
					Array.Copy(_items, index + count, _items, index, _size - index);
				}
				while (i > _size) _items[--i] = null;
				_version++;
			}
		}

		// Returns an IList that contains count copies of value.
		//
		public static ArrayList Repeat(Object value, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException();//(nameof(count), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();

			ArrayList list = new ArrayList((count > _defaultCapacity) ? count : _defaultCapacity);
			for (int i = 0; i < count; i++)
				list.Add(value);
			return list;
		}

		// Reverses the elements in this list.
		public virtual void Reverse()
		{
			Reverse(0, Count);
		}

		// Reverses the elements in a range of this list. Following a call to this
		// method, an element in the range given by index and count
		// which was previously located at index i will now be located at
		// index index + (index + count - i - 1).
		//
		// This method uses the Array.Reverse method to reverse the
		// elements.
		//
		public virtual void Reverse(int index, int count)
		{
			throw new Exception("unimplemented");
		}

		// Sets the elements starting at the given index to the elements of the
		// given collection.
		//
		public virtual void SetRange(int index, ICollection c)
		{
			if (c == null) throw new ArgumentNullException();//(nameof(c), Environment.GetResourceString("ArgumentNull_Collection"));
			Contract.EndContractBlock();
			int count = c.Count;
			if (index < 0 || index > _size - count) throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));

			if (count > 0)
			{
				c.CopyTo(_items, index);
				_version++;
			}
		}

		public virtual ArrayList GetRange(int index, int count)
		{
			if (index < 0 || count < 0)
				throw new ArgumentOutOfRangeException();//(( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			if (_size - index < count)
				throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();
			return new Range(this, index, count);
		}

		// Sorts the elements in this list.  Uses the default comparer and
		// Array.Sort.
		public virtual void Sort()
		{//Comparer.Default
			Sort(0, Count, null);
		}

		// Sorts the elements in this list.  Uses Array.Sort with the
		// provided comparer.
		public virtual void Sort(IComparer comparer)
		{
			Sort(0, Count, comparer);
		}

		// Sorts the elements in a section of this list. The sort compares the
		// elements to each other using the given IComparer interface. If
		// comparer is null, the elements are compared to each other using
		// the IComparable interface, which in that case must be implemented by all
		// elements of the list.
		//
		// This method uses the Array.Sort method to sort the elements.
		//
		public virtual void Sort(int index, int count, IComparer comparer)
		{
			throw new Exception("unimplemented");
		}

		// Returns a thread-safe wrapper around an IList.
		//
		//	[HostProtection(Synchronization = true)]
		public static IList Synchronized(IList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<IList>() != null);
			Contract.EndContractBlock();
			return new SyncIList(list);
		}

		// Returns a thread-safe wrapper around a ArrayList.
		//
		//[HostProtection(Synchronization = true)]
		public static ArrayList Synchronized(ArrayList list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			Contract.Ensures(Contract.Result<ArrayList>() != null);
			Contract.EndContractBlock();
			return new SyncArrayList(list);
		}

		// ToArray returns a new Object array containing the contents of the ArrayList.
		// This requires copying the ArrayList, which is an O(n) operation.
		public virtual Object[] ToArray()
		{
			Contract.Ensures(Contract.Result<Object[]>() != null);

			Object[] array = new Object[_size];
			Array.Copy(_items, 0, array, 0, _size);
			return array;
		}

		// ToArray returns a new array of a particular type containing the contents
		// of the ArrayList.  This requires copying the ArrayList and potentially
		// downcasting all elements.  This copy may fail and is an O(n) operation.
		// Internally, this implementation calls Array.Copy.
		//
		//[SecuritySafeCritical]
		public virtual Array ToArray(Type type)
		{
			throw new Exception("unimplemented");
		}

		// Sets the capacity of this list to the size of the list. This method can
		// be used to minimize a list's memory overhead once it is known that no
		// new elements will be added to the list. To completely clear a list and
		// release all memory referenced by the list, execute the following
		// statements:
		//
		// list.Clear();
		// list.TrimToSize();
		//
		public virtual void TrimToSize()
		{
			Capacity = _size;
		}

		// This class wraps an IList, exposing it as a ArrayList
		// Note this requires reimplementing half of ArrayList...
		[Serializable]
		private class IListWrapper : ArrayList
		{
			private readonly IList _list;

			internal IListWrapper(IList list)
			{
				_list = list;
				_version = 0; // list doesn't not contain a version number
			}

			public override int Capacity
			{
				get
				{
					return _list.Count;
				}
				set
				{
					if (value < Count) throw new ArgumentOutOfRangeException();//(nameof(value), Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
					Contract.EndContractBlock();
				}
			}

			public override int Count
			{
				get { return _list.Count; }
			}

			public override bool IsReadOnly
			{
				get { return _list.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return _list.IsFixedSize; }
			}

			public override bool IsSynchronized
			{
				get { return _list.IsSynchronized; }
			}

			public override Object this[int index]
			{
				get
				{
					return _list[index];
				}
				set
				{
					_list[index] = value;
					_version++;
				}
			}

			public override Object SyncRoot
			{
				get { return _list.SyncRoot; }
			}

			public override int Add(Object obj)
			{
				int i = _list.Add(obj);
				_version++;
				return i;
			}

			public override void AddRange(ICollection c)
			{
				InsertRange(Count, c);
			}

			// Other overloads with automatically work
			public override int BinarySearch(int index, int count, Object value, IComparer comparer)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();//(( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				if (this.Count - index < count)
					throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));
				Contract.EndContractBlock();
				if (comparer == null)
					throw new Exception("unimplemented");

				int lo = index;
				int hi = index + count - 1;
				int mid;
				while (lo <= hi)
				{
					mid = (lo + hi) / 2;
					int r = comparer.Compare(value, _list[mid]);
					if (r == 0)
						return mid;
					if (r < 0)
						hi = mid - 1;
					else
						lo = mid + 1;
				}

				// return bitwise complement of the first element greater than value.
				// Since hi is less than lo now, ~lo is the correct item.
				return ~lo;
			}

			public override void Clear()
			{
				// If _list is an array, it will support Clear method.
				// We shouldn't allow clear operation on a FixedSized ArrayList
				if (_list.IsFixedSize)
				{
					throw new NotSupportedException();//(Environment.GetResourceString("NotSupported_FixedSizeCollection"));
				}

				_list.Clear();
				_version++;
			}

			public override Object Clone()
			{
				// This does not do a shallow copy of _list into a ArrayList!
				// This clones the IListWrapper, creating another wrapper class!
				return new IListWrapper(_list);
			}

			public override bool Contains(Object obj)
			{
				return _list.Contains(obj);
			}

			public override void CopyTo(Array array, int index)
			{
				_list.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (array == null)
					throw new ArgumentNullException();// (nameof(array));
				if (index < 0 || arrayIndex < 0)
					throw new ArgumentOutOfRangeException();// (( index < 0 ) ? nameof(index) : nameof(arrayIndex), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				if (count < 0)
					throw new ArgumentOutOfRangeException();// (nameof(count), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				if (array.Length - arrayIndex < count)
					throw new ArgumentException();// (Environment.GetResourceString("Argument_InvalidOffLen"));
				if (array.Rank != 1)
					throw new ArgumentException();// (Environment.GetResourceString("Arg_RankMultiDimNotSupported"));
				Contract.EndContractBlock();

				if (_list.Count - index < count)
					throw new ArgumentException();// (Environment.GetResourceString("Argument_InvalidOffLen"));

				for (int i = index; i < index + count; i++)
					array.SetValue(_list[i], arrayIndex++);
			}

			public override IEnumerator GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();// (( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				Contract.EndContractBlock();
				if (_list.Count - index < count)
					throw new ArgumentException();// (Environment.GetResourceString("Argument_InvalidOffLen"));

				return new IListWrapperEnumWrapper(this, index, count);
			}

			public override int IndexOf(Object value)
			{
				return _list.IndexOf(value);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex)
			{
				return IndexOf(value, startIndex, _list.Count - startIndex);
			}

			public override int IndexOf(Object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > this.Count) throw new ArgumentOutOfRangeException();// (nameof(startIndex), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				if (count < 0 || startIndex > this.Count - count) throw new ArgumentOutOfRangeException();// (nameof(count), Environment.GetResourceString("ArgumentOutOfRange_Count"));
				Contract.EndContractBlock();

				int endIndex = startIndex + count;
				if (value == null)
				{
					for (int i = startIndex; i < endIndex; i++)
						if (_list[i] == null)
							return i;
					return -1;
				}
				else
				{
					for (int i = startIndex; i < endIndex; i++)
						if (_list[i] != null && _list[i].Equals(value))
							return i;
					return -1;
				}
			}

			public override void Insert(int index, Object obj)
			{
				_list.Insert(index, obj);
				_version++;
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (c == null)
					throw new ArgumentNullException();// (nameof(c), Environment.GetResourceString("ArgumentNull_Collection"));
				if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException();// (nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				Contract.EndContractBlock();

				if (c.Count > 0)
				{
					ArrayList al = _list as ArrayList;
					if (al != null)
					{
						// We need to special case ArrayList.
						// When c is a range of _list, we need to handle this in a special way.
						// See ArrayList.InsertRange for details.
						al.InsertRange(index, c);
					}
					else
					{
						IEnumerator en = c.GetEnumerator();
						while (en.MoveNext())
						{
							_list.Insert(index++, en.Current);
						}
					}
					_version++;
				}
			}

			public override int LastIndexOf(Object value)
			{
				return LastIndexOf(value, _list.Count - 1, _list.Count);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex, int count)
			{
				if (_list.Count == 0)
					return -1;

				if (startIndex < 0 || startIndex >= _list.Count) throw new ArgumentOutOfRangeException();// (nameof(startIndex), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				if (count < 0 || count > startIndex + 1) throw new ArgumentOutOfRangeException(); // (nameof(count), Environment.GetResourceString("ArgumentOutOfRange_Count"));

				int endIndex = startIndex - count + 1;
				if (value == null)
				{
					for (int i = startIndex; i >= endIndex; i--)
						if (_list[i] == null)
							return i;
					return -1;
				}
				else
				{
					for (int i = startIndex; i >= endIndex; i--)
						if (_list[i] != null && _list[i].Equals(value))
							return i;
					return -1;
				}
			}

			public override void Remove(Object value)
			{
				int index = IndexOf(value);
				if (index >= 0)
					RemoveAt(index);
			}

			public override void RemoveAt(int index)
			{
				_list.RemoveAt(index);
				_version++;
			}

			public override void RemoveRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();// (( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				Contract.EndContractBlock();
				if (_list.Count - index < count)
					throw new ArgumentException();// (Environment.GetResourceString("Argument_InvalidOffLen"));

				if (count > 0)    // be consistent with ArrayList
					_version++;

				while (count > 0)
				{
					_list.RemoveAt(index);
					count--;
				}
			}

			public override void Reverse(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();// (( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				Contract.EndContractBlock();
				if (_list.Count - index < count)
					throw new ArgumentException();//(Environment.GetResourceString("Argument_InvalidOffLen"));

				int i = index;
				int j = index + count - 1;
				while (i < j)
				{
					Object tmp = _list[i];
					_list[i++] = _list[j];
					_list[j--] = tmp;
				}
				_version++;
			}

			public override void SetRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException();//(nameof(c), Environment.GetResourceString("ArgumentNull_Collection"));
				}
				Contract.EndContractBlock();

				if (index < 0 || index > _list.Count - c.Count)
				{
					throw new ArgumentOutOfRangeException();//(nameof(index), Environment.GetResourceString("ArgumentOutOfRange_Index"));
				}

				if (c.Count > 0)
				{
					IEnumerator en = c.GetEnumerator();
					while (en.MoveNext())
					{
						_list[index++] = en.Current;
					}
					_version++;
				}
			}

			public override ArrayList GetRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();//(( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				Contract.EndContractBlock();
				if (_list.Count - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				return new Range(this, index, count);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new Exception("unimplemented");
			}

			public override Object[] ToArray()
			{
				Object[] array = new Object[Count];
				_list.CopyTo(array, 0);
				return array;
			}

			//[SecuritySafeCritical]
			public override Array ToArray(Type type)
			{
				throw new Exception("unimplemented");
			}

			public override void TrimToSize()
			{
				// Can't really do much here...
			}

			// This is the enumerator for an IList that's been wrapped in another
			// class that implements all of ArrayList's methods.
			[Serializable]
			private sealed class IListWrapperEnumWrapper : IEnumerator, ICloneable
			{
				private IEnumerator _en;
				private int _remaining;
				private int _initialStartIndex;   // for reset
				private int _initialCount;        // for reset
				private bool _firstCall;       // firstCall to MoveNext

				private IListWrapperEnumWrapper()
				{
				}

				internal IListWrapperEnumWrapper(IListWrapper listWrapper, int startIndex, int count)
				{
					_en = listWrapper.GetEnumerator();
					_initialStartIndex = startIndex;
					_initialCount = count;
					while (startIndex-- > 0 && _en.MoveNext()) ;
					_remaining = count;
					_firstCall = true;
				}

				public Object Clone()
				{
					// We must clone the underlying enumerator, I think.
					IListWrapperEnumWrapper clone = new IListWrapperEnumWrapper();
					clone._en = (IEnumerator)((ICloneable)_en).Clone();
					clone._initialStartIndex = _initialStartIndex;
					clone._initialCount = _initialCount;
					clone._remaining = _remaining;
					clone._firstCall = _firstCall;
					return clone;
				}

				public bool MoveNext()
				{
					if (_firstCall)
					{
						_firstCall = false;
						return _remaining-- > 0 && _en.MoveNext();
					}
					if (_remaining < 0)
						return false;
					bool r = _en.MoveNext();
					return r && _remaining-- > 0;
				}

				public Object Current
				{
					get
					{
						if (_firstCall)
							throw new InvalidOperationException(); // (Environment.GetResourceString(ResId.InvalidOperation_EnumNotStarted));
						if (_remaining < 0)
							throw new InvalidOperationException();// (Environment.GetResourceString(ResId.InvalidOperation_EnumEnded));
						return _en.Current;
					}
				}

				public void Reset()
				{
					_en.Reset();
					int startIndex = _initialStartIndex;
					while (startIndex-- > 0 && _en.MoveNext()) ;
					_remaining = _initialCount;
					_firstCall = true;
				}
			}
		}

		[Serializable]
		private class SyncArrayList : ArrayList
		{
			private readonly ArrayList _list;
			private readonly Object _root;

			internal SyncArrayList(ArrayList list)
				: base(false)
			{
				_list = list;
				_root = list.SyncRoot;
			}

			public override int Capacity
			{
				get
				{
					lock (_root)
					{
						return _list.Capacity;
					}
				}

				//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
				set
				{
					lock (_root)
					{
						_list.Capacity = value;
					}
				}
			}

			public override int Count
			{
				get { lock (_root) { return _list.Count; } }
			}

			public override bool IsReadOnly
			{
				get { return _list.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return _list.IsFixedSize; }
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override Object this[int index]
			{
				get
				{
					lock (_root)
					{
						return _list[index];
					}
				}
				set
				{
					lock (_root)
					{
						_list[index] = value;
					}
				}
			}

			public override Object SyncRoot
			{
				get { return _root; }
			}

			public override int Add(Object value)
			{
				lock (_root)
				{
					return _list.Add(value);
				}
			}

			public override void AddRange(ICollection c)
			{
				lock (_root)
				{
					_list.AddRange(c);
				}
			}

			public override int BinarySearch(Object value)
			{
				lock (_root)
				{
					return _list.BinarySearch(value);
				}
			}

			public override int BinarySearch(Object value, IComparer comparer)
			{
				lock (_root)
				{
					return _list.BinarySearch(value, comparer);
				}
			}

			//	[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int BinarySearch(int index, int count, Object value, IComparer comparer)
			{
				lock (_root)
				{
					return _list.BinarySearch(index, count, value, comparer);
				}
			}

			public override void Clear()
			{
				lock (_root)
				{
					_list.Clear();
				}
			}

			public override Object Clone()
			{
				lock (_root)
				{
					return new SyncArrayList((ArrayList)_list.Clone());
				}
			}

			public override bool Contains(Object item)
			{
				lock (_root)
				{
					return _list.Contains(item);
				}
			}

			public override void CopyTo(Array array)
			{
				lock (_root)
				{
					_list.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (_root)
				{
					_list.CopyTo(array, index);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				lock (_root)
				{
					_list.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (_root)
				{
					return _list.GetEnumerator();
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override IEnumerator GetEnumerator(int index, int count)
			{
				lock (_root)
				{
					return _list.GetEnumerator(index, count);
				}
			}

			public override int IndexOf(Object value)
			{
				lock (_root)
				{
					return _list.IndexOf(value);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex)
			{
				lock (_root)
				{
					return _list.IndexOf(value, startIndex);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex, int count)
			{
				lock (_root)
				{
					return _list.IndexOf(value, startIndex, count);
				}
			}

			public override void Insert(int index, Object value)
			{
				lock (_root)
				{
					_list.Insert(index, value);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void InsertRange(int index, ICollection c)
			{
				lock (_root)
				{
					_list.InsertRange(index, c);
				}
			}

			public override int LastIndexOf(Object value)
			{
				lock (_root)
				{
					return _list.LastIndexOf(value);
				}
			}

			//	[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex)
			{
				lock (_root)
				{
					return _list.LastIndexOf(value, startIndex);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex, int count)
			{
				lock (_root)
				{
					return _list.LastIndexOf(value, startIndex, count);
				}
			}

			public override void Remove(Object value)
			{
				lock (_root)
				{
					_list.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (_root)
				{
					_list.RemoveAt(index);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void RemoveRange(int index, int count)
			{
				lock (_root)
				{
					_list.RemoveRange(index, count);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Reverse(int index, int count)
			{
				lock (_root)
				{
					_list.Reverse(index, count);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void SetRange(int index, ICollection c)
			{
				lock (_root)
				{
					_list.SetRange(index, c);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override ArrayList GetRange(int index, int count)
			{
				lock (_root)
				{
					return _list.GetRange(index, count);
				}
			}

			public override void Sort()
			{
				lock (_root)
				{
					_list.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				lock (_root)
				{
					_list.Sort(comparer);
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Sort(int index, int count, IComparer comparer)
			{
				lock (_root)
				{
					_list.Sort(index, count, comparer);
				}
			}

			public override Object[] ToArray()
			{
				lock (_root)
				{
					return _list.ToArray();
				}
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override Array ToArray(Type type)
			{
				lock (_root)
				{
					return _list.ToArray(type);
				}
			}

			public override void TrimToSize()
			{
				lock (_root)
				{
					_list.TrimToSize();
				}
			}
		}

		[Serializable]
		private class SyncIList : IList
		{
			private readonly IList _list;
			private readonly Object _root;

			internal SyncIList(IList list)
			{
				_list = list;
				_root = list.SyncRoot;
			}

			public virtual int Count
			{
				get { lock (_root) { return _list.Count; } }
			}

			public virtual bool IsReadOnly
			{
				get { return _list.IsReadOnly; }
			}

			public virtual bool IsFixedSize
			{
				get { return _list.IsFixedSize; }
			}

			public virtual bool IsSynchronized
			{
				get { return true; }
			}

			public virtual Object this[int index]
			{
				get
				{
					lock (_root)
					{
						return _list[index];
					}
				}
				set
				{
					lock (_root)
					{
						_list[index] = value;
					}
				}
			}

			public virtual Object SyncRoot
			{
				get { return _root; }
			}

			public virtual int Add(Object value)
			{
				lock (_root)
				{
					return _list.Add(value);
				}
			}

			public virtual void Clear()
			{
				lock (_root)
				{
					_list.Clear();
				}
			}

			public virtual bool Contains(Object item)
			{
				lock (_root)
				{
					return _list.Contains(item);
				}
			}

			public virtual void CopyTo(Array array, int index)
			{
				lock (_root)
				{
					_list.CopyTo(array, index);
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				lock (_root)
				{
					return _list.GetEnumerator();
				}
			}

			public virtual int IndexOf(Object value)
			{
				lock (_root)
				{
					return _list.IndexOf(value);
				}
			}

			public virtual void Insert(int index, Object value)
			{
				lock (_root)
				{
					_list.Insert(index, value);
				}
			}

			public virtual void Remove(Object value)
			{
				lock (_root)
				{
					_list.Remove(value);
				}
			}

			public virtual void RemoveAt(int index)
			{
				lock (_root)
				{
					_list.RemoveAt(index);
				}
			}
		}

		[Serializable]
		private class FixedSizeList : IList
		{
			private readonly IList _list;

			internal FixedSizeList(IList l)
			{
				_list = l;
			}

			public virtual int Count
			{
				get { return _list.Count; }
			}

			public virtual bool IsReadOnly
			{
				get { return _list.IsReadOnly; }
			}

			public virtual bool IsFixedSize
			{
				get { return true; }
			}

			public virtual bool IsSynchronized
			{
				get { return _list.IsSynchronized; }
			}

			public virtual Object this[int index]
			{
				get
				{
					return _list[index];
				}
				set
				{
					_list[index] = value;
				}
			}

			public virtual Object SyncRoot
			{
				get { return _list.SyncRoot; }
			}

			public virtual int Add(Object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public virtual void Clear()
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public virtual bool Contains(Object obj)
			{
				return _list.Contains(obj);
			}

			public virtual void CopyTo(Array array, int index)
			{
				_list.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			public virtual int IndexOf(Object value)
			{
				return _list.IndexOf(value);
			}

			public virtual void Insert(int index, Object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public virtual void Remove(Object value)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}
		}

		[Serializable]
		private class FixedSizeArrayList : ArrayList
		{
			private ArrayList _list;

			internal FixedSizeArrayList(ArrayList l)
			{
				_list = l;
				_version = _list._version;
			}

			public override int Count
			{
				get { return _list.Count; }
			}

			public override bool IsReadOnly
			{
				get { return _list.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return true; }
			}

			public override bool IsSynchronized
			{
				get { return _list.IsSynchronized; }
			}

			public override Object this[int index]
			{
				get
				{
					return _list[index];
				}
				set
				{
					_list[index] = value;
					_version = _list._version;
				}
			}

			public override Object SyncRoot
			{
				get { return _list.SyncRoot; }
			}

			public override int Add(Object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int BinarySearch(int index, int count, Object value, IComparer comparer)
			{
				return _list.BinarySearch(index, count, value, comparer);
			}

			public override int Capacity
			{
				get { return _list.Capacity; }

				//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
				set { throw new NotSupportedException("NotSupported_FixedSizeCollection"); }
			}

			public override void Clear()
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override Object Clone()
			{
				FixedSizeArrayList arrayList = new FixedSizeArrayList(_list);
				arrayList._list = (ArrayList)_list.Clone();
				return arrayList;
			}

			public override bool Contains(Object obj)
			{
				return _list.Contains(obj);
			}

			public override void CopyTo(Array array, int index)
			{
				_list.CopyTo(array, index);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				_list.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override IEnumerator GetEnumerator(int index, int count)
			{
				return _list.GetEnumerator(index, count);
			}

			public override int IndexOf(Object value)
			{
				return _list.IndexOf(value);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex)
			{
				return _list.IndexOf(value, startIndex);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex, int count)
			{
				return _list.IndexOf(value, startIndex, count);
			}

			public override void Insert(int index, Object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override int LastIndexOf(Object value)
			{
				return _list.LastIndexOf(value);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex)
			{
				return _list.LastIndexOf(value, startIndex);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex, int count)
			{
				return _list.LastIndexOf(value, startIndex, count);
			}

			public override void Remove(Object value)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void SetRange(int index, ICollection c)
			{
				_list.SetRange(index, c);
				_version = _list._version;
			}

			public override ArrayList GetRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException();// (( index < 0 ? nameof(index) : nameof(count) ), Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				if (Count - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				return new Range(this, index, count);
			}

			//	[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Reverse(int index, int count)
			{
				_list.Reverse(index, count);
				_version = _list._version;
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Sort(int index, int count, IComparer comparer)
			{
				_list.Sort(index, count, comparer);
				_version = _list._version;
			}

			public override Object[] ToArray()
			{
				return _list.ToArray();
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override Array ToArray(Type type)
			{
				return _list.ToArray(type);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}
		}

		[Serializable]
		private class ReadOnlyList : IList
		{
			private readonly IList _list;

			internal ReadOnlyList(IList l)
			{
				_list = l;
			}

			public virtual int Count
			{
				get { return _list.Count; }
			}

			public virtual bool IsReadOnly
			{
				get { return true; }
			}

			public virtual bool IsFixedSize
			{
				get { return true; }
			}

			public virtual bool IsSynchronized
			{
				get { return _list.IsSynchronized; }
			}

			public virtual Object this[int index]
			{
				get
				{
					return _list[index];
				}
				set
				{
					throw new NotSupportedException("NotSupported_ReadOnlyCollection");
				}
			}

			public virtual Object SyncRoot
			{
				get { return _list.SyncRoot; }
			}

			public virtual int Add(Object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public virtual void Clear()
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public virtual bool Contains(Object obj)
			{
				return _list.Contains(obj);
			}

			public virtual void CopyTo(Array array, int index)
			{
				_list.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			public virtual int IndexOf(Object value)
			{
				return _list.IndexOf(value);
			}

			public virtual void Insert(int index, Object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public virtual void Remove(Object value)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}
		}

		[Serializable]
		private class ReadOnlyArrayList : ArrayList
		{
			private ArrayList _list;

			internal ReadOnlyArrayList(ArrayList l)
			{
				_list = l;
			}

			public override int Count
			{
				get { return _list.Count; }
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override bool IsFixedSize
			{
				get { return true; }
			}

			public override bool IsSynchronized
			{
				get { return _list.IsSynchronized; }
			}

			public override Object this[int index]
			{
				get
				{
					return _list[index];
				}
				set
				{
					throw new NotSupportedException("NotSupported_ReadOnlyCollection");
				}
			}

			public override Object SyncRoot
			{
				get { return _list.SyncRoot; }
			}

			public override int Add(Object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int BinarySearch(int index, int count, Object value, IComparer comparer)
			{
				return _list.BinarySearch(index, count, value, comparer);
			}

			public override int Capacity
			{
				get { return _list.Capacity; }

				//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
				set { throw new NotSupportedException("NotSupported_ReadOnlyCollection"); }
			}

			public override void Clear()
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override Object Clone()
			{
				ReadOnlyArrayList arrayList = new ReadOnlyArrayList(_list);
				arrayList._list = (ArrayList)_list.Clone();
				return arrayList;
			}

			public override bool Contains(Object obj)
			{
				return _list.Contains(obj);
			}

			public override void CopyTo(Array array, int index)
			{
				_list.CopyTo(array, index);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				_list.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override IEnumerator GetEnumerator(int index, int count)
			{
				return _list.GetEnumerator(index, count);
			}

			public override int IndexOf(Object value)
			{
				return _list.IndexOf(value);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex)
			{
				return _list.IndexOf(value, startIndex);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int IndexOf(Object value, int startIndex, int count)
			{
				return _list.IndexOf(value, startIndex, count);
			}

			public override void Insert(int index, Object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override int LastIndexOf(Object value)
			{
				return _list.LastIndexOf(value);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex)
			{
				return _list.LastIndexOf(value, startIndex);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex, int count)
			{
				return _list.LastIndexOf(value, startIndex, count);
			}

			public override void Remove(Object value)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override ArrayList GetRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (Count - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				return new Range(this, index, count);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override Object[] ToArray()
			{
				return _list.ToArray();
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override Array ToArray(Type type)
			{
				return _list.ToArray(type);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}
		}

		// Implements an enumerator for a ArrayList. The enumerator uses the
		// internal version number of the list to ensure that no modifications are
		// made to the list while an enumeration is in progress.
		[Serializable]
		private sealed class ArrayListEnumerator : IEnumerator, ICloneable
		{
			private readonly ArrayList list;
			private int index;
			private readonly int endIndex;       // Where to stop.
			private readonly int version;
			private Object currentElement;
			private readonly int startIndex;     // Save this for Reset.

			internal ArrayListEnumerator(ArrayList list, int index, int count)
			{
				this.list = list;
				startIndex = index;
				this.index = index - 1;
				endIndex = this.index + count;  // last valid index
				version = list._version;
				currentElement = null;
			}

			public Object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (version != list._version) throw new InvalidOperationException();// Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
				if (index < endIndex)
				{
					currentElement = list[++index];
					return true;
				}
				else
				{
					index = endIndex + 1;
				}

				return false;
			}

			public Object Current
			{
				get
				{
					if (index < startIndex)
						throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumNotStarted));
					else if (index > endIndex)
					{
						throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumEnded));
					}
					return currentElement;
				}
			}

			public void Reset()
			{
				if (version != list._version) throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
				index = startIndex - 1;
			}
		}

		// Implementation of a generic list subrange. An instance of this class
		// is returned by the default implementation of List.GetRange.
		[Serializable]
		private class Range : ArrayList
		{
			private ArrayList _baseList;
			private readonly int _baseIndex;

			[ContractPublicPropertyName("Count")]
			private int _baseSize;

			private int _baseVersion;

			internal Range(ArrayList list, int index, int count) : base(false)
			{
				_baseList = list;
				_baseIndex = index;
				_baseSize = count;
				_baseVersion = list._version;

				// we also need to update _version field to make Range of Range work
				_version = list._version;
			}

			private void InternalUpdateRange()
			{
				if (_baseVersion != _baseList._version)
					throw new InvalidOperationException("InvalidOperation_UnderlyingArrayListChanged");
			}

			private void InternalUpdateVersion()
			{
				_baseVersion++;
				_version++;
			}

			public override int Add(Object value)
			{
				InternalUpdateRange();
				_baseList.Insert(_baseIndex + _baseSize, value);
				InternalUpdateVersion();
				return _baseSize++;
			}

			public override void AddRange(ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException(nameof(c));
				}
				Contract.EndContractBlock();

				InternalUpdateRange();
				int count = c.Count;
				if (count > 0)
				{
					_baseList.InsertRange(_baseIndex + _baseSize, c);
					InternalUpdateVersion();
					_baseSize += count;
				}
			}

			// Other overloads with automatically work
			public override int BinarySearch(int index, int count, Object value, IComparer comparer)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();
				InternalUpdateRange();

				int i = _baseList.BinarySearch(_baseIndex + index, count, value, comparer);
				if (i >= 0) return i - _baseIndex;
				return i + _baseIndex;
			}

			public override int Capacity
			{
				get
				{
					return _baseList.Capacity;
				}

				set
				{
					if (value < Count) throw new ArgumentOutOfRangeException(nameof(value), "ArgumentOutOfRange_SmallCapacity");
					Contract.EndContractBlock();
				}
			}

			public override void Clear()
			{
				InternalUpdateRange();
				if (_baseSize != 0)
				{
					_baseList.RemoveRange(_baseIndex, _baseSize);
					InternalUpdateVersion();
					_baseSize = 0;
				}
			}

			public override Object Clone()
			{
				InternalUpdateRange();
				Range arrayList = new Range(_baseList, _baseIndex, _baseSize);
				arrayList._baseList = (ArrayList)_baseList.Clone();
				return arrayList;
			}

			public override bool Contains(Object item)
			{
				InternalUpdateRange();
				if (item == null)
				{
					for (int i = 0; i < _baseSize; i++)
						if (_baseList[_baseIndex + i] == null)
							return true;
					return false;
				}
				else
				{
					for (int i = 0; i < _baseSize; i++)
						if (_baseList[_baseIndex + i] != null && _baseList[_baseIndex + i].Equals(item))
							return true;
					return false;
				}
			}

			public override void CopyTo(Array array, int index)
			{
				if (array == null)
					throw new ArgumentNullException(nameof(array));
				if (array.Rank != 1)
					throw new ArgumentException("Arg_RankMultiDimNotSupported");
				if (index < 0)
					throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_NeedNonNegNum");
				if (array.Length - index < _baseSize)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.CopyTo(_baseIndex, array, index, _baseSize);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (array == null)
					throw new ArgumentNullException(nameof(array));
				if (array.Rank != 1)
					throw new ArgumentException("Arg_RankMultiDimNotSupported");
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (array.Length - arrayIndex < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.CopyTo(_baseIndex + index, array, arrayIndex, count);
			}

			public override int Count
			{
				get
				{
					InternalUpdateRange();
					return _baseSize;
				}
			}

			public override bool IsReadOnly
			{
				get { return _baseList.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return _baseList.IsFixedSize; }
			}

			public override bool IsSynchronized
			{
				get { return _baseList.IsSynchronized; }
			}

			public override IEnumerator GetEnumerator()
			{
				return GetEnumerator(0, _baseSize);
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				return _baseList.GetEnumerator(_baseIndex + index, count);
			}

			public override ArrayList GetRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				return new Range(this, index, count);
			}

			public override Object SyncRoot
			{
				get
				{
					return _baseList.SyncRoot;
				}
			}

			public override int IndexOf(Object value)
			{
				InternalUpdateRange();
				int i = _baseList.IndexOf(value, _baseIndex, _baseSize);
				if (i >= 0) return i - _baseIndex;
				return -1;
			}

			public override int IndexOf(Object value, int startIndex)
			{
				if (startIndex < 0)
					throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_NeedNonNegNum");
				if (startIndex > _baseSize)
					throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_Index");
				Contract.EndContractBlock();

				InternalUpdateRange();
				int i = _baseList.IndexOf(value, _baseIndex + startIndex, _baseSize - startIndex);
				if (i >= 0) return i - _baseIndex;
				return -1;
			}

			public override int IndexOf(Object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > _baseSize)
					throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_Index");

				if (count < 0 || (startIndex > _baseSize - count))
					throw new ArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_Count");
				Contract.EndContractBlock();

				InternalUpdateRange();
				int i = _baseList.IndexOf(value, _baseIndex + startIndex, count);
				if (i >= 0) return i - _baseIndex;
				return -1;
			}

			public override void Insert(int index, Object value)
			{
				if (index < 0 || index > _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.Insert(_baseIndex + index, value);
				InternalUpdateVersion();
				_baseSize++;
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (index < 0 || index > _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
				if (c == null)
				{
					throw new ArgumentNullException(nameof(c));
				}
				Contract.EndContractBlock();

				InternalUpdateRange();
				int count = c.Count;
				if (count > 0)
				{
					_baseList.InsertRange(_baseIndex + index, c);
					_baseSize += count;
					InternalUpdateVersion();
				}
			}

			public override int LastIndexOf(Object value)
			{
				InternalUpdateRange();
				int i = _baseList.LastIndexOf(value, _baseIndex + _baseSize - 1, _baseSize);
				if (i >= 0) return i - _baseIndex;
				return -1;
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override int LastIndexOf(Object value, int startIndex, int count)
			{
				InternalUpdateRange();
				if (_baseSize == 0)
					return -1;

				if (startIndex >= _baseSize)
					throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_Index");
				if (startIndex < 0)
					throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_NeedNonNegNum");

				int i = _baseList.LastIndexOf(value, _baseIndex + startIndex, count);
				if (i >= 0) return i - _baseIndex;
				return -1;
			}

			// Don't need to override Remove

			public override void RemoveAt(int index)
			{
				if (index < 0 || index >= _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.RemoveAt(_baseIndex + index);
				InternalUpdateVersion();
				_baseSize--;
			}

			public override void RemoveRange(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();

				// No need to call _bastList.RemoveRange if count is 0.
				// In addition, _baseList won't change the vresion number if count is 0.
				if (count > 0)
				{
					_baseList.RemoveRange(_baseIndex + index, count);
					InternalUpdateVersion();
					_baseSize -= count;
				}
			}

			public override void Reverse(int index, int count)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.Reverse(_baseIndex + index, count);
				InternalUpdateVersion();
			}

			//[SuppressMessage("Microsoft.Contracts", "CC1055")]  // Skip extra error checking to avoid *potential* AppCompat problems.
			public override void SetRange(int index, ICollection c)
			{
				InternalUpdateRange();
				if (index < 0 || index >= _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
				_baseList.SetRange(_baseIndex + index, c);
				if (c.Count > 0)
				{
					InternalUpdateVersion();
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				if (index < 0 || count < 0)
					throw new ArgumentOutOfRangeException((index < 0 ? nameof(index) : nameof(count)), "ArgumentOutOfRange_NeedNonNegNum");
				if (_baseSize - index < count)
					throw new ArgumentException("Argument_InvalidOffLen");
				Contract.EndContractBlock();

				InternalUpdateRange();
				_baseList.Sort(_baseIndex + index, count, comparer);
				InternalUpdateVersion();
			}

			public override Object this[int index]
			{
				get
				{
					InternalUpdateRange();
					if (index < 0 || index >= _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
					return _baseList[_baseIndex + index];
				}
				set
				{
					InternalUpdateRange();
					if (index < 0 || index >= _baseSize) throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
					_baseList[_baseIndex + index] = value;
					InternalUpdateVersion();
				}
			}

			public override Object[] ToArray()
			{
				InternalUpdateRange();
				Object[] array = new Object[_baseSize];
				Array.Copy(_baseList._items, _baseIndex, array, 0, _baseSize);
				return array;
			}

			//[SecuritySafeCritical]
			public override Array ToArray(Type type)
			{
				throw new Exception("unimplemented");
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("NotSupported_RangeCollection");
			}
		}

		[Serializable]
		private sealed class ArrayListEnumeratorSimple : IEnumerator, ICloneable
		{
			private readonly ArrayList list;
			private int index;
			private readonly int version;
			private Object currentElement;
			private readonly bool isArrayList;

			// this object is used to indicate enumeration has not started or has terminated
			private static readonly Object dummyObject = new Object();

			internal ArrayListEnumeratorSimple(ArrayList list)
			{
				this.list = list;
				this.index = -1;
				version = list._version;
				isArrayList = (list.GetType() == typeof(ArrayList));
				currentElement = dummyObject;
			}

			public Object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (version != list._version)
				{
					throw new InvalidOperationException();// Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
				}

				if (isArrayList)
				{  // avoid calling virtual methods if we are operating on ArrayList to improve performance
					if (index < list._size - 1)
					{
						currentElement = list._items[++index];
						return true;
					}
					else
					{
						currentElement = dummyObject;
						index = list._size;
						return false;
					}
				}
				else
				{
					if (index < list.Count - 1)
					{
						currentElement = list[++index];
						return true;
					}
					else
					{
						index = list.Count;
						currentElement = dummyObject;
						return false;
					}
				}
			}

			public Object Current
			{
				get
				{
					object temp = currentElement;
					if (dummyObject == temp)
					{ // check if enumeration has not started or has terminated
						if (index == -1)
						{
							throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumNotStarted));
						}
						else
						{
							throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumEnded));
						}
					}

					return temp;
				}
			}

			public void Reset()
			{
				if (version != list._version)
				{
					throw new InvalidOperationException();//(Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
				}

				currentElement = dummyObject;
				index = -1;
			}
		}

		internal class ArrayListDebugView
		{
			private readonly ArrayList arrayList;

			public ArrayListDebugView(ArrayList arrayList)
			{
				if (arrayList == null)
					throw new ArgumentNullException(nameof(arrayList));

				this.arrayList = arrayList;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public Object[] Items
			{
				get
				{
					return arrayList.ToArray();
				}
			}
		}
	}
}
