using ClinicSystem.Models;

namespace ClinicSystem.Interfaces
{
	public interface IMailService
	{
		Task SendEmailAsync(MailRequest mailRequest);
	}
}
