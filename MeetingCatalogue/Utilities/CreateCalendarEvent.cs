using System;
using System.IO;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

namespace MeetingCatalogue.Utilities
{
    public class CreateCalendarEvent
    {
        // TODO meeting as parameter, replace placeholder data
        public static void createEvent()
        {
            UserCredential credential;


            var requiredPath = Path.GetDirectoryName(Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)));
            var jsonLocation = requiredPath + "\\MeetingCatalogue\\Utilities\\client_secrets.json";
            var localPath = new Uri(jsonLocation).LocalPath;
            using (var stream = new FileStream(localPath, FileMode.Open,
                FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user", CancellationToken.None,
                    new FileDataStore("Calendar.Auth.Store")).Result;
            }

            // Create the service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MeetingCatalogue",
            });

            Event newEvent = new Event
            {
                Summary = "Appointment",
                Location = "Somewhere",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "America/Los_Angeles"
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Now.AddMinutes(30),
                    TimeZone = "America/Los_Angeles"
                },
                Attendees = new List<EventAttendee>()
                  {
                      new EventAttendee() { Email = "be@g.com" }
                  }
            };

            Event myEvent = service.Events.Insert(newEvent, "primary").Execute();

        }
    }
}