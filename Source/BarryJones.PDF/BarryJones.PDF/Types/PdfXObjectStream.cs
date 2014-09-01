using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace BarryJones.PDF.Types {

	internal sealed class PdfXObjectStream : PdfStream {

		private Image image;
		private PdfName name;
		private PdfName type = new PdfName("Type");
		private PdfName subType = new PdfName("Subtype");
		private PdfName width;
		private PdfName height;
		private PdfName colorSpace;
		private PdfName bitsPerComponent;
		private PdfName filter;

		public PdfXObjectStream(Image image, PdfName name) 
			: base() {
			this.image = image;
			this.name = name;
			this.width = new PdfName("Width");
			this.height = new PdfName("Height");
			this.filter = new PdfName("Filter");

			image.Save(base.s.BaseStream, image.RawFormat);
			s.Write("\n");

			// Add the elements for an image XObject to the stream dictionary
			this.streamDictionary.Add(type, new PdfName("XObject"));
			this.streamDictionary.Add(subType, new PdfName("Image"));
			this.streamDictionary.Add(new PdfName("Name"), name);
			this.streamDictionary.Add(width, new PdfInt(image.Width));
			this.streamDictionary.Add(height, new PdfInt(image.Height));
			this.streamDictionary.Add(new PdfName("BitsPerComponent"), new PdfInt(8)); //TODO: Remove test
			this.streamDictionary.Add(filter, new PdfName("DCTDecode"));
			this.streamDictionary.Add(new PdfName("ColorSpace"), new PdfName("DeviceCMYK"));
		}

		~PdfXObjectStream() {
			// TODO: IMplement the Disposable pattern
			s.Close();
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(this.IndirectReference.Id != -1) {
				stream.Write(this.IndirectReference.Id);
				stream.Write(" ");
				stream.Write(this.IndirectReference.Generation);
				stream.Write(" obj\n");
			}
			else
			{
				throw new InvalidOperationException("Cannot write an unregistered stream");
			}

			// Make sure we are showing the correct stream length
			((PdfInt)streamDictionary[length]).Value = (int)s.BaseStream.Length;
			stream.Write("\nstream\n");

			stream.Write("BI\n");
			streamDictionary.Write(stream);
			stream.Write("DI \n");

			s.Flush();
			byte [] buffer = memStream.ToArray();
			char [] stringBuffer = new char[buffer.Length];
			buffer.CopyTo(stringBuffer, 0);
			stream.Write(stringBuffer, 0, stringBuffer.Length); //.BaseStream.Write(buffer, 0, buffer.Length);

			stream.Write("EI\n");

			stream.Write("endstream");
			if(this.IndirectReference.Id != -1) {
				stream.Write("\nendobj");
			}
		}
	}
}
