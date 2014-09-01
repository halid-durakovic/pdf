using System;
using System.Collections;
using System.IO;
using System.Text;
using BarryJones.PDF.Types;

namespace BarryJones.PDF {

	public sealed class PdfDocument {
		private Header header;
		private Catalog catalog;
		private Trailer trailer;
		private static int ids;							// The id counter for registered objects
		private static ArrayList registeredObject;

		public PdfDocument() {
			Init();
		}

		private void Init() {
			registeredObject = new ArrayList();			// Create container before instantiating other objects
			ids = 0;									// We start at zero
			header = new Header(1, 5);					// Currently only supported type (PDF 1.5)
			catalog = new Catalog();
			trailer = new Trailer();
			PdfDocument.RegisterForOutput(catalog);		// Register the catalog
		}

		public PdfPage CreatePage() {
			return this.CreatePage(PdfPageSizes.A4, PdfPageOrientation.Portrait);
		}

		public PdfPage CreatePage(PdfPageOrientation orientation) {
			return this.CreatePage(PdfPageSizes.A4, orientation);
		}

		public PdfPage CreatePage(PdfPageSizes pageSize) {
			switch(pageSize) {
				case PdfPageSizes.A1:
					return In_CreatePage(31.45f, 22.22f, PdfPageOrientation.Portrait);
				case PdfPageSizes.A2:
					return In_CreatePage(22.22f, 15.71f, PdfPageOrientation.Portrait);
				case PdfPageSizes.A3:
					return In_CreatePage(15.71f, 11.11f, PdfPageOrientation.Portrait);
				case PdfPageSizes.A4:
					return In_CreatePage(11.11f, 7.86f, PdfPageOrientation.Portrait);
				case PdfPageSizes.A5:
					return In_CreatePage(7.86f, 5.54f, PdfPageOrientation.Portrait);
				default:
					throw new InvalidOperationException("Parameter pageSize must be a member of the PdfPageSizes enumeration.");
			}
		}

		public PdfPage CreatePage(PdfPageSizes pageSize, PdfPageOrientation orientation) {
			switch(pageSize) {
				case PdfPageSizes.A1:
					return In_CreatePage(31.45f, 22.22f, orientation);
				case PdfPageSizes.A2:
					return In_CreatePage(22.22f, 15.71f, orientation);
				case PdfPageSizes.A3:
					return In_CreatePage(15.71f, 11.11f, orientation);
				case PdfPageSizes.A4:
					return In_CreatePage(11.11f, 7.86f, orientation);
				case PdfPageSizes.A5:
					return In_CreatePage(7.86f, 5.54f, orientation);
				default:
					throw new InvalidOperationException("Parameter pageSize must be a member of the PdfPageSizes enumeration.");
			}
		}

		public PdfPage CreatePage(float height, float width) {
			return In_CreatePage(height, width, PdfPageOrientation.Portrait);
		}

		private PdfPage In_CreatePage(float height, float width, PdfPageOrientation orientation) {
			if(orientation == PdfPageOrientation.Landscape) {
				float temp = width;
				width = height;
				height = width;
			}

			PdfPage newPage = new PdfPage(catalog.Pages, this, height, width);
			PdfDocument.RegisterForOutput(newPage);
			catalog.Pages.RegisterPage(newPage);

			return newPage;
		}

		public void RemovePage(PdfPage page) {
			PdfDocument.registeredObject.Remove(page);
			catalog.Pages.DeRegisterPage(page);
		}

		internal static void RegisterForOutput(PdfObject o) {
			// Check if the item is already registered in this document
			if(registeredObject.Contains(o)) 
				throw new InvalidOperationException("Object already registered on this document");
			if(o.IndirectReference.Id != -1) 
				throw new InvalidOperationException("Object is already registered");

			// Provide the object with a valid unique identifier and increment the id counter
			o.IndirectReference.Id = ++ids;

			// Add the object to the registered array
			registeredObject.Add(o);
		}

		public void Save(string filename) {
			Stream fileStream = File.Create(filename);
			this.WriteDocumentOutput(fileStream);
			fileStream.Close();
		}

		public string SaveAsStream() {
			StreamReader s = new StreamReader(GetDocumentOutput());
			return s.ReadToEnd();
		}

		private void WriteDocumentOutput(Stream stream) {
			StreamWriter s = new StreamWriter(stream);
			StringBuilder sb = new StringBuilder();
			StringBuilder refTable = new StringBuilder();
			int refCount = 1;
			int refStart = 0;

			// Write Header
			s.Write(header.Write());

			// Write out main content and prep the reference table
			foreach(PdfObject o in PdfDocument.registeredObject) {
				// Reference entry
				refTable.Append(sb.Length.ToString("0000000000"));
				refTable.Append(" ");
				refTable.Append(o.IndirectReference.Generation.ToString("00000"));
				refTable.Append(" n\n");
				refCount++;

				o.Write(s);
				s.Write("\n");
			}

			refStart = sb.Length;						// Get start location of the reference table

			// Write out the reference table
			s.Write("xref\n");
			s.Write(0);
			s.Write(" ");
			s.Write(refCount);
			s.Write("\n");
			s.Write("0000000000 65535 f\n");
			s.Write(refTable.ToString());

			// Write out the trailer
			s.Write("trailer\n");
			PdfDictionary trailerDic = new PdfDictionary();
			trailerDic.Add(new PdfName("Size"), new PdfInt(refCount));
			trailerDic.Add(new PdfName("Root"), catalog.IndirectReference);
			trailerDic.Write(s);
			s.Write("\nstartxref\n");
			s.Write(refStart);
			s.Write("\n%%EOF");
			s.Flush();
		}

		private MemoryStream GetDocumentOutput() {
			System.IO.MemoryStream ms = new MemoryStream();

			WriteDocumentOutput(ms);

			return ms;
		}
	}
}
