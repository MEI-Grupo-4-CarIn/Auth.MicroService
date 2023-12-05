using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.MicroService.Application.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Auth.MicroService.Application.Services
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmail(string email, string token, CancellationToken ct)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Username"]));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = "Auth.MicroService - Password Reset Request";
            emailMessage.Body = new TextPart("plain") 
            { 
                Text = $"Dear user,\n\n" +
                "You have requested to reset your password. Please make a POST request to the /api/auth/resetPassword endpoint with the following JSON body:\n\n" +
                "{\n" +
                $"  \"token\": \"{token}\",\n" +
                "  \"newPassword\": \"your new password\"\n" +
                "}\n\n" +
                "Please replace \"your new password\" with your new password.\n\n" +
                "If you did not request a password reset, please ignore this email.\n\n" +
                "Disclaimer: This is a project for a master's degree and is not intended for real use.\n\n" +
                "Best,\n" +
                "Auth.MicroService Team"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["Email:Smtp:Host"], int.Parse(_configuration["Email:Smtp:Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["Email:Smtp:Username"], Environment.GetEnvironmentVariable("SMTP_PASSWORD"));

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}