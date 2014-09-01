using System;
using System.Text;

namespace BarryJones.PDF.Types {
	//
	// any object can be specified as an indirect object, this provides
	// the object with a reference number with which other elements can
	// refer to it. [pdf reference 1.5 - 3.2.9]
	//
	// a reference to an indirect object is made as:
	//	<object number> <gen number> R
	//
	// the actual object for reference is defined as:
	//	<object number> <gen number> obj
	//		...data...
	//	endobj
	//
	public sealed class PdfIndirectReference : PdfObject {
		private int id = -1;
		private int generation = 0;

		public PdfIndirectReference() {
		}

		// positive object number
		public int Id {
			get { return id; }
			set { 
				if(value >= 0) {
					id = value;
				}
				else {
					throw new ArgumentException("value must be greater than zero");
				}
			}
		}

		// non-negative; in all new files the generation number will always be zero
		public int Generation {
			get { return generation; }
			set { 
				if(value >= 0) {
					generation = value;
				}
				else {
					throw new ArgumentException("value must be greater than zero");
				}
			}
		}

		public string GetReference() {
			StringBuilder sb = new StringBuilder();
			sb.Append(this.id);
			sb.Append(" ");
			sb.Append(this.generation);
			sb.Append(" R");
			return sb.ToString();
		}

		public override void Write(System.IO.StreamWriter stream) {
			stream.Write(GetReference());
		}
	}
}
