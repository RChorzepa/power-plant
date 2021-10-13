using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerPlant.WebUI.Infrastructure.Jobs;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlant.WebUI.Infrastructure.Services
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;

        public IScheduler Scheduler { get; set; }

        public QuartzHostedService( IServiceProvider serviceProvider)
        {
            using(var scope = serviceProvider.CreateScope())
            {
                _schedulerFactory = scope.ServiceProvider.GetService<ISchedulerFactory>();
                _jobFactory = scope.ServiceProvider.GetService<IJobFactory>();
                _jobSchedules = scope.ServiceProvider.GetService<IEnumerable<JobSchedule>>();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                await Scheduler.ScheduleJob(
                   jobDetail: JobBuilder
                        .Create(jobSchedule.JobType)
                       .WithIdentity(jobSchedule.JobType.FullName)
                       .WithDescription(jobSchedule.JobType.Name)
                       .Build(),
                   trigger: TriggerBuilder
                       .Create()
                       .WithIdentity($"{jobSchedule.JobType.FullName}.trigger")
                       .WithCronSchedule(jobSchedule.CronExpression)
                       .WithDescription(jobSchedule.CronExpression)
                       .Build(),
                   cancellationToken: cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }
    }
}
