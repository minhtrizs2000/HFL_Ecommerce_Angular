using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;
using MimeKit;
using QLBanDoGiaDung_API.Settings;
using System;
using Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QLBanDoGiaDung_API.Interfaces;
using System.Threading;

namespace QLBanDoGiaDung_API.Services
{
  public class MailService : IMailService
  {
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> options)
    {
      _mailSettings = options.Value;
    }

    public async Task SendEmailAsync(MailRequest mailRequest)
    {
      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
      email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
      CancellationToken cancellationToken = default;
      SecureSocketOptions options = SecureSocketOptions.Auto;
      email.Subject = mailRequest.Subject;
      var builder = new BodyBuilder();
      //if (mailRequest.Attachments != null)
      //{
      //  byte[] fileBytes;
      //  foreach (var file in mailRequest.Attachments)
      //  {
      //    if (file.Length > 0)
      //    {
      //      using (var ms = new MemoryStream())
      //      {
      //        file.CopyTo(ms);
      //        fileBytes = ms.ToArray();
      //      }
      //      builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
      //    }
      //  }
      //}
      builder.HtmlBody = mailRequest.Body;
      email.Body = builder.ToMessageBody();
      using var smtp = new SmtpClient();
      smtp.Connect(_mailSettings.Host, _mailSettings.Port, options, cancellationToken);
      smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password, cancellationToken);
      await smtp.SendAsync(email);
      smtp.Disconnect(true);
    }
  }
}