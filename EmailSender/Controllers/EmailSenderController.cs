using EmailSender.Configurations;
using EmailSender.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EmailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailSenderController : ControllerBase
    {
        private readonly SMTPSetting _smtpSettings;

        public EmailSenderController(IOptions<SMTPSetting> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        [HttpPost(Name = "SendEmail")]
        public IActionResult SendEmail([FromForm] EmailDTO emailDto)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage(_smtpSettings.Email, emailDto.To))
                {
                    mailMessage.Subject = emailDto.Subject;
                    mailMessage.Body = emailDto.Body;

                    if (!string.IsNullOrEmpty(emailDto.CC))
                        mailMessage.CC.Add(emailDto.CC);

                    if (emailDto.Attachment != null)
                    {
                        string fileName = Path.GetFileName(emailDto.Attachment.FileName);
                        mailMessage.Attachments.Add(new Attachment(emailDto.Attachment.OpenReadStream(), fileName));
                    }
                    mailMessage.IsBodyHtml = false;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _smtpSettings.Host;
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = _smtpSettings.Port;
                        smtp.Send(mailMessage);

                        return Ok("Email Sent Succesfully!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error Occurred while sending Email: {ex.Message}!");
            }
        }
    }
}
