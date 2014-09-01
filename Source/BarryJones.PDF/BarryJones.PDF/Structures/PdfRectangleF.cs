using System;
using BarryJones.PDF.Types;

namespace BarryJones.PDF.Structures {

	internal class PdfRectangleF : PdfArray {
		private PdfReal point1;
		private PdfReal point2;
		private PdfReal point3;
		private PdfReal point4;

		public PdfRectangleF() {
			point1 = new PdfReal();
			point2 = new PdfReal();
			point3 = new PdfReal();
			point4 = new PdfReal();
			Init();
		}

		public PdfRectangleF(PdfReal point1, PdfReal point2, PdfReal point3, PdfReal point4) {
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

		public PdfReal Point1 {	get { return point1; } set { point1 = value; } }
		public PdfReal Point2 { get { return point2; } set { point2 = value; } }
		public PdfReal Point3 { get { return point3; } set { point3 = value; } }
		public PdfReal Point4 { get { return point4; } set { point4 = value; } }
	}
}
