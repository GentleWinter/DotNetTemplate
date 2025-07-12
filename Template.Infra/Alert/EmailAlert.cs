using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sasquat.Infra.Alerts
{
    public class EmailAlert
    {
        private readonly ILogger<EmailAlert> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _from;

        public EmailAlert(IConfiguration configuration, ILogger<EmailAlert> logger)
        {
            _logger = logger;
            
            _smtpHost = configuration["Email:SmtpHost"] ?? throw new ArgumentException("SmtpHost");
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? throw new ArgumentException("StmpPort"));
            _smtpUsername = configuration["Email:Username"] ?? throw new ArgumentException("Username");
            _from = configuration["Email:From"] ?? throw new ArgumentException("From");
            _smtpPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new ArgumentException("EmailPassword");
        }

        public async Task OnboardingAlertAsync(string contact, string checkoutUrl)
        {
            try
            {
                var htmlBody = await LoadTemplateAsync("OnboardingModel.html");
                htmlBody = htmlBody.Replace("{{paymentUrl}}", checkoutUrl);
                await SendEmailAsync(contact, "Welcome to Sasquat! 🐾", htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Failed to send onboarding email to {contact}: {Message}", contact, ex.Message);
            }
        }
        
        public async Task<bool> AuthCode(string contact, string authCode)
        {
            try
            {
                var htmlBody = await LoadTemplateAsync("AuthCodeModel.html");
                htmlBody = htmlBody.Replace("{{authCode}}", authCode);
                await SendEmailAsync(contact, "Your auth code! 🐾", htmlBody);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Failed to send onboarding email to {contact}: {Message}", contact, ex.Message);
                return false;
            }
        }
        
        private static async Task<string> LoadTemplateAsync(string fileName)
        {
            var assembly = typeof(EmailAlert).Assembly;
            var resourceName = $"Sasquat.Infra.Alerts.Templates.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException($"Resource '{resourceName}' not found.");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var mail = new MailMessage(_from, to)
                {
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mail.AlternateViews.Add(CreateAlternateViewWithMascot(htmlBody));

                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mail);
                _logger.LogInformation("Email sent to {to}.", to);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[ERROR] Failed to send email to {to}: {message}", to, ex.Message);
            }
        }
        
        private static AlternateView CreateAlternateViewWithMascot(string htmlBody)
        {
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            var assembly = typeof(EmailAlert).Assembly;

            var memoryStream = new MemoryStream();
            memoryStream.Position = 0;

            return alternateView;
        }
    }
}
