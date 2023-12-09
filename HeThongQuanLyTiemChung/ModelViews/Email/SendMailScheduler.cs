using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews.Email
{
    
    public class SendMailScheduler : IJob
    {
        private readonly db_VaccineContext _context;

        private ISendMailService _mailService;
        public async Task Execute(IJobExecutionContext context)
        {

            ExecuteTask();
        }

        public SendMailScheduler(db_VaccineContext context, ISendMailService mailService)
        {
            _context = context;           
            _mailService = mailService;
        }

        private void ExecuteTask()
        {


            var ngayhen = _context.AppointDates.Include(p => p.Customer).AsNoTracking().Where(p => p.AppointmentDate == DateTime.Now).FirstOrDefault();
            var khachhang = _context.Customers.AsNoTracking().Where(p => p.CustomerId == ngayhen.CustomerId).ToList();

            //Lấy dịch vụ sendmailservice


            var content = new MailContent();


            content.To = "haob1909909@student.ctu.edu.vn";
            content.Subject = "Kiểm tra thử";
            content.Body = "Test at " + DateTime.Now;

             _mailService.SendMail(content);

        }
      

    }
}
