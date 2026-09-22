using CleanserBlazorUI.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ICleanserBlazorUI.Interface;
// Services/EmailSender.cs
public interface ICustomEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendEmailAsync(string email, string subject, string message,string isit__R_rest__F_Forget__C_Confirm);
}

public class EmailSender : ICustomEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailSender> _logger;
    private readonly IWebHostEnvironment _env;
    public EmailSender(IOptions<SmtpSettings> settings, ILogger<EmailSender> logger, IWebHostEnvironment env)
    {
        _settings = settings.Value;
        _logger = logger;
        _env = env;
        string imagePath = Path.Combine(_env.WebRootPath, "images", "logocolor.jpg");
    }
    
//    public async Task SendEmailAsync(string email, string subject, string message, ApplicationUser _user)
//    {
//        ApplicationUser user = _user;

//        var theUser = user.Email.Split('@')[0];
//        var password = user.subject.Split("@@@")[1];
//        string htmlContent = $@"
//<!DOCTYPE html>
//<html lang=""en"">
//<head>
//  <meta charset=""UTF-8"">
//  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
//  <title>Account</title>
//  <style>
//    body, table, td, a {{
//      -webkit-text-size-adjust: 100%;
//      -ms-text-size-adjust: 100%;
//      margin: 0;
//      padding: 0;
//    }}
//    table, td {{
//      border-collapse: collapse;
//    }}
//    img {{
//      border: 0;
//      height: auto;
//      line-height: 100%;
//      outline: none;
//      text-decoration: none;
//    }}
//    body {{
//      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
//      background-color: #f9fafb;
//      color: #333;
//      line-height: 1.6;
//    }}
//    .email-container {{
//      max-width: 600px;
//      margin: 0 auto;
//      padding: 40px 20px;
//      background: linear-gradient(135deg, #ffffff 0%, #f0f4f8 100%);
//      border-radius: 16px;
//      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
//    }}
//    .header {{
//      text-align: center;
//      padding: 20px 0;
//    }}
//    .header img {{
//      max-width: 220px;
//      height: auto;
//      margin: 0 auto;
//    }}
//.button {{
//    display: inline-block;
//    margin: 30px 0;
//    padding: 14px 30px;
//    font-size: 16px;
//    color: #fff;
//    background: #00AB55;
//    text-decoration: none;
//    border-radius: 8px;
//    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
//    transition: transform 0.3s ease, box-shadow 0.3s ease;
//}}
//  </style>
//</head>
//<body>
//  <div class=""email-container"">
//    <div class=""header"">
//      <!-- Reference the inline image using cid:logoImage -->
//      <img src=""cid:logoImage"" alt=""XDSdata Ghana Logo"" style=""max-width: 220px; height: auto;"">
//    </div>
//    <div class=""content"">
//      <h1 style=""font-size: 28px; color: #1e293b; margin-bottom: 20px;"">Hello {theUser},</h1>
//      <p>We received a request to reset your password. If this wasn't you, please <span class=""highlight"">ignore this email</span>.</p>
//      <a href=""{message}"" class=""button"">Reset Password</a>
//      <p>If the button above doesn't work, copy and paste this link:</p>
//      <p><a href=""{message}"" style=""color: #6366f1;"">{message}</a></p>
//    </div>
//    <div class=""divider""></div>
//    <div class=""footer"">
//      <p>Need help? Contact <a href=""mailto:noreply.XDSmonitor@XDSdatagh.com"">noreply.XDSmonitor@XDSdatagh.com</a>.</p>
//      <p>&copy; {DateTime.Now.Year} XDS DATA GHANA. All rights reserved.</p>
//    </div>
//  </div>
//</body>
//</html>
//";

//        try
//        {
//            // Define the path to your image (e.g., from wwwroot/images/logo.png)
//            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logocolor.png");

//            // Create a new MailMessage
//            var mail = new MailMessage
//            {
//                From = new MailAddress(_settings.FromEmail),
//                Subject = subject,
//                IsBodyHtml = true
//            };
//            mail.To.Add(user.Email);

//            // Create an AlternateView for the HTML content
//            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");

//            // Create the LinkedResource for the image and set its ContentId
//            LinkedResource inlineLogo = new LinkedResource(imagePath)
//            {
//                ContentId = "logoImage",  // Must match the CID in the img tag
//                TransferEncoding = System.Net.Mime.TransferEncoding.Base64
//            };

//            // Add the LinkedResource to the AlternateView
//            htmlView.LinkedResources.Add(inlineLogo);

//            // Add the AlternateView to the MailMessage
//            mail.AlternateViews.Add(htmlView);

//            // Configure and send the email using SmtpClient
//            using (var client = new SmtpClient(_settings.Server, _settings.Port))
//            {
//                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
//                client.EnableSsl = _settings.EnableSsl;
//                await client.SendMailAsync(mail);
//            }

//            _logger.LogInformation($"Email sent to {email}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Email sending failed");
//            throw; // Re-throw to maintain compatibility with Identity
//        }
//    }

    public async Task SendEmailAsync(string email, string _subject, string htmlMessage)
    {
        string subject = _subject.Split('_')[0];
        string password = _subject.Split('_')[1] ?? string.Empty;
        string theUser = email.Split('@')[0];
        string htmlContent = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Account</title>
  <style>
    body, table, td, a {{
      -webkit-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
      margin: 0;
      padding: 0;
    }}
    table, td {{
      border-collapse: collapse;
    }}
    img {{
      border: 0;
      height: auto;
      line-height: 100%;
      outline: none;
      text-decoration: none;
    }}
    body {{
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      background-color: #f9fafb;
      color: #333;
      line-height: 1.6;
    }}
    .email-container {{
      max-width: 600px;
      margin: 0 auto;
      padding: 40px 20px;
      background: linear-gradient(135deg, #ffffff 0%, #f0f4f8 100%);
      border-radius: 16px;
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    }}
    .header {{
      text-align: center;
      padding: 20px 0;
    }}
    .header img {{
      max-width: 220px;
      height: auto;
      margin: 0 auto;
    }}
.button {{
    display: inline-block;
    margin: 30px 0;
    padding: 14px 30px;
    font-size: 16px;
    color: #fff;
    background: #00AB55;
    text-decoration: none;
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    transition: transform 0.3s ease, box-shadow 0.3s ease;
}}
  </style>
</head>
<body>
  <div class=""email-container"">
    <div class=""header"">
      <!-- Reference the inline image using cid:logoImage -->
      <img src=""cid:logoImage"" alt=""XDSdata Ghana Logo"" style=""max-width: 220px; height: auto;"">
    </div>
    <div class=""content"">
      <h1 style=""font-size: 28px; color: #1e293b; margin-bottom: 20px;"">Hello {theUser},</h1>
      <p>We received a request to reset your password. If this wasn't you, please <span class=""highlight"">ignore this email</span>.</p>
      <a href=""{htmlMessage}"" class=""button"">Confirm Password</a>
      <p>If the button above doesn't work, copy and paste this link:</p>
      <p><a href=""{htmlMessage}"" style=""color: #6366f1;"">{htmlMessage}</a></p>
  <div style=""max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); padding: 30px;"">
        <h1 style=""color: #333333; font-size: 24px; margin-bottom: 20px; text-align: center;"">Your Account Details</h1>
        
        <div style=""background-color: #f2f2f2; padding: 20px; border-radius: 6px; margin-bottom: 20px;"">
            <p style=""margin: 0; font-size: 16px; color: #555555;""><strong>Username:</strong> <span style=""color: #007BFF;"">{email}</span></p>
        </div>

        <div style=""background-color: #f2f2f2; padding: 20px; border-radius: 6px;"">
            <p style=""margin: 0; font-size: 16px; color: #555555;""><strong>Password:</strong> <span style=""color: #007BFF;"">{password}</span></p>
        </div>

        <p style=""text-align: center; margin-top: 30px; font-size: 14px; color: #888888;"">
            Please keep your account details safe and do not share them with anyone.
        </p>
    </div>
    </div>
    <div class=""divider""></div>
    <div class=""footer"">
      <p>Need help? Contact <a href=""mailto:noreply.XDSmonitor@XDSdatagh.com"">noreply.XDSmonitor@XDSdatagh.com</a>.</p>
      <p>&copy; {DateTime.Now.Year} XDS DATA GHANA. All rights reserved.</p>
    </div>
  </div>
</body>
</html>
";
        try
        {
            // Define the path to your image (e.g., from wwwroot/images/logo.png)
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logocolor.jpg");

            // Create a new MailMessage
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            // Create an AlternateView for the HTML content
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");

            // Create the LinkedResource for the image and set its ContentId
            LinkedResource inlineLogo = new LinkedResource(imagePath)
            {
                ContentId = "logoImage",  // Must match the CID in the img tag
                TransferEncoding = System.Net.Mime.TransferEncoding.Base64
            };

            // Add the LinkedResource to the AlternateView
            htmlView.LinkedResources.Add(inlineLogo);

            // Add the AlternateView to the MailMessage
            mail.AlternateViews.Add(htmlView);

            // Configure and send the email using SmtpClient
            using (var client = new SmtpClient(_settings.Server, _settings.Port))
            {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                client.EnableSsl = _settings.EnableSsl;
                await client.SendMailAsync(mail);
            }

            _logger.LogInformation($"Email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed");
            throw; // Re-throw to maintain compatibility with Identity
        }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, string isit__R_rest__F_Forget__C_Confirm)
    {
        string buttonText = string.Empty;
        switch (isit__R_rest__F_Forget__C_Confirm)
        {
            case "r":
                buttonText = "Reset Password";
                subject = "Reset Password";
                break;
            case "f":
                buttonText = "Forget Password";
                subject = "Forget Password";
                break;
            case "c":
                buttonText = "Please Reset Password";
                subject = "Please Reset Password";
                break;
        }

        var theUser = email.Split('@')[0];
        string htmlContent = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{buttonText}</title>
  <style>
    body, table, td, a {{
      -webkit-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
      margin: 0;
      padding: 0;
    }}
    table, td {{
      border-collapse: collapse;
    }}
    img {{
      border: 0;
      height: auto;
      line-height: 100%;
      outline: none;
      text-decoration: none;
    }}
    body {{
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      background-color: #f9fafb;
      color: #333;
      line-height: 1.6;
    }}
    .email-container {{
      max-width: 600px;
      margin: 0 auto;
      padding: 40px 20px;
      background: linear-gradient(135deg, #ffffff 0%, #f0f4f8 100%);
      border-radius: 16px;
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    }}
    .header {{
      text-align: center;
      padding: 20px 0;
    }}
    .header img {{
      max-width: 220px;
      height: auto;
      margin: 0 auto;
    }}
.button {{
    display: inline-block;
    margin: 30px 0;
    padding: 14px 30px;
    font-size: 16px;
    color: #fff;
    background: #00AB55;
    text-decoration: none;
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    transition: transform 0.3s ease, box-shadow 0.3s ease;
}}
  </style>
</head>
<body>
  <div class=""email-container"">
    <div class=""header"">
      <!-- Reference the inline image using cid:logoImage -->
      <img src=""cid:logoImage"" alt=""XDSdata Ghana Logo"" style=""max-width: 220px; height: auto;"">
    </div>
    <div class=""content"">
      <h1 style=""font-size: 28px; color: #1e293b; margin-bottom: 20px;"">Hello {theUser},</h1>
      <p>We received a request to reset your password. If this wasn't you, please <span class=""highlight"">ignore this email</span>.</p>
      <a href=""{htmlMessage}"" class=""button"">{buttonText}</a>
      <p>If the button above doesn't work, copy and paste this link:</p>
      <p><a href=""{htmlMessage}"" style=""color: #6366f1;"">{htmlMessage}</a></p>
    </div>
    <div class=""divider""></div>
    <div class=""footer"">
      <p>Need help? Contact <a href=""mailto:noreply.XDSmonitor@XDSdatagh.com"">noreply.XDSmonitor@XDSdatagh.com</a>.</p>
      <p>&copy; {DateTime.Now.Year} XDS DATA GHANA. All rights reserved.</p>
    </div>
  </div>
</body>
</html>
";
        try
        {
            // Define the path to your image (e.g., from wwwroot/images/logo.png)
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logocolor.jpg");

            // Create a new MailMessage
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = buttonText,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            // Create an AlternateView for the HTML content
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");

            // Create the LinkedResource for the image and set its ContentId
            LinkedResource inlineLogo = new LinkedResource(imagePath)
            {
                ContentId = "logoImage",  // Must match the CID in the img tag
                TransferEncoding = System.Net.Mime.TransferEncoding.Base64
            };

            // Add the LinkedResource to the AlternateView
            htmlView.LinkedResources.Add(inlineLogo);

            // Add the AlternateView to the MailMessage
            mail.AlternateViews.Add(htmlView);

            // Configure and send the email using SmtpClient
            using (var client = new SmtpClient(_settings.Server, _settings.Port))
            {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                client.EnableSsl = _settings.EnableSsl;
                await client.SendMailAsync(mail);
            }

            _logger.LogInformation($"Email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed");
            throw; // Re-throw to maintain compatibility with Identity
        }
    }
}