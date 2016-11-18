using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace master.framework.mail
{
    public abstract class BaseManager
    {
        public void Send(MailAddress from, List<string> to, dto.EmailMessage email, dto.EmailConfiguration conf = null)
        {
            if (conf == null)
            {
                conf = dto.EmailConfiguration.InstanceDefault();
                if (conf == null)
                {
                    this.logError(ex: new ArgumentException("Email not configured"));
                    return;
                }
            }

            try
            {
                var fromAddress = new MailAddress(from.Address, from.DisplayName);
                var smtp = new SmtpClient(conf.SMTP, conf.Port);
                smtp.EnableSsl = conf.EnableSSL;
                smtp.UseDefaultCredentials = string.IsNullOrWhiteSpace(conf.Login);
                if (!string.IsNullOrWhiteSpace(conf.Login))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(conf.Login, conf.Password);
                }

                foreach (var item in to.Select((value, index) => new dto.ForEach<string>() { Index = index, Value = value }).ToList())
                {
                    var send = new MailMessage(from, new MailAddress(item.Value));
                    send.Subject = email.Subject;
                    send.Body = email.Body;
                    send.IsBodyHtml = email.IsBodyHtml;
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(send.Body, null, "text/html");
                    send.AlternateViews.Add(htmlView);
                    smtp.Send(send);
                    send.Dispose();
                }
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                this.logError("Error sending e-mail.", ex);
            }
        }
    }
}
