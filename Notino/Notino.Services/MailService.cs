using System.IO;
using System.Net.Mail;

namespace Notino.Common.Service
{
    public class MailService : IMailService
    {
        public void SendMail(string emailAddress, byte[] fileAttachment, string fileName)
        {
            MailMessage mail = new();
            SmtpClient SmtpServer = new("smtp.notino.com");
            mail.From = new MailAddress("support@notino.com");
            mail.To.Add(emailAddress);
            mail.Subject = "Converted file";
            mail.Body = "Converted file in attachments.";

            Attachment attachment;
            attachment = new Attachment(new MemoryStream(fileAttachment), fileName);
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("support@notino.com", "PA55W0RD");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
