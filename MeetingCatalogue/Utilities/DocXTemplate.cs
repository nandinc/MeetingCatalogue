using Novacode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MeetingCatalogue.Utilities
{
    public class DocXTemplate
    {
        public static void CreateWordDocument()
        {
           // TODO location chooser? or something more general
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            string fileName = path+System.IO.Path.DirectorySeparatorChar+"MeetingDetails.docx";

            // Create the document in memory:
            var document = DocX.Create(fileName);

            // Add a Table to this document.
            Table t = document.AddTable(8, 2);

            // Specify some properties for this Table.
            t.Alignment = Alignment.center;

            //t.Design = TableDesign.MediumGrid1Accent2;

            // Add paramater names
            t.Rows[0].Cells[0].Paragraphs.First().Append("Created");
            t.Rows[1].Cells[0].Paragraphs.First().Append("Owner");
            t.Rows[2].Cells[0].Paragraphs.First().Append("Participants");
            t.Rows[3].Cells[0].Paragraphs.First().Append("From");
            t.Rows[4].Cells[0].Paragraphs.First().Append("To");
            t.Rows[5].Cells[0].Paragraphs.First().Append("Location");
            t.Rows[6].Cells[0].Paragraphs.First().Append("Agenda");
            t.Rows[7].Cells[0].Paragraphs.First().Append("Summary");

            // set meeting details
            // TODO switch, add meeting as a parameter
            t.Rows[0].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[1].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[2].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[3].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[4].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[5].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[6].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[7].Cells[1].Paragraphs.First().Append("Created");

            //t.Rows[0].Cells[1].Paragraphs.First().Append(meeting.Created.ToString());
            //t.Rows[1].Cells[1].Paragraphs.First().Append(meeting.Owner.ToString());
            //t.Rows[2].Cells[1].Paragraphs.First().Append(meeting.Participants.ToList().ToString());
            //t.Rows[3].Cells[1].Paragraphs.First().Append(meeting.From.ToString());
            //t.Rows[4].Cells[1].Paragraphs.First().Append(meeting.To.ToString());
            //t.Rows[5].Cells[1].Paragraphs.First().Append(meeting.Location);
            //t.Rows[6].Cells[1].Paragraphs.First().Append(meeting.Agenda);
            //t.Rows[7].Cells[1].Paragraphs.First().Append(meeting.Summary);

            // header
            string headerText = "Meeting details";

            // Title Formatting:
            var titleFormat = new Formatting();
            titleFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");
            titleFormat.Size = 18D;
            titleFormat.Position = 12;

            Paragraph title = document.InsertParagraph(headerText, false, titleFormat);
            title.Alignment = Alignment.center;

            // Insert the Table into the document.
            document.InsertTable(t);

            document.Save(); // Release this document from memory.

            // Open in Word:
            Process.Start("WINWORD.EXE", fileName);

        }

    }
}