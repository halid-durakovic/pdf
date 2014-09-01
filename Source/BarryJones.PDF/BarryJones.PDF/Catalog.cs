using System;
using System.Text;
using BarryJones.PDF.Types;

namespace BarryJones.PDF {

	//
	internal class Catalog : PdfDictionary {
		PdfName	type = new PdfName("Type");
		PdfName	pages = new PdfName("Pages");
		Pages pagesObj = new Pages();

		public Catalog() {
			this.Add(type, new PdfName("Catalog"));
			PdfDocument.RegisterForOutput(pagesObj);				// Register the pages object for output
			this.Add(pages, pagesObj.IndirectReference);
		}

		/// <summary>
		/// Gets or sets the pages reference for the catalog
		/// </summary>
		public Pages Pages {
			get { return pagesObj; }
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
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