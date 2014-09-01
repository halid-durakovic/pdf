using System;
using System.IO;
using System.Collections;

namespace BarryJones.PDF.HelperClasses {

	// reader for Adobe Font Metric files as specified in the specification
	internal class AFMFileReader {

		private int metricSets = 0;					// Writing directions (optional) 0,1 or 2. 0 is default/assumed if not present
		private string fontName;					// (Required) name of the font program as presented to the postscript language operator
		private string fullName;					// (Optional) the full text name of the font
		private string weight;						// (Optional) the weight of the font e.g. Roman, Bold, Light
		private Box fontBBox;						// (Required) four numbers giving the lowerleft and upperright corners of the bounding box
		private string version;						// (Optional) the version of the font, matches the FontInfo dictionary in the font program
		private string notice;						//
		private string encodingScheme;				// (Optional) string naming the default vector encoding for this font
		private int mappingScheme;					// (Not required on base font programs) Integer detailing the mapping scheme
		private int escChar;						// (Required is mapping scheme is 3) The byte value of the escape charecter
		private string charecterSet;				// (Optional) String describing the charecter set for this font program
		private int charecters;						// (Optional) the number of charecters defined in this program
		private bool isBaseFont;					// (Optional) True if base font, false otherwise. True is assumed if not present
		private int[] vVector;						// (Required with metricSets 2)
		private bool isFixedV;						// (Optional) True is vVector applies to all glyphs
		private int capHeight;						// (Optional) Ussually the y-value of the top of a capital H
		private int xHeight;						// (Optional) Ussually the y-value of the top of a lowercase x
		private int ascender;						// (Optional) Ussually the y-value of the top of a lowercase d
		private int descender;						// (Optional) Ussually the y-value of the bottom of a lowercase p
		private int startDirection = 0;				// Writing direction (optional) 0 is implied
		private Hashtable characterMetrics;			// Container for character metrics information
		private StreamReader fileStream;

		public AFMFileReader(string fileName) {
		}

		public AFMFileReader(StreamReader fileStream) {
			if(fileStream == null)
				throw new AFMFileReaderException("The passed in StreamReader was null");
//			if(fileStream.ReadLine().IndexOf("StartFontMetrics") != 0)
//				throw new AFMFileReaderException("The file does not conform to the AFM specification");

			this.fileStream = fileStream;
		}

		~AFMFileReader() {
			// Do not destroy the stream we did not create it, just make sure we destroy the references
			fileStream = null;
		}
	}


	// represents a line from the AFM file describing font metrics
	internal class AFMFileReaderCharecterMetrics {
		private int c;								// Default charecter code, -1 if not encoded
		private int wx;
		private int w0x;
		private int w1x;
		private int wy;
		private int w0y;
		private int w1y;
		private int w;
		private int w0;
		private int w1;
		private int vv;
		private string n;							// Name
		private Box B;
	}


	internal struct Box {
		public int LowerLeftX;
		public int LowerLeftY;
		public int UpperRightX;
		public int UpperRightY;
	}

	public class AFMFileReaderException : Exception {

		public AFMFileReaderException(string message) : base(message) {
		}

		public AFMFileReaderException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
