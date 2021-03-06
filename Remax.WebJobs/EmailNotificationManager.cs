﻿using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remax.WebJobs.Settings;

namespace Remax.WebJobs
{
    public class EmailNotificationManager : INotificationManager
    {
        private readonly IOptions<EmailSetting> _emailSettings;
        private readonly SmtpClient _smtpClient;

        public EmailNotificationManager(IOptions<EmailSetting> emailSettings)
        {
            _emailSettings = emailSettings;
            _smtpClient = _smtpClient = new SmtpClient
            {
                UseDefaultCredentials = false,
                Host = emailSettings.Value.Domain,
                Credentials = new NetworkCredential(emailSettings.Value.Username,
                    emailSettings.Value.Password),
                Port = emailSettings.Value.Port,
                DeliveryMethod = emailSettings.Value.DeliveryMethod,
                PickupDirectoryLocation = emailSettings.Value.PickupDirectoryLocation,
                EnableSsl = emailSettings.Value.DeliveryMethod == SmtpDeliveryMethod.Network
            };            
        }

        public async void SitesUpdated(SiteUpdatedDetail[] sitesSuccesUpdated,
            SiteUpdatedDetail[] sitesFailed
            )
        {
            var sitesSuccededJoin = string.Join("<br/>", sitesSuccesUpdated.Select(x => $"<a href='{x.Url}'>{x.AppService}</a>"));
            var sitesFailedJoin = string.Join("<br/>", sitesFailed.Select(x => x.AppService));
            var template = $@"Se han actualizado los sitios<br/><br/>
                            {sitesSuccededJoin}<br/><br/>
                            Los siguientes sitios han fallado:<br/><br/>
                            {sitesFailedJoin}
                            ";

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Value.FromEmail),
                To = { _emailSettings.Value.ToEmail },
                Subject = "Lambda - Remax. Notificación de actualización de sitios",
                Body = template,
                IsBodyHtml = true
            };

            await Task.Run(() =>
            {
                _smtpClient.SendAsync(mailMessage, null);
            });

        }
    }
}
