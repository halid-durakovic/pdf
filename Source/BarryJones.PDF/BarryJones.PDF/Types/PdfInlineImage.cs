using System;

namespace BarryJones.PDF.Types {

	internal class PdfInlineImage : PdfStream {

		public PdfInlineImage(System.Drawing.Image image) 
			: base() {
			base.streamDictionary.Add(new PdfName("Name"), new PdfName("Test"));
			base.streamDictionary.Add(new PdfName("BitsPerComponent"), new PdfInt(8));
			base.streamDictionary.Add(new PdfName("ColorSpace"), new PdfName("RGB"));
			base.streamDictionary.Add(new PdfName("Width"), new PdfInt(8));
			base.streamDictionary.Add(new PdfName("Height"), new PdfInt(8));
			base.streamDictionary.Add(new PdfName("Filter"), new PdfName("A85"));

			image.Save(base.memStream, image.RawFormat);
			s.Write("\n");
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(this.IndirectReference.Id != -1) {
				stream.Write(this.IndirectReference.Id);
				stream.Write(" ");
				stream.Write(this.IndirectReference.Generation);
				stream.Write(" obj\n");
			}
			else {
				throw new InvalidOperationException("Cannot write an unregistered stream");
			}

			s.Flush();
			byte [] buffer = memStream.ToArray();
			char [] stringBuffer = new char[buffer.Length];
			buffer.CopyTo(stringBuffer, 0);
			stream.Write(stringBuffer, 0, stringBuffer.Length); //.BaseStream.Write(buffer, 0, buffer.Length);

			stream.Write("endstream");
			if(this.IndirectReference.Id != -1) {
				stream.Write("\nendobj");
			}
		}
	}
}
