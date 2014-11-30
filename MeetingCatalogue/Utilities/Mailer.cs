using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using MeetingCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace MeetingCatalogue.Utilities
{
    public enum ActionType
    {
        Created, Updated, Deleted
    }

    public class Mailer
    {
        public static void SendEmail(Meeting meeting, ActionType action)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("meetingtracker@meetingtracker.com", "Meeting Tracker");
            foreach (var user in meeting.Participants)
            {
                msg.To.Add(new MailAddress(user.Email, user.UserName));
            }
            msg.Subject = meeting.Title;

            msg.Body = GetBody(meeting, action);

            var ical = new System.Net.Mail.Attachment(new MemoryStream( Encoding.UTF8.GetBytes( GetCalendarEntry(meeting, action) ) ), new System.Net.Mime.ContentType("text/calendar"));
            msg.Attachments.Add(ical);

            //System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            //contype.Parameters.Add("method", "REQUEST");
            //contype.Parameters.Add("name", "Meeting.ics");
            //msg.Attachments.Add(new System.Net.Mail.MailMailAttachment())
            //AlternateView avCal = AlternateView.CreateAlternateViewFromString(calendarEvent, contype);
            //msg.AlternateViews.Add(avCal);

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );

            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            smtpClient.Credentials = credentials;

            smtpClient.Send(msg);
        }

        private static string GetBody(Meeting meeting, ActionType action)
        {
            return String.Format("{0} event was {1}.", meeting.Title, action.ToString().ToLower());
        }

        private static string GetCalendarEntry(Meeting meeting, ActionType action)
        {
            var iCal = new iCalendar();

            var evt = iCal.Create<DDay.iCal.Event>();
            evt.Summary = meeting.Summary;
            evt.Organizer = new Organizer(meeting.Owner.Email);

            evt.Created = new iCalDateTime(meeting.CreatedOn);
            evt.Start = new iCalDateTime(meeting.From.Value);
            evt.End = new iCalDateTime(meeting.To.Value);
            evt.Location = meeting.Location;

            evt.LastModified = new iCalDateTime(DateTime.Now);
            // TODO: Generate UID
            evt.Status = action == ActionType.Deleted ? EventStatus.Cancelled : EventStatus.Confirmed;

            string calendarEvent = new iCalendarSerializer().SerializeToString(iCal);
            return calendarEvent;
        }
    }
}