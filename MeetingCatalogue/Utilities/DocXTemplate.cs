using MeetingCatalogue.Models;
using Novacode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MeetingCatalogue.Utilities
{
    public class DocXTemplate
    {
        public static MemoryStream CreateWordDocument(Meeting meeting)
        {
            // TODO location chooser? or something more general
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //string fileName = path+System.IO.Path.DirectorySeparatorChar+"MeetingDetails.docx";

            MemoryStream memoryStream = new MemoryStream();

            // Create the document in memory:
            //var document = DocX.Create("meeting" + meeting.ID + ".docx");
            var document = DocX.Create(memoryStream);

            // Add a Table to this document.
            Table t = document.AddTable(8, 2);

            // Specify some properties for this Table.
            t.Alignment = Alignment.center;

            //t.Design = TableDesign.MediumGrid1Accent2;

            // Add paramater names
            //t.Rows[0].Cells[0].Paragraphs.First().Append("Created");
            t.Rows[1].Cells[0].Paragraphs.First().Append("Organizer");
            t.Rows[2].Cells[0].Paragraphs.First().Append("Participants");
            t.Rows[3].Cells[0].Paragraphs.First().Append("From");
            t.Rows[4].Cells[0].Paragraphs.First().Append("To");
            t.Rows[5].Cells[0].Paragraphs.First().Append("Location");
            t.Rows[6].Cells[0].Paragraphs.First().Append("Agenda");
            t.Rows[7].Cells[0].Paragraphs.First().Append("Summary");

            var participants = document.AddList();
            foreach (var user in meeting.Participants)
            {
                document.AddListItem(participants, user.UserName);
            }

            // set meeting details
            // TODO switch, add meeting, user as a parameter
            //t.Rows[0].Cells[1].Paragraphs.First().Append("Created");
            t.Rows[1].Cells[1].Paragraphs.First().Append(meeting.Owner.UserName);
            //t.Rows[2].Cells[1].Paragraphs.First().Append(String.Join(", ", meeting.Participants.Select(u => u.UserName)));
            t.Rows[2].Cells[1].InsertList(participants);
            t.Rows[3].Cells[1].Paragraphs.First().Append(meeting.From.Value.ToString("yyyy-MM-dd HH:mm"));
            t.Rows[4].Cells[1].Paragraphs.First().Append(meeting.To.Value.ToString("yyyy-MM-dd HH:mm"));
            t.Rows[5].Cells[1].Paragraphs.First().Append(meeting.Location);
            // TODO: convert HTML to DocX or at least strip it
            t.Rows[6].Cells[1].Paragraphs.First().Append(meeting.Agenda);
            t.Rows[7].Cells[1].Paragraphs.First().Append(meeting.Summary);

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

            document.Save();

            return memoryStream;
        } 

        //public static void SendDocument(Meeting meeting, ApplicationUser recipient)
        //{
        //    var document = CreateWordDocument(meeting);

        //    MailMessage msg = new MailMessage();
        //    msg.From = new MailAddress("meetingcatalogue@meetingcatalogue.com");
        //    msg.To.Add(new MailAddress("berkiendre@gmail.com"));
        //    msg.Subject = "subject";
        //    msg.Body = "body";

        //    var credentials = new NetworkCredential(
        //               ConfigurationManager.AppSettings["mailAccount"],
        //               ConfigurationManager.AppSettings["mailPassword"]
        //               );


        //    SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
        //    smtpClient.Credentials = credentials;
            
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        byte[] contentAsBytes = Encoding.UTF8.GetBytes(document.Text);
        //        memoryStream.Write(contentAsBytes, 0, contentAsBytes.Length);

        //        // Set the position to the beginning of the stream.
        //        memoryStream.Seek(0, SeekOrigin.Begin);

        //        // Create attachment
        //        ContentType contentType = new ContentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        //        contentType.MediaType = MediaTypeNames.Application.Rtf;
        //        contentType.Name = "Meeting details";
        //        Attachment attachment = new Attachment(memoryStream, contentType);

        //        // Add the attachment
        //        msg.Attachments.Add(attachment);

        //        // Send Mail via SmtpClient
        //        smtpClient.Send(msg);
        //    }

        //    //document.Save(); // Release this document from memory.

        //    //// Open in Word:
        //    //Process.Start("WINWORD.EXE", fileName);
        //}
    }
}