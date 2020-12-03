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
        public string Sendermemail { get; set; }
        public string AppPassword { get; set; }
        public string Receivememail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public GmailService()
        {
            MessageText = "";
            SenderName = "網站管理員";
            Sendermemail = "candy41518@gmail.com";
            AppPassword = "kuxvvwzhsezrbvey";
            Receivememail = "";
            Subject = "";
            Body = "";

        }

        public void Send()
        {
            var fromemail = new MailAddress(Sendermemail, SenderName);
            var tomemail = new MailAddress(Receivememail);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromemail.Address,AppPassword)
            };

            using (var message = new MailMessage(fromemail, tomemail)
            {
                Subject = Subject,
                Body = Body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

    }
}