using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MeetingCatalogue.Utilities
{
    public class SendEmail
    {
        // TODO real data
        public static void sendEmail()
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo("berkiendre@gmail.com");
            myMessage.From = new System.Net.Mail.MailAddress(
                                "berkiendre@live.co.uk", "BE");
            myMessage.Subject = "asd";
            myMessage.Text = "asd";
            //myMessage.AddAttachment()
            myMessage.Html = "asd";

            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["mailAccount"],
                       ConfigurationManager.AppSettings["mailPassword"]
                       );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            if (transportWeb != null)
            {
                transportWeb.Deliver(myMessage);
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                //Task.FromResult(0);
            }
        }
    
    }
}