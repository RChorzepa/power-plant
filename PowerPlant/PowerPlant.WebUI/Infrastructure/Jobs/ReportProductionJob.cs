using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Persistence;
using PowerPlant.Infrastructure.Services.MailService;
using PowerPlant.Infrastructure.Services.MailService.Models;
using PowerPlant.Infrastructure.Services.MailServices.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlant.WebUI.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ReportProductionJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IProductionRepositoryAsync _repository;

        public ReportProductionJob(IProductionRepositoryAsync repository, IMailService mailService, IConfiguration configuration)
        {
            _repository = repository;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
       
                var data = await _repository.GetReportByDatyAsync(DateTime.Now);

                foreach (var item in data)
                {
                    var message = new MessageConfiguration
                    {
                        From = _configuration["Settings:ReportNotification:From"],
                        To = _configuration["Settings:ReportNotification:To"].Split(", "),
                        Subject = $"Report for generator: {item.Key}",
                        Attachments = new List<Attachment>
                    {
                        new Attachment
                        {
                            Name = $"Generator_{item.Key}_{DateTime.Now.ToShortDateString()}.xlsx",
                            Content = CreateFileContent(item.Value)
                        }
                    }
                    };

                    await _mailService.SendMessage(message);
                }
        }

        private byte[] CreateFileContent(IEnumerable<(DateTime date, double avg)> data)
        {
            byte[] content;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");

                int row = 1;

                worksheet.Cell(row, 1).Value = "Date";
                worksheet.Cell(row, 2).Value = "Avg";

                foreach (var item in data)
                {
                    ++row;
                    worksheet.Cell(row, 1).Value = item.date;
                    worksheet.Cell(row, 2).Value = item.avg;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                }

            }

            return content;
        }
    }
}
