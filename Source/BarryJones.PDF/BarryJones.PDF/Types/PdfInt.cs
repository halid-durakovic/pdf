using System;
using System.Text;

namespace BarryJones.PDF.Types {

	public sealed class PdfInt : PdfNumeric {
		private int internalValue;

		public PdfInt() {
		}

		public PdfInt(int initialValue) {
			this.internalValue = initialValue;
		}

		public int Value {
			get { return internalValue; }
			set { internalValue = value; }
		}

		public override void Write(System.IO.StreamWriter stream) {
			stream.Write(internalValue);
		}
	}
}
