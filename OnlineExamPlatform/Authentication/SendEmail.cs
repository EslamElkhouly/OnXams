using System.Net;
using System.Net.Mail;

namespace OnlineExamPlatform.Authentication
{
    public class SendEmail
    {
        public void Send(string sendTo, string messageSubject, string messageBody)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("eslam2036@gmail.com", "eslamelkhouly300");


            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("eslam2036@gmail.com", "OnXams");
            mailMessage.To.Add(sendTo);
            mailMessage.Subject =messageSubject;
            mailMessage.Body = messageBody;
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }
}