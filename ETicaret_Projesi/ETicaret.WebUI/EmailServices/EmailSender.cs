using System.Net;
using System.Net.Mail;

namespace ETicaret.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        // Bu değişkenler Mail göndermek için SMTP Server'ının ihtiyacı olan değişkenler. (SMTP server = Mail gönderen server)
        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private bool _enableSSL;
        public EmailSender(string host, int port, string username, string password, bool enableSSL)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _enableSSL = enableSSL;
        }

        // email: kime göndereceğiz = To
        // subject: Konusu
        // htmlMessage: mesajın içeriği
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = _enableSSL
            };
            return client.SendMailAsync(new MailMessage(_username, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            });
        }
    }
}
