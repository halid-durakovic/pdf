using System;

namespace BarryJones.PDF.Types {
	//
	// pdf representation of the boolean object, pdf supports boolean
	// by the keywords true and false [pdf reference 1.5 - 3.2.1]
	//
	internal sealed class PdfBoolean : PdfObject {

		private bool internalValue;

		public PdfBoolean() {
		}

		public PdfBoolean(bool boolean) {
			this.internalValue = boolean;
		}

		public bool Value {
			get { return internalValue; }
			set { internalValue = value; }
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(internalValue) {
				stream.Write("true");
			}
			else {
				stream.Write("false");
			}
		}
	}
}
