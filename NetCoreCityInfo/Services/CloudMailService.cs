using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = Startup.Configuration["mailSetting:mailToAddress"];
        private string _mailFrom = Startup.Configuration["mailSetting:mailFromAddress"];

        public void Send(string subject, string message)
        {
            // send mail output to debug window
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with cloud mail service.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
