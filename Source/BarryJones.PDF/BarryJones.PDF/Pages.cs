using System;
using System.Text;
using BarryJones.PDF.Types;

namespace BarryJones.PDF {

	internal sealed class Pages : PdfDictionary {

		private PdfName type = new PdfName("Type");
		private PdfName kids = new PdfName("Kids");
		private PdfName count = new PdfName("Count");
		private PdfInt countObj = new PdfInt();

		public Pages() {
			this.Add(type, new PdfName("Pages"));
			this.Add(kids, new PdfArray());
			this.Add(count, countObj);
		}

		public void RegisterPage(PdfPage page) {
			// Get the pages reference informaiton and write it to the kids array, then increment
			// the page count
			((PdfArray)this[kids]).Add(page.IndirectReference);
			countObj.Value++;
		}

		public void DeRegisterPage(PdfPage page) {
			// Removes the pages reference information from the array and decreases the page count.
			((PdfArray)this[kids]).Remove(page.IndirectReference);
			countObj.Value--;
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(this.IndirectReference.Id != -1) {
				stream.Write(this.IndirectReference.Id);
				stream.Write(" ");
				stream.Write(this.IndirectReference.Generation);
				stream.Write(" obj\n");
			}
			base.Write(stream);
			if(this.IndirectReference.Id != -1) {
				stream.Write("\nendobj");
			}
		}
	}
}
