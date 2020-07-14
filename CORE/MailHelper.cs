using System.Net.Mail;

namespace CORE
{
    /// <summary>
    /// Helps send smtp mail
    /// </summary>
    public sealed class MailHelper
    {
        readonly string defaultSender = "";
        SmtpClient client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpDomain">Smtp address</param>
        /// <param name="smtpPort">Smtp port</param>
        /// <param name="smtpId">Smtp id</param>
        /// <param name="smtpPassword">Smtp password</param>
        public MailHelper(string smtpDomain, int smtpPort, string smtpId, string smtpPassword)
        {
            client = new SmtpClient(smtpDomain, smtpPort);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;            
            client.Credentials = new System.Net.NetworkCredential(smtpId, smtpPassword, smtpDomain);
            defaultSender = smtpId;
        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="subject">Mali title</param>
        /// <param name="body">Mail body</param>
        /// <param name="to">Mail to list</param>
        /// <param name="from">Mail from</param>
        public void Send(string subject, string body, string[] to, string from = "")
        {
            if (from == "") from = defaultSender;

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(from);

            foreach (string str in to)
            {
                message.To.Add(new MailAddress(str));
            }

            message.Subject = subject;
            message.Body = body;
            client.Send(message);
        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="subject">Mali title</param>
        /// <param name="body">Mail body</param>
        /// <param name="to">Mail to</param>
        /// <param name="from">Mail from</param>
        public void Send(string subject, string body, string to, string from = "")
        {
            string[] toArr = new string[1];
            toArr[0] = to;
            Send(subject, body, toArr, from);
        }
    }
}
