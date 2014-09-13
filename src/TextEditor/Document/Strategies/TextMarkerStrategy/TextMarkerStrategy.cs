
using System;

using System.Drawing;
using System.Diagnostics;
using System.Collections;

namespace NetFocus.DataStructure.TextEditor.Document
{	
	/// <summary>
	/// Description of MarkerStrategy.	
	/// </summary>
	public class TextMarkerStrategy
	{
		ArrayList textMarker = new ArrayList();
		IDocument document;
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		public ArrayList TextMarker {
			get {
				return textMarker;
			}
		}
		
		public TextMarkerStrategy(IDocument document)
		{
			this.document = document;
			document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
		}
		
		Hashtable markersTable = new Hashtable();
		
		public ArrayList GetMarkers(int offset)
		{
			if (!markersTable.Contains(offset)) {
				ArrayList markers = new ArrayList();
				for (int i = 0; i < textMarker.Count; ++i) {
					TextMarker marker = (TextMarker)textMarker[i];
					if (marker.Offset <= offset && offset <= marker.Offset + marker.Length) {
						markers.Add(marker);
					}
				}
				markersTable[offset] = markers;
			}
			return (ArrayList)markersTable[offset];
		}
		
		public ArrayList GetMarkers(int offset, int length)
		{
			ArrayList markers = new ArrayList();
			for (int i = 0; i < textMarker.Count; ++i) {
				TextMarker marker = (TextMarker)textMarker[i];
				if (marker.Offset <= offset && offset <= marker.Offset + marker.Length ||
				    marker.Offset <= offset + length && offset + length <= marker.Offset + marker.Length ||
				    offset <= marker.Offset && marker.Offset <= offset + length ||
				    offset <= marker.Offset + marker.Length && marker.Offset + marker.Length <= offset + length
				    ) {
					markers.Add(marker);
				}
			}
			return markers;
		}
		
		public ArrayList GetMarkers(Point position)
		{
			ArrayList markers = new ArrayList();
			if (position.Y >= document.TotalNumberOfLines || position.Y < 0) {
				return markers;
			}
			LineSegment segment = document.GetLineSegment(position.Y);
			return GetMarkers(segment.Offset + position.X);
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			// reset markers table
			markersTable.Clear();
			document.UpdateSegmentListOnDocumentChange(textMarker, e);
		}
	}
}
