using System;
using System.Collections;
using System.Text;

namespace BarryJones.PDF.Types {
	//
	// The PdfDictionary object is an associative collection of key/value pairs. The dictionary is
	// represented like:
	// << key value
	//    key value
	// >>
	//
	public class PdfDictionary : PdfObject, IDictionary {

		private int count;
		private int version;
		private int maxElements;
		private DictionaryEntry [] list;
		private const int DEF_NUM_ELEMENTS = 16;

		public PdfDictionary() {
			list = new DictionaryEntry[DEF_NUM_ELEMENTS];
			maxElements = DEF_NUM_ELEMENTS;
		}

		public PdfDictionary(int numStartingElements) {
			list = new DictionaryEntry[numStartingElements];
			maxElements = numStartingElements;
		}

		#region IDictionary Members
		public bool IsReadOnly {
			get { return false; }
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			return new PdfDictionaryEnumerator(this);
		}
		
		public PdfObject this[PdfName key] {
			get { return (PdfObject)((IDictionary)this)[key]; }
			set { ((IDictionary)this)[key] = value; }
		}

		object IDictionary.this[object key]	{
			get {
				foreach(DictionaryEntry de in list) {
					if(de.Key == key) return de.Value;
				}
				return null;
			}
			set {
				DictionaryEntry temp = new DictionaryEntry();
				// TODO:  Add PdfDictionary.this setter implementation
				foreach(DictionaryEntry de in list) {
					if(de.Key == key) {
						temp = de;
					}
				}
				temp.Value = value;
			}
		}

		public void Remove(PdfName key) {
			((IDictionary)this).Remove(key);
		}

		void IDictionary.Remove(object key) {
			int index = -1;

			// Check if the item exists in the collection
			for(int n = 0; n < Count; n++) {
				if(list[n].Key.Equals(key)) {
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

		public bool Contains(PdfName key) {
			return ((IDictionary)this).Contains(key);
		}

		bool IDictionary.Contains(object key) {
			foreach(DictionaryEntry de in list) {
				if(de.Key == key) return true;
			}
			return false;
		}

		public void Clear() {
			list = new DictionaryEntry[maxElements];
			version++;
			count = 0;
		}

		public ICollection Values {
			get {
				ArrayList ret = new ArrayList();
				foreach(DictionaryEntry de in list) {
					ret.Add(de.Value);
				}

				return ret;
			}
		}

		public void Add(PdfName key, PdfObject value) {
			((IDictionary)this).Add(key, value);
		}

		void IDictionary.Add(object key, object value) {
			// Check if we have enough space, if not make more
			if(Count >= maxElements) {
				ReBoundArray();
			}

			version++;	// Increase version number to invalidate any running enumerators

			list[Count] = new DictionaryEntry(key, value);
			count++;
		}

		public ICollection Keys {
			get {
				ArrayList ret = new ArrayList();
				foreach(DictionaryEntry de in list) {
					ret.Add(de.Key);
				}

				return ret;
			}
		}

		public bool IsFixedSize {
			get { return false; }
		}

		public class PdfDictionaryEnumerator : IDictionaryEnumerator {
			private PdfDictionary dictionary;
			private int startVersion;
			private object current;
			private int index;

			public PdfDictionaryEnumerator(PdfDictionary dictionary) {
				this.dictionary = dictionary;
				this.startVersion = dictionary.version;
			}

			public object Key {
				get {
					if(current == dictionary) 
						throw new InvalidCastException("Enumerator not begun");

					return ((DictionaryEntry)current).Key;
				}
			}

			public object Value {
				get {
					if(current == dictionary) 
						throw new InvalidOperationException("Enumerator not begun");

					return ((DictionaryEntry)current).Value;
				}
			}

			public DictionaryEntry Entry {
				get {
					if(current == dictionary) 
						throw new InvalidOperationException("Enumerator not begun");

					return (DictionaryEntry)current;
				}
			}

			public void Reset() {
				// Check if enumerator is still valid
				if(startVersion != dictionary.version) 
					throw new InvalidOperationException("Enumerator invalidated");

				// Reset the enumerator to the begining of the collection
				current = dictionary;
				index = -1;				// Place index before first element
			}

			public object Current {
				get {
					object temp = current;
					if(temp == dictionary) {
						if(index == -1)
							throw new InvalidOperationException("Enumerator not started");
						else
							throw new InvalidOperationException("Enumerator not begun");
					}

					return temp;
				}
			}

			public bool MoveNext() {
				if(startVersion != dictionary.version) 
					throw new InvalidOperationException("The enumerator is invalid");
				if(index <= (dictionary.Count-1)) {
					current = dictionary.list[index];
					index++;
					return true;
				}
				else {
					current = dictionary;
					index = dictionary.Count;
				}
				return false;
			}
		}
		#endregion

		#region ICollection Members
		public bool IsSynchronized {
			get { return false; }
		}

		public int Count {
			get { return count; }
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

		public object SyncRoot {
			get { return this; }
		}

		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new PdfDictionaryEnumerator(this);
		}
		#endregion

		protected virtual void ReBoundArray() {
			// Increment the maximum number of elements contained in the array
			maxElements *=2;

			// Move all the elements from the old array into an ArrayList
			ArrayList tempContainer = new ArrayList();
			foreach(DictionaryEntry att in list) {
				tempContainer.Add(att);
			}

			// Redimension the array
			list = new DictionaryEntry[maxElements];

			// Replace all elements back into the array
			for(int n = 0; n < tempContainer.Count; n++) {
				list[n] = (DictionaryEntry)tempContainer[n];
			}
		}

		public override void Write(System.IO.StreamWriter stream) {
			stream.Write("<< ");
			// Write each dictionary entry to the stream
			foreach(DictionaryEntry de in this) { 
				((PdfObject)de.Key).Write(stream);
				stream.Write(" ");
				((PdfObject)de.Value).Write(stream);
				stream.Write("\n");
			}
			stream.Write(">>");
		}
	}
}
