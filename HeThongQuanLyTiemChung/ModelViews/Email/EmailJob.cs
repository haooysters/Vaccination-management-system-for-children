
using Microsoft.AspNetCore.Mvc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews.Email
{
    public class EmailJob : IJob
    {

        private readonly ISendMailService _mailService;
        public EmailJob(ISendMailService mailService)
        {           
            _mailService = mailService;
        }

        public async Task Execute(IJobExecutionContext context)

        {

            var content = new MailContent();


            content.To = "haob1909909@student.ctu.edu.vn";
            content.Subject = "Kiểm tra thử";
            content.Body = "Test at " + DateTime.Now;

            await _mailService.SendMail(content);

        }
    

    }
}
