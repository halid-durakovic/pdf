using System;
using System.IO;

namespace BarryJones.PDF.Types {

	// base class for all pdf types
	public abstract class PdfObject {

		private PdfIndirectReference reference;

		public PdfObject() {
		}

		public PdfIndirectReference IndirectReference {
			get {
				if(reference == null)
					reference = new PdfIndirectReference();
				return reference;
			}
		}

		public abstract void Write(StreamWriter stream);
	}
}
