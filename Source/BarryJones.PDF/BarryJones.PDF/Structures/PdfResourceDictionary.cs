using System;
using System.Text;
using BarryJones.PDF.Types;

namespace BarryJones.PDF.Structures {

	public enum ProcSetTypes : byte {
		PDF,
		Text
	}

	internal class PdfResourceDictionary : PdfDictionary {

		private PdfName procSet = new PdfName("ProcSet");
		private PdfArray procSetObj = new PdfArray(new PdfObject[]{ new PdfName("PDF") });
		private PdfName font = new PdfName("Font");
		private PdfDictionary fontObj = new PdfDictionary();

		public PdfResourceDictionary() {
			this.Add(procSet, procSetObj);
			this.Add(font, fontObj);

			// Add a basic font to the page as a default
			PdfFont f = new PdfFont("F1", FontTypes.Type1, InstalledFonts.Helvetica);
			PdfDocument.RegisterForOutput(f);
			fontObj.Add(f.FontName, f.IndirectReference);
		}

		public void AddProcSet(ProcSetTypes type) {
			foreach(PdfName n in procSetObj) {
				if(n.Value == type.ToString())
					return;
			}
			procSetObj.Add(new PdfName(type.ToString()));
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
