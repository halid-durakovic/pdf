using System;
using System.Text;

namespace BarryJones.PDF.Types {

	public sealed class PdfReal : PdfNumeric {

		private float internalValue;

		public PdfReal() {
		}

		public PdfReal(float initialValue) {
			this.internalValue = initialValue;
		}

		public float Value {
			get { return internalValue; }
			set { internalValue = value; }
		}

		public override void Write(System.IO.StreamWriter stream) {
			stream.Write(internalValue);
		}
	}
}
