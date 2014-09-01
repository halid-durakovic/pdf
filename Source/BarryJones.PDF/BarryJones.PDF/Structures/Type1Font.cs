using System;
using System.Resources;

namespace BarryJones.PDF.Structures {

	public class Type1Font {

		public Type1Font(InstalledFonts name) {
			switch(name) {
				case InstalledFonts.Helvetica:
					break;
				case InstalledFonts.Courier:
					break;
				case InstalledFonts.TimesNewRoman:
					break;
			}
		}
	}
}
