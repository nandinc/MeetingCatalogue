using System;
using System.IO;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;

namespace MeetingCatalogue.Utilities
{
    public class CreateCalendarEvent
    {
        public static void createEvent(Models.Meeting meeting, Models.ApplicationUser user)
        {
            #region google calendar api
            // This part is uploading the event to the owner's google calendar
            // using the calendar API
            // not used, check iCal format below it
            
            //UserCredential credential;

            //var requiredPath = Path.GetDirectoryName(Path.GetDirectoryName(
            //    System.IO.Path.GetDirectoryName(
            //    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)));
            //var jsonLocation = requiredPath + "\\MeetingCatalogue\\Utilities\\client_secrets.json";
            //var localPath = new Uri(jsonLocation).LocalPath;
            //using (var stream = new FileStream(localPath, FileMode.Open,
            //    FileAccess.Read))
            //{
            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        new[] { CalendarService.Scope.Calendar },
            //        "user", CancellationToken.None,
            //        new FileDataStore("Calendar.Auth.Store")).Result;
            //}

            //var service = new CalendarService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = "MeetingCatalogue",
            //});

            //Google.Apis.Calendar.v3.Data.Event newEvent = new Google.Apis.Calendar.v3.Data.Event
            //{
            //    Summary = "Appointment",
            //    Location = "Somewhere",
            //    Start = new EventDateTime()
            //    {
            //        DateTime = DateTime.Now,
            //        TimeZone = "America/Los_Angeles"
            //    },
            //    End = new EventDateTime()
            //    {
            //        DateTime = DateTime.Now.AddMinutes(30),
            //        TimeZone = "America/Los_Angeles"
            //    },
            //    Attendees = new List<EventAttendee>()
            //      {
            //          new EventAttendee() { Email = "be@g.com" }
            //      }
            //};

            //Google.Apis.Calendar.v3.Data.Event myEvent = service.Events.Insert(newEvent, "primary").Execute();
            #endregion

            // can use this for test
            //var iCal = new iCalendar();
            //var evt = iCal.Create<DDay.iCal.Event>();
            //evt.Summary = "Summary";
            //evt.Organizer = new Organizer("Organizer");
            //evt.Start = new iCalDateTime(DateTime.Now);
            //evt.End = new iCalDateTime(DateTime.Now.AddMinutes(45));
            //evt.Description = "Description";
            //evt.Location = "Location";

            var iCal = new iCalendar();

            var evt = iCal.Create<DDay.iCal.Event>();
            evt.Summary = meeting.Summary;
            evt.Organizer = new Organizer(user.Email);

            if (meeting.From.HasValue)
            {
                evt.Start = new iCalDateTime(meeting.From ?? DateTime.Now);
            }
            
            if (meeting.To.HasValue)
            {
                evt.End = new iCalDateTime(meeting.To ?? DateTime.Now);
            }
            evt.Location = meeting.Location;  

            string calendarEvent = new iCalendarSerializer().SerializeToString(iCal);

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("meetingcatalogue@meetingcatalogue.com");
            foreach (var participant in meeting.Participants)
            {
                msg.To.Add(new MailAddress(participant.Email));
            }
            msg.To.Add(new MailAddress(user.Email));
            msg.Subject = meeting.Title;
            msg.Body = "You have been marked as a participant in an upcoming meeting. Check the calendar entry for details.";

            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["mailAccount"],
                       ConfigurationManager.AppSettings["mailPassword"]
                       );


            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            smtpClient.Credentials = credentials;

            System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            contype.Parameters.Add("method", "REQUEST");
            contype.Parameters.Add("name", "Meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(calendarEvent, contype);
            msg.AlternateViews.Add(avCal);
            smtpClient.Send(msg);
        }
    }
}