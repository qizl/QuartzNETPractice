using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace QuartzNETPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            runProgram().GetAwaiter().GetResult();
        }

        static async Task runProgram()
        {
            try
            {
                var props = new NameValueCollection
                {
                    ["quartz.serializer.type"] = "binary"
                };
                var factory = new StdSchedulerFactory(props);
                var scheduler = await factory.GetScheduler();

                await scheduler.Start();

                var job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    //.WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()) // 间隔10s秒执行一次
                    .WithCronSchedule("0 57 10 * * ?") // 每天10:57执行
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                await Task.Delay(TimeSpan.FromSeconds(60));

                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteAsync(se.ToString());
            }
        }
    }
}
