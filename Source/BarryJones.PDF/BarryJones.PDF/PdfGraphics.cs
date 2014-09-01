using System;
using System.Drawing;
using BarryJones.PDF.Types;
using BarryJones.PDF.Structures;
using BarryJones.PDF.GraphicalElements;

namespace BarryJones.PDF {
	/// <summary>
	/// Summary description for Graphics.
	/// </summary>
	public sealed class PdfGraphics {
		// Fields
		private PdfPage page;							// The page object this graphics object writes to
		private PdfStream stream;

		/// <summary>
		/// Constructor
		/// </summary>
		internal PdfGraphics(PdfPage page, PdfDocument doc) {
			this.page = page;
			stream = new PdfStream();
			PdfDocument.RegisterForOutput(stream);
			page.RegisterContentStream(stream.IndirectReference);

			// Add the transform matrix for the graphics state
		}

		private void ConvertToUserSpace(ref Point point) {
			point.Y = (int)page.Height - point.Y;
		}

		/// <summary>
		/// Draws a single line, starting at parameter start and ending at parameter end
		/// </summary>
		/// <param name="start">The starting coordinates for the line</param>
		/// <param name="end">The finishing coordinates for the line</param>
		public void DrawLine(Point start, Point end) {
			In_DrawLine(start, end, 1, Color.Black);
		}

		/// <summary>
		/// Draws a single line, starting at parameter start and ending at parameter end. With the specified width.
		/// </summary>
		/// <param name="start">The starting coordinates for the line</param>
		/// <param name="end">The finishing coordinates for the line</param>
		/// <param name="width">The width of the line being drawn</param>
		public void DrawLine(Point start, Point end, int width) {
			In_DrawLine(start, end, width, Color.Black);
		}

		/// <summary>
		/// Draws a single line, starting at parameter start and ending at parameter end. With the specified width.
		/// </summary>
		/// <param name="start">The starting coordinates for the line</param>
		/// <param name="end">The finishing coordinates for the line</param>
		/// <param name="width">The width of the line being drawn</param>
		/// <param name="color">The color of the line being drawn</param>
		public void DrawLine(Point start, Point end, int width, Color color) {
			In_DrawLine(start, end, width, color);
		}

		/// <summary>
		/// Draws a single line, starting at parameter start and ending at parameter end. With the specified width.
		/// </summary>
		/// <param name="start">The starting coordinates for the line</param>
		/// <param name="end">The finishing coordinates for the line</param>
		/// <param name="width">The width of the line being drawn</param>
		/// <param name="color">The color of the line being drawn</param>
		private void In_DrawLine(Point start, Point end, int width, Color color) {
			float r = color.R / 255;														// Convert value to 0.0 - 1.0 range
			float g = color.G / 255;
			float b = color.B /255;

			// Register the new type
			page.AddProcSet(ProcSetTypes.Text);

			ConvertToUserSpace(ref start);
			ConvertToUserSpace(ref end);

			stream.WriteToSream("q");														// Push stack copy
			stream.WriteToSream(r.ToString("0.0") + " " + g.ToString("0.0") + " " + b.ToString("0.0") + " RG");
			stream.WriteToSream(width.ToString() + " w");									// Set line width
			stream.WriteToSream(start.X.ToString() + " " + start.Y.ToString() + " m");	// Set line start
			stream.WriteToSream(end.X.ToString() + " " + end.Y.ToString() + " l");		// Set line end
			stream.WriteToSream("S");														// Draw the line
			stream.WriteToSream("Q");														// Pop stack copy
		}

		/// <summary>
		/// Draws the specified text at the location start
		/// </summary>
		/// <param name="text">The text to display on screen</param>
		/// <param name="start">The point in the document to start text output</param>
		public void DrawText(string text, Point start) {
			In_DrawText(text, start, 14, 0);
		}

		/// <summary>
		/// Draws the specified text at the location start
		/// </summary>
		/// <param name="text">The text to display on screen</param>
		/// <param name="start">The point in the document to start text output</param>
		/// <param name="size">The size in points of the text</param>
		public void DrawText(string text, Point start, int size) {
			In_DrawText(text, start, size, 0);
		}

		/// <summary>
		/// Draws the specified text at the location start
		/// </summary>
		/// <param name="text">The text to display on screen</param>
		/// <param name="start">The point in the document to start text output</param>
		/// <param name="size">The size in points of the text</param>
		private void In_DrawText(string text, Point start, int size, int n) {
			// Register the new type
			page.AddProcSet(ProcSetTypes.Text);

			ConvertToUserSpace(ref start);

			int y = start.Y;//  - size;						// Convery start point to top of font

			stream.WriteToSream("BT");
			stream.WriteToSream("/F1 " + size.ToString() + " Tf");
			stream.WriteToSream(start.X.ToString() + " " + y.ToString() + " Td");
			stream.WriteToSream("(" + text + ") Tj");
			stream.WriteToSream("ET");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="rect"></param>
		public void DrawRectangle(Rectangle rect) {
			In_DrawRectangle(rect, Color.Black, Color.Empty, 1);
		}

		public void DrawRectangle(Rectangle rect, Color lineColor, Color fillColor, int lineWidth) {
			In_DrawRectangle(rect, lineColor, fillColor, lineWidth);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="rect"></param>
		private void In_DrawRectangle(Rectangle rect, Color lineColor, Color fillColor, int lineWidth) {
			// Register the new type
			page.AddProcSet(ProcSetTypes.Text);

			// Convert points to user space
			Point p = new Point(rect.X, rect.Y);
			ConvertToUserSpace(ref p);
			rect.X = p.X;
			rect.Y = p.Y;

			// Draw the rectangle
			stream.WriteToSream("q");													// Save state
			stream.WriteToSream(lineWidth.ToString() + " w");							// Set the line width
			stream.WriteToSream(														// Set the line color
				((float)lineColor.R / 255f).ToString("0.0") + " " +
				((float)lineColor.G / 255f).ToString("0.0") + " " +
				((float)lineColor.B / 255f).ToString("0.0") + " " +
				" RG"
				);
			if(!fillColor.IsEmpty) {
				stream.WriteToSream(													// Set the fill color
					((float)fillColor.R / 255f).ToString("0.0") + " " +
					((float)fillColor.G / 255f).ToString("0.0") + " " +
					((float)fillColor.B / 255f).ToString("0.0") + " " +
					" rg"
					);
			}
			stream.WriteToSream(														// Draw the rectangle
				rect.X.ToString() + " " +
				rect.Y.ToString() + " " +
				rect.Width.ToString() + " " +
				rect.Height.ToString() + " re"
				);
			if(fillColor.IsEmpty)
				stream.WriteToSream("S");												// Stroke the rectangle
			else
				stream.WriteToSream("B");												// Fill and stroke the rectangle
			stream.WriteToSream("Q");													// Restore state
		}

		public void DrawImage(Image image, Point start) {
			this.ConvertToUserSpace(ref start);

			// Create the image stream and register it for output
			PdfInlineImage img = new PdfInlineImage(image);
			PdfDocument.RegisterForOutput(img);
			page.RegisterContentStream(img.IndirectReference);

			// Create the reference stream and image diplay stuff
//			PdfStream imageStream = new PdfStream();
//			PdfDocument.RegisterForOutput(imageStream);
//			page.RegisterContentStream(imageStream.IndirectReference);
//
//			imageStream.WriteToSream("q");
//			imageStream.WriteToSream("0 0 0 0 " +
//				start.X.ToString() + " " +
//				start.Y.ToString() + " cm"
//				);
//			imageStream.WriteToSream("/I1 Do");
//			imageStream.WriteToSream("Q");
		}


		/// <summary>
		/// Draw graphical element to the document
		/// </summary>
		/// <param name="e"></param>
		public void DrawElement(Element e, Point start) {
		}

		public void DrawArc() {
		}

		public void DrawCircle() {
		}

		public void DrawElipse() {
		}

		public void DrawTriangle() {
		}
	}
}
