using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.PLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailASync (string toemail,string Subject,string Body);
    }
}
