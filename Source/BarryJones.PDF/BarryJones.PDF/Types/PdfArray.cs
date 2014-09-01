using System;
using System.Collections;
using System.IO;
using System.Text;

namespace BarryJones.PDF.Types {
	//
	// A PDF Array object allows the storage of a sequential list of PdfObjects, this can include such things
	// as text, numbers, booleans, dictionaries and other arrays.
	// 
	// An array is displayed as a space delimeted list inside square brackets:
	// [ 0.034 /Name false [ true 3344 ] ]
	// 
	// PDF Arrays only support single dimension arrays but larger dimensions can be constructed by inserting
	// arrays inside arrays.
	//
	internal class PdfArray : PdfObject, ICollection, IList {

		private PdfObject [] list;
		private int version;
		private int count;
		private int maxElements;
		private const int DEF_NUM_ELEMENTS = 16;

		public PdfArray() {
			list = new PdfObject[DEF_NUM_ELEMENTS];
			maxElements = DEF_NUM_ELEMENTS;
		}

		public PdfArray(int numStartingElements) {
			list = new PdfObject[numStartingElements];
			maxElements = numStartingElements;
		}

		public PdfArray(PdfObject [] items) {
			list = new PdfObject[ items.Length ];
			maxElements = items.Length;
			foreach(PdfObject pdfO in items) {
				this.Add(pdfO);
			}
		}

		#region ICollection Interface
		public int Count {
			get {  return count; }
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		public void CopyTo(Array array, int index) {
			// Check for invalid parameters
			if(null == array)	throw new NullReferenceException("Array cannot be null");
			if(index < 0)	throw new ArgumentOutOfRangeException("index", index, "Index must be greater than zero");
			if(array.Rank > 1) throw new ArgumentException("Array cannot be multidimensional", "array");
			if(index > Count) throw new ArgumentException("Index is greater than bounds of collection", "index");
			if(array.GetLength(0) - index < Count) throw new ArgumentException("Array is not big enough to contain collections elements", "array");

			// Perform the copy
			for(int n = 0; n <= Count; n++) {
				array.SetValue(list[n], index + n);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new PdfArrayEnumerator(this);
		}

		public class PdfArrayEnumerator : IEnumerator {
			private PdfArray list;
			private int startVersion;
			private int index;
			private object currentElement;

			public PdfArrayEnumerator(PdfArray collection) {
				list = collection;
				startVersion = list.version;
				index = -1;
				currentElement = list;
			}

			/// <summary>
			/// Gets the current element pointed to by the enumerator
			/// </summary>
			public object Current {
				get {
					object temp = currentElement;
					if(temp == list) {
						if(index == -1)
							throw new InvalidOperationException("Enumerator not started");
						else
							throw new InvalidOperationException("Enumerator not begun");
					}
					return temp;
				}
			}

			// move the iterator to the next element in the collection
			public bool MoveNext() {
				// check if the enumerated collection has changed
				if(startVersion != list.version) 
					throw new InvalidOperationException("Enumerator invalidated");

				// Can we move to the next element if so move to it and incrment the index counter
				if(index < (list.Count - 1)) {
					index++;
					currentElement = list[index];
					return true;
				}
				else {
					currentElement = list;
					index = list.Count;
				}
				return false;
			}

			// reset the position of the enumerator to the begining of the collection
			public void Reset() {
				// Check if enumerator is still valid
				if(startVersion != list.version) 
					throw new InvalidOperationException("Enumerator invalidated");

				// Reset the enumerator to the begining of the collection
				currentElement = list;
				index = -1;	// Place index before first element
			}
		}
		#endregion

		#region IList Interface
		public bool IsFixedSize { get { return false; } }
		public bool IsReadOnly { get { return false; } }

		// hidden indexor
		object IList.this[int index] {
			get {
				if(index > maxElements || index < 0) 
					throw new ArgumentOutOfRangeException("index", index, "Index out of valid range");
				return list[index];
			}
			set {
				if(index > maxElements || index < 0) 
					throw new ArgumentOutOfRangeException("index", index, "Index out of valid range");
				list[index] = (PdfObject)value;
			}
		}

		// typed indexor
		public PdfObject this[int index] {
			get {
				return (PdfObject)((IList)this)[index];
			}
			set {
				((IList)this)[index] = value;
			}
		}

		int IList.Add(object item) {
			if(Count >= maxElements) {		// Do some bounds checking
				ReBoundArray();
			}
			version++;						// Increase the version to inform enumerators changes have been made
			list[Count] = (PdfObject)item;	// Add the element onto the array
			count++;

			return Count-1;
		}

		public int Add(PdfObject item) { 
			return ((IList)this).Add(item); 
		}

		public void Clear() {
			for(int n = 0; n < Count; n++) {
				list[n] = null;
			}
			version++;						// increment version to invalidate enuemrators
			count = 0;
		}

		bool IList.Contains(object item) {
			foreach(object att in this) {
				if(att.Equals(item)) return true;
			}
			return false;
		}

		public bool Contains(PdfObject item) {
			return ((IList)this).Contains(item);
		}

		int IList.IndexOf(object item) {
			int retVal = -1;

			for(int n = 0; n < Count; n++) {
				if(list[n].Equals((object)item)) {
					retVal = n;
					break;
				}
			}
			return retVal;
		}

		public int IndexOf(PdfObject item) {
			return ((IList)this).IndexOf(item);
		}

		void IList.Insert(int index, object item) {
			if(index >= Count) 
				throw new ArgumentOutOfRangeException("index", item, "Index is outside bounds of array");
			if((Count + 1) >= maxElements) 
				ReBoundArray();

			// Iterate through the array backwards moving each item up one place then add the
			// item to the specified index
			for(int n = Count; n >= index; n--) {
				list[n+1] = list[n];
			}
			list[index] = (PdfObject)item;
			count++;
			version++;
		}

		public void Insert(int index, PdfObject item) {
			((IList)this).Insert(index, item);
		}

		void IList.Remove(object item) {
			int index = -1;

			// Check if the item exists in the collection
			for(int n = 0; n < Count; n++) {
				if(list[n].Equals(item)) {
					index = n;
					break;
				}
			}

			if(-1 != index) {
				for(int n = index; n < Count; n++) {
					list[n] = list[n+1];
				}
				count--;
				version++;
			}
		}

		public void Remove(PdfObject item) {
			((IList)this).Remove(item);
		}

		public void RemoveAt(int index) {
			if(index >= Count) 
				throw new ArgumentOutOfRangeException("index", index, "Index is outside bounds of array");
			Remove(list[index]);
			version++;
		}
		#endregion

		protected virtual void ReBoundArray() {
			// Increment the maximum number of elements contained in the array
			maxElements *= 2;

			// Move all the elements from the old array into an ArrayList
			ArrayList tempContainer = new ArrayList();
			foreach(PdfObject att in list) {
				tempContainer.Add(att);
			}

			// Redimension the array
			list = new PdfObject[maxElements];

			// Replace all elements back into the array
			for(int n = 0; n < tempContainer.Count; n++) {
				list[n] = (PdfObject)tempContainer[n];
			}
		}

		/// <summary>
		/// Writes the object to the provided stream
		/// </summary>
		public override void Write(StreamWriter stream) {
			bool first = true;

			stream.Write("[");
			foreach(PdfObject o in this) {
				if(!first)
					stream.Write(" ");
				o.Write(stream);				// Make sure all array elements are written to the stream

				if(first) 
					first = false;
			}
			stream.Write("]");
		}
	}
}
