using System;
using System.IO;
using System.Net.Mail;

namespace Helper
{
    /// <summary>
    /// Helps send smtp mail
    /// </summary>
    public sealed class MailHelper
    {
        private readonly SmtpClient Client;        
        private readonly string DefaultSender = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpDomain">Smtp domain</param>
        /// <param name="smtpPort">Smtp port</param>
        public MailHelper(string smtpDomain, int smtpPort)
        {
            Client = new SmtpClient(smtpDomain, smtpPort);
            Client.EnableSsl = true;
            Client.UseDefaultCredentials = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpDomain">Smtp address</param>
        /// <param name="smtpPort">Smtp port</param>
        /// <param name="smtpId">Smtp id</param>
        /// <param name="smtpPassword">Smtp password</param>
        public MailHelper(string smtpDomain, int smtpPort, string smtpId, string smtpPassword) : this(smtpDomain, smtpPort)
        {
            Client.Credentials = new System.Net.NetworkCredential(smtpId, smtpPassword, smtpDomain);
            DefaultSender = smtpId;
        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="to">Compatible string, string[]</param>
        /// <param name="from"></param>
        /// <param name="attachment">Compatible filepath(string), byte[](need title)</param>
        /// <param name="attachmentTitle"></param>
        public void Send(string subject, string body, object to, string from = null
            , object attachment = null, object attachmentTitle = null
            ,SendCompletedEventHandler sendCompletedEventHandler = null)
        {
            using (MailMessage message = new MailMessage())
            {
                message.IsBodyHtml = true;
                message.From = new MailAddress(from ?? DefaultSender);

                if (to.GetType() == typeof(string[]))
                {
                    foreach (string str in (string[])to)
                    {
                        message.To.Add(new MailAddress(str));
                    }
                }
                else if (to.GetType() == typeof(string))
                {
                    message.To.Add(new MailAddress((string)to));
                }
                else
                {
                    throw new Exception("Wrong mailto type.");
                }

                if (attachment != null)
                {
                    if (attachment.GetType() == typeof(string[]))
                    {
                        foreach (string str in (string[])attachment)
                        {
                            message.Attachments.Add(new Attachment(str));
                        }
                    }
                    else if (attachment.GetType() == typeof(string))
                    {
                        message.Attachments.Add(new Attachment((string)attachment));
                    }
                    else if (attachment.GetType() == typeof(byte[][]) && attachmentTitle.GetType() == typeof(string[]))
                    {
                        int len;
                        if ((len = ((byte[][])attachment).Length) == ((string[])attachmentTitle).Length)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                using (MemoryStream ms = new MemoryStream(((byte[])attachment)[i]))
                                {
                                    message.Attachments.Add(new Attachment(ms
                                        , ((string[])attachmentTitle)[i] ?? "Attachment"));
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Attachment Title and binary doesn't match.");
                        }
                    }
                    else if (attachment.GetType() == typeof(byte[]) && attachmentTitle.GetType() == typeof(string))
                    {
                        using (MemoryStream ms = new MemoryStream((byte[])attachment))
                        {
                            message.Attachments.Add(new Attachment(ms
                                , (string)attachmentTitle ?? "Attachment"));
                        }
                    }
                    else
                    {
                        throw new Exception("Wrong attachment type.");
                    }
                }

                message.Subject = subject;
                message.Body = body;

                if (sendCompletedEventHandler != null)
                {
                    Client.SendAsync(message, sendCompletedEventHandler);
                }
                else
                {
                    Client.Send(message);
                }
            }
        }
    }
}
