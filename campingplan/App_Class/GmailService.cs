using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace campingplan.App_Class
{
    public class GmailService
    {
        public string MessageText { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string AppPassword { get; set; }
        public string ReceiveEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public GmailService()
        {
            MessageText = "";
            SenderName = "網站管理員";
            SenderEmail = "candy41518@gmail.com";
            AppPassword = "kuxvvwzhsezrbvey";
            ReceiveEmail = "";
            Subject = "";
            Body = "";

        }

        public void Send()
        {
            var fromemail = new MailAddress(SenderEmail, SenderName);
            var toemail = new MailAddress(ReceiveEmail);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromemail.Address,AppPassword)
            };

            using (var message = new MailMessage(fromemail, toemail)
            {
                Subject = Subject,
                Body = Body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

    }
}