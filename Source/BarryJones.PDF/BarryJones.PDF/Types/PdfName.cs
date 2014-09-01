using System;
using System.Text;

namespace BarryJones.PDF.Types {

	public sealed class PdfName : PdfObject {
		private string internalName;

		public PdfName() {
		}

		public PdfName(string name) {
			this.internalName = name;
		}

		public string Value {
			get { return internalName; }
			set { internalName = value; }
		}

		public override void Write(System.IO.StreamWriter stream) {
			stream.Write("/" + internalName);
		}
	}
}
