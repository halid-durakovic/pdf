using System;
using BarryJones.PDF.Types;

namespace BarryJones.PDF.Structures {

	internal class PdfRectangle : PdfArray {
		private PdfInt point1;
		private PdfInt point2;
		private PdfInt point3;
		private PdfInt point4;

		public PdfRectangle() {
			point1 = new PdfInt();
			point2 = new PdfInt();
			point3 = new PdfInt();
			point4 = new PdfInt();
			Init();
		}

		public PdfRectangle(PdfInt point1, PdfInt point2, PdfInt point3, PdfInt point4) {
			this.point1 = point1;
			this.point2 = point2;
			this.point3 = point3;
			this.point4 = point4;
			Init();
		}

		private void Init() {
			// Add the elements to the array
			this.Add(point1);
			this.Add(point2);
			this.Add(point3);
			this.Add(point4);
		}

		public PdfInt Point1 {
			get { return point1; }
			set { point1 = value; }
		}

		public PdfInt Point2 {
			get { return point2; }
			set { point2 = value; }
		}

		public PdfInt Point3 {
			get { return point3; }
			set { point3 = value; }
		}

		public PdfInt Point4 {
			get { return point4; }
			set { point4 = value; }
		}
	}
}
