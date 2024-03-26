using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        #region SendMail
        public async Task<bool> SendMail(string To, string Subject, string Body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", From));
                message.To.Add(new MailboxAddress("", To));
                message.Subject = Subject;
                message.Body = new TextPart("html")
                {
                    Text = Body
                };
                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(SmtpServer, Port, false);
                await client.AuthenticateAsync(UserName, Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion SendMail

        #region SendMailWithAttachments
        public async Task<bool> SendMailWithAttachments(string To, string Subject, string Body, List<string> Attachments)
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress("", From));
            message.To.Add(new MailboxAddress("", To));
            message.Subject = Subject;

            var multipart = new Multipart("mixed");

            var bodyPart = new TextPart("html")
            {
                Text = Body
            };
            multipart.Add(bodyPart);

            if (Attachments != null)
            {
                foreach (string attachmentPath in Attachments)
                {
                    if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    {
                        var attachment = new MimePart()
                        {
                            Content = new MimeContent(File.OpenRead(attachmentPath), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(attachmentPath)
                        };
                        multipart.Add(attachment);
                    }
                }
            }

            message.Body = multipart;

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(SmtpServer, Port, false);
            await client.AuthenticateAsync(UserName, Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        #endregion SendMailWithAttachments

        #region Encode
        public static string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        #endregion Encode

        #region Decode
        public static string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion Decode 
    }
}
