﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HeThongQuanLyTiemChung.ModelViews
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
