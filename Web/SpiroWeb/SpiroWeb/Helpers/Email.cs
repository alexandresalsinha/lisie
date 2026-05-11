using System;
using System.Net;
using System.Net.Mail;

namespace SpiroWeb.Helpers
{
    public static class Email
    {
        static public string Filename = "logs.txt";
        static public string FolderPath = string.Empty;
        static public bool Send(string emailFrom, string emailTo, string subject, string htmlBody, string textBody = "")
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(emailFrom);
                message.To.Add(new MailAddress(emailTo));
                message.Subject = subject;
                //message.IsBodyHtml = true; //to make message body as html  
                if (textBody != string.Empty)
                    message.Body = textBody;
                else
                {
                    message.IsBodyHtml = true;
                    message.Body = htmlBody;
                }
                smtp.Port = 587;
                smtp.Host = "mail.mycloud.pt"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("lisie@lisie.app", "e@.tPb<2yJbTx>Et");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); return false; }
            return true;
        }
    }
}