using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using ClinicSystem.Interfaces;
using ClinicSystem.Models;


namespace ClinicSystem.Services
{
    public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;

		public MailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}
		public async Task SendEmailAsync(MailRequest mailRequest)
		{
			try
			{
				if (string.IsNullOrEmpty(mailRequest.ToEmail))
					throw new ArgumentNullException(nameof(mailRequest.ToEmail), "Recipient email cannot be null or empty");

				if (string.IsNullOrEmpty(mailRequest.Subject))
					throw new ArgumentNullException(nameof(mailRequest.Subject), "Email subject cannot be null or empty");

				if (string.IsNullOrEmpty(mailRequest.Body))
					throw new ArgumentNullException(nameof(mailRequest.Body), "Email body cannot be null or empty");
				var email = new MimeMessage();
				email.Sender = MailboxAddress.Parse(_mailSettings.Email);   
				email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));  
				email.Subject = mailRequest.Subject;                       

				var builder = new BodyBuilder
				{
					HtmlBody = mailRequest.Body                            
				};
				email.Body = builder.ToMessageBody();

				using (var smtp = new SmtpClient())
				{
					smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
					await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
					await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
					await smtp.SendAsync(email);
					await smtp.DisconnectAsync(true);
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to send email: {ex.Message}", ex);
			}
		}

	}
}

