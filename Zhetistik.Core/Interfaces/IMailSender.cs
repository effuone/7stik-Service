using Zhetistik.Data.MailAccess;

namespace Zhetistik.Core.Interfaces
{
    public interface IMailSender
    {
        public Task SendEmailAsync(MailRequest mailRequest);
    }
}