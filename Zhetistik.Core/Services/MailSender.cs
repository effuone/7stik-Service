using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Zhetistik.Data.MailAccess;
using Microsoft.Extensions.Configuration;
using Zhetistik.Core.Interfaces;

namespace Zhetistik.Core.Services;

public class MailSender : IMailSender
{
    private readonly IConfiguration _config;

    public MailSender(IConfiguration config)
    {
        _config = config;
    }
    public async Task SendEmailAsync(MailRequest mailRequest)
    {
      var _mailSettings = _config.GetSection(nameof(MailSettings)).Get<Zhetistik.Data.MailAccess.MailSettings>();
      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
      email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
      email.Subject = mailRequest.Subject;
      var builder = new BodyBuilder();
      if(mailRequest.Attachments!=null)
      {
        byte[] fileBytes;
        foreach(var file in mailRequest.Attachments)
        {
          if(file.Length > 0)
          {
            using(var ms = new MemoryStream())
            {
              await file.CopyToAsync(ms);
              fileBytes = ms.ToArray();
            }
            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
          }
        }
      }
      builder.HtmlBody = mailRequest.Body;
      email.Body = builder.ToMessageBody();
      // Uri.TryCreate($"https://{_mailSettings.Host}", UriKind.RelativeOrAbsolute, out Uri uri);
      using var smtp = new SmtpClient();
      await smtp.ConnectAsync(_mailSettings.Host.ToString(), _mailSettings.Port, SecureSocketOptions.StartTls);
      await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
      await smtp.SendAsync(email);
      await smtp.DisconnectAsync(true);
    }
}