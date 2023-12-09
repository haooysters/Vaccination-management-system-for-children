using HeThongQuanLyTiemChung.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews.Email
{
   


    public class JobScheduler
    {
        

        public static void Start()

        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();           


            IJobDetail job = JobBuilder.Create<EmailJob>().Build();



            ITrigger trigger = TriggerBuilder.Create()

                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInHours(24)

                     //s.WithIntervalInSeconds(20)

                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(18, 51))

                  )

                .Build();



            scheduler.ScheduleJob(job, trigger);

        }

    }
}
