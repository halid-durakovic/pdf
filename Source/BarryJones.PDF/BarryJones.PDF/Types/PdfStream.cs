using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BarryJones.PDF.Types {

	internal class PdfStream : PdfObject {

		protected PdfDictionary streamDictionary;
		protected MemoryStream memStream;
		protected StreamWriter s;
		protected PdfName length = new PdfName("Length");

		public PdfStream() {
			streamDictionary = new PdfDictionary();
			memStream = new MemoryStream();
			s = new StreamWriter(memStream);
			streamDictionary.Add(length, new PdfInt(0));
		}

		~PdfStream() {
			// TODO: Change this to use the dispose pattern as these resources may not
			// be available later
			memStream.Close();
			s.Close();
		}

		public void WriteToSream(string stream) {
			s.Write(stream);
			s.Write("\n");
			s.Flush();
		}

		public override void Write(System.IO.StreamWriter stream) {
			if(this.IndirectReference.Id != -1) {
				stream.Write(this.IndirectReference.Id);
				stream.Write(" ");
				stream.Write(this.IndirectReference.Generation);
				stream.Write(" obj\n");
			}
			else
				throw new InvalidOperationException("Cannot write an unregistered stream");

			// Make sure we are showing the correct stream length
			((PdfInt)streamDictionary[length]).Value = (int)s.BaseStream.Length;
			streamDictionary.Write(stream);
			stream.Write("\nstream\n");

			s.Flush();
			byte[] buffer = memStream.ToArray(); //new byte[s.BaseStream.Length];
			char[] stringBuffer = new char[buffer.Length];
			buffer.CopyTo(stringBuffer, 0);
			stream.Write(stringBuffer, 0, stringBuffer.Length);

			stream.Write("endstream");
			if(this.IndirectReference.Id != -1) {
				stream.Write("\nendobj");
			}
		}
	}
}
