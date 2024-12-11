using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace MyAspNetApp.Controllers
{
    public class TokenController : Controller
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("generate-token")]
        public IActionResult GenerateToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            string token = Guid.NewGuid().ToString();
            SendVerificationEmail(email, token);

            return Ok($"Generated Token: {token}. An email has been sent to {email} with the token.");
        }

        private void SendVerificationEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            var fromAddress = _configuration["Smtp:Username"];
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("Sender email (Smtp:Username) cannot be null or empty.");
            }

            var verificationLink = Url.Action("VerifyEmail", "Token", new { token }, Request.Scheme);
            var subject = "Xác thực email của bạn";
            var body = $"<h1>Xác thực tài khoản</h1><p>Vui lòng nhấp vào <a href=\"{verificationLink}\">đây</a> để xác thực tài khoản của bạn.</p>";

            using (var client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"])))
            {
                client.Credentials = new NetworkCredential(fromAddress, _configuration["Smtp:Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, "Your Name"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Thêm email vào danh sách người nhận
                mailMessage.To.Add(email);

                client.Send(mailMessage);
            }
        }



        public IActionResult VerifyEmail(string token)
        {
            return Ok($"Email verified with token: {token}");
        }
    }
}
