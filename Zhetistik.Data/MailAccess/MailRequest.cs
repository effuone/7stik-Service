using Microsoft.AspNetCore.Http;

namespace Zhetistik.Data.MailAccess
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}