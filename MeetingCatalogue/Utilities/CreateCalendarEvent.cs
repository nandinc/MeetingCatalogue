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

namespace MeetingCatalogue.Utilities
{
    public class CreateCalendarEvent
    {
        // TODO meeting as parameter, replace placeholder data
        public static void createEvent()
        {
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

            var iCal = new iCalendar();
            var evt = iCal.Create<DDay.iCal.Event>();
            evt.Summary = "Summary";
            evt.Start = new iCalDateTime(DateTime.Now);
            evt.End = new iCalDateTime(DateTime.Now.AddMinutes(45));
            evt.Description = "Description";
            evt.Location = "Location";  
            //evt.Attendees = new List<Attendee>

            string calendarEvent = new iCalendarSerializer().SerializeToString(iCal);

        }
    }
}