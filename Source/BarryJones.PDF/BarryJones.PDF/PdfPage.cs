using System;
using System.Text;
using BarryJones.PDF.Structures;
using BarryJones.PDF.Types;

namespace BarryJones.PDF {

	public sealed class PdfPage : PdfDictionary {
		private PdfName type = new PdfName("Type");
		private PdfName parent = new PdfName("Parent");
		private PdfName resources = new PdfName("Resources");
		private PdfName mediaBox = new PdfName("MediaBox");
		private PdfName contents = new PdfName("Contents");
		private PdfArray contentsObj = new PdfArray();
		private PdfResourceDictionary resDic;
		private PdfDocument doc;								// Reference to containing document
		private float width;									// The width and height of the document
		private float height;

		// Constants
		private const float USER_SPACE_RESOLUTION = 72;			// 72 dpi

		internal PdfPage(Pages parentObj, PdfDocument doc, float pageHeight, float pageWidth) {
			// Set the width and height properties
			this.width = pageWidth * USER_SPACE_RESOLUTION;
			this.height = pageHeight * USER_SPACE_RESOLUTION;

			this.Add(type, new PdfName("PdfPage"));
			this.Add(parent, parentObj.IndirectReference);
			this.Add(resources, new PdfDictionary());			// TODO: This is not right
			this.Add(mediaBox, new PdfRectangleF(new PdfReal(0), new PdfReal(0), new PdfReal(width), new PdfReal(height)));
			this.doc = doc;
		}

		// register a content stream for use in the page
		internal void RegisterContentStream(PdfIndirectReference reference) {
			if(!this.Contains(contents)) {
				this.Add(contents, contentsObj);
			}
			contentsObj.Add(reference);
		}

		internal void AddProcSet(ProcSetTypes type) {
			resDic.AddProcSet(type);
		}

		public PdfGraphics GetGraphics() {
			// Create a new resource dictionary for the page
			resDic = new PdfResourceDictionary();

			// doc.RegisterForOutput(resDic);
			// this[resources] = resDic;

			this.Remove(resources);
			this.Add(resources, resDic);

			return new PdfGraphics(this, doc);
		}

		public float Height{
			get { return this.height; }
		}

		public float Width { 
			get { return this.width; }
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
