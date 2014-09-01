using System;
using System.Text;

namespace BarryJones.PDF {

	internal class Header {
		private int majorVersion;
		private int minorVersion;

		public Header() {
		}

		public Header(int majorVersion, int minorVersion) {
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
		}

		public int MajorVersion {
			get { return majorVersion; }
			set { majorVersion = value; }
		}

		public int MinorVersion {
			get { return minorVersion; }
			set { minorVersion = value; }
		}

		public string Write() {
			StringBuilder sb = new StringBuilder();
			sb.Append("%PDF-");
			sb.Append(this.MajorVersion);
			sb.Append(".");
			sb.Append(this.minorVersion);
			sb.Append("\n%");

			sb.Append((char)226);		// Append binary characters to tell to compression apps
			sb.Append((char)227);		// to treat as a binary file
			sb.Append((char)207);
			sb.Append((char)211);
			sb.Append("\n");

			return sb.ToString();
		}
	}
}
