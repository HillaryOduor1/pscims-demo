using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace PSCIMS.Shared
{
    public static class EmailService
    {
        private static string Host
        {
            get { return ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com"; }
        }

        private static int Port
        {
            get { return int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587"); }
        }

        private static string User
        {
            get { return ConfigurationManager.AppSettings["SmtpUser"]; }
        }

        private static string Pass
        {
            get { return ConfigurationManager.AppSettings["SmtpPass"]; }
        }

        private static string From
        {
            get { return ConfigurationManager.AppSettings["SmtpFrom"] ?? User; }
        }

        public static void Send(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Pass))
                throw new InvalidOperationException("SMTP is not configured. Set SmtpHost / SmtpUser / SmtpPass in Web.config.");

            using (var client = new SmtpClient(Host, Port))
            {
                // Gmail requires TLS on port 587 (STARTTLS).
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(User, Pass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                // Fail faster (10 seconds) so the UI doesn't hang for 20+ seconds
                client.Timeout = 10000; 

                using (var msg = new MailMessage(From, to, subject, body))
                {
                    msg.IsBodyHtml = false;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Headers.Add("X-Mailer", "PSCIMS");
                    
                    try
                    {
                        client.Send(msg);
                    }
                    catch (SmtpException ex)
                    {
                        throw new Exception("SMTP Error: Could not connect to Gmail. Check your internet connection or App Password. (Inner: " + ex.Message + ")", ex);
                    }
                }
            }
        }

        public static void SendOtp(string toEmail, string firstName, string code)
        {
            string subject = "PSCIMS - Your email verification code";
            string body =
                "Hello " + (firstName ?? "") + ",\r\n\r\n" +
                "Your PSCIMS verification code is:\r\n\r\n" +
                "    " + code + "\r\n\r\n" +
                "This code expires in 10 minutes.\r\n\r\n" +
                "If you did not request this code, you can safely ignore this email.\r\n\r\n" +
                "Regards,\r\n" +
                "Public Service Commission\r\n" +
                "Information Management System\r\n";

            Send(toEmail, subject, body);
        }
    }
}

/*using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace PSCIMS.Shared
{
    public static class EmailService
    {
        private static string Host
        {
            get { return ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com"; }
        }

        private static int Port
        {
            get { return int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587"); }
        }

        private static string User
        {
            get { return ConfigurationManager.AppSettings["SmtpUser"]; }
        }

        private static string Pass
        {
            get { return ConfigurationManager.AppSettings["SmtpPass"]; }
        }

        private static string From
        {
            get { return ConfigurationManager.AppSettings["SmtpFrom"] ?? User; }
        }

        private static bool Secure
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["SmtpSecure"] ?? "false"); }
        }

        public static void Send(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Pass))
                throw new InvalidOperationException("SMTP is not configured. Set SmtpHost / SmtpUser / SmtpPass in Web.config.");

            using (var client = new SmtpClient(Host, Port))
            {
                // Gmail requires TLS on port 587 (STARTTLS). Always enable it.
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(User, Pass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 20000;

                using (var msg = new MailMessage(From, to, subject, body))
                {
                    msg.IsBodyHtml = false;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Headers.Add("X-Mailer", "PSCIMS");
                    client.Send(msg);
                }
            }
        }

        public static void SendOtp(string toEmail, string firstName, string code)
        {
            string subject = "PSCIMS - Your email verification code";
            string body =
                "Hello " + (firstName ?? "") + ",\r\n\r\n" +
                "Your PSCIMS verification code is:\r\n\r\n" +
                "    " + code + "\r\n\r\n" +
                "This code expires in 10 minutes.\r\n\r\n" +
                "If you did not request this code, you can safely ignore this email.\r\n\r\n" +
                "Regards,\r\n" +
                "Public Service Commission\r\n" +
                "Information Management System\r\n";

            Send(toEmail, subject, body);
        }
    }
}*/
