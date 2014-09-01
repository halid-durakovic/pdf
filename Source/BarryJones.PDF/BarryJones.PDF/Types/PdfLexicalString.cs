using System;

namespace BarryJones.PDF.Types {

	public sealed class PdfLexicalString : PdfString {
		private string internalValue;

		public PdfLexicalString() {
		}

		public PdfLexicalString(string initialValue) {
			this.internalValue = initialValue;
		}

		public string Value {
			get { return internalValue; }
			set { internalValue = value; }
		}

		public override void Write(System.IO.StreamWriter stream) {
		}
	}
}
