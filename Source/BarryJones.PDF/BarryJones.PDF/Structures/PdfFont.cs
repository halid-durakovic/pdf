using System;
using System.Collections;
using System.Text;
using BarryJones.PDF.Types;

namespace BarryJones.PDF.Structures {

	public enum FontTypes : byte {
		Type1,
		TrueType,
		Type3
	}

	public enum InstalledFonts : byte {
		Helvetica,
		Courier,
		TimesNewRoman
	}

	public class PdfFont : PdfDictionary {
		private PdfName type = new PdfName("Type");
		private PdfName name = new PdfName("Name");
		private PdfName subType = new PdfName("Subtype");
		private PdfName baseFont = new PdfName("BaseFont");
		private PdfName nameObj;
		private PdfName subTypeObj;
		private PdfName baseFontObj;
		// private HashTable charecterMetrics;
		
		public PdfFont(string fontName, FontTypes subType, InstalledFonts baseFont){
			nameObj = new PdfName(fontName);
			subTypeObj = new PdfName(subType.ToString());
			baseFontObj = new PdfName(baseFont.ToString());
			this.Add(type, new PdfName("Font"));
			this.Add(this.name, nameObj);
			this.Add(this.subType ,subTypeObj);
			this.Add(this.baseFont ,baseFontObj);
			// this.Add(new PdfName("Encoding"), new PdfName("MacRomanEncoding"));
		}

		public PdfName FontName {
			get { return nameObj; }
		}

		public PdfName SubType {
			get { return subTypeObj; }
		}

		public PdfName BaseFont {
			get { return baseFontObj; }
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(this.IndirectReference.Id != -1) {
				stream.Write(this.IndirectReference.Id);
				stream.Write(" ");
				stream.Write(this.IndirectReference.Generation);
				stream.Write(" obj\n");
			}
			base.Write(stream);		// Write dictionary elements
			if(this.IndirectReference.Id != -1) {
				stream.Write("\nendobj");
			}
		}
	}
}
