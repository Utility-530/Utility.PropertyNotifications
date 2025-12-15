using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Utility.Entities;
using Utility.Enums;
using Utility.Services.Meta;
using Operation = Utility.Entities.Operation;
using Person = Utility.Entities.Person;
using Task = Utility.Entities.Task;


namespace Utility.Services
{
    public record ScheduleInputParam() : Param<EmailReminderService>(nameof(EmailReminderService.ToMessage), "schedule");
    public record MimeMessageReturnParam() : Param<EmailReminderService>(nameof(EmailReminderService.ToMessage));
    public record MimeMessageInputParam() : Param<EmailReminderService>(nameof(EmailReminderService.Send), "message");
    public record PeopleInputParam() : Param<EmailReminderService>(nameof(EmailReminderService.Send), "people");
    public record PeopleInputCreatePaymentParam() : Param<ScheduleFactory>(nameof(ScheduleFactory.CreatePaymentSchedule), "people");
    public record DayOfWeekInputParam() : Param<ScheduleFactory>(nameof(ScheduleFactory.CreatePaymentSchedule), "dayOfWeek");
    public record AmountInGBPInputParam() : Param<ScheduleFactory>(nameof(ScheduleFactory.CreatePaymentSchedule), "amountInGBP");
    public record ScheduleOutputParam() : Param<ScheduleFactory>(nameof(ScheduleFactory.CreatePaymentSchedule));


    public class EmailReminderService
    {
        const string nameFrom = "Declan";
        const string emailFrom = "rolyat289@gmail.com";
        const string passwordFrom = "W3Qcgshxz1JLCEyk";

        public static MimeMessage ToMessage(Schedule schedule)
        {
            //var str = GeneralParserService.ToHtmlString(PropertyDescriptors.DescriptorFactory.CreateRoot(schedule));

            return new MimeMessage
            {
                Subject = $"Reminder: {schedule.Tasks[0].ToString()}",
                Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {

                    Text = ToBody(schedule)
                }
            };
        }

        public static string ToBody(Schedule schedule)
        {
            return $"<h1>Schedule</h1>" +
                    $"<p><b>{schedule.Tasks[0].ToString()}</b></p>" +
                    $"<p>{schedule.Tasks[1].ToString()} </p>" +
                    $"<p>{schedule.Tasks[2].ToString()} </p>";
        }

        public static void Send(MimeMessage message, IEnumerable<Utility.Entities.Person> people)
        {
            message.From.Add(new MailboxAddress(nameFrom, emailFrom));

            foreach (var payer in people)
                message.To.Add(new MailboxAddress(payer.FirstName, payer.EmailAddress));

            using var smtp = new SmtpClient { };
            smtp.Connect("smtp-relay.brevo.com", 587, false);
            smtp.Authenticate(emailFrom, passwordFrom);

            smtp.Send(message);
            smtp.Disconnect(true);
        }

        public static DateTime NextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }

    public class ScheduleFactory
    {
        //const int amountInGBP = 70;

        static string Name(IEnumerable<Person> people, DayOfWeek dayOfWeek, int number = 0)
        {
            //var names = new[] { "Finnian", "Declan", "Fergus" };
            var names = people.Select(a => a.FirstName).ToArray();
            DateTime inputDate = DateTime.Now;
            CultureInfo cul = CultureInfo.CurrentCulture;

            int weekNum = cul.Calendar.GetWeekOfYear(
                inputDate,
                CalendarWeekRule.FirstDay,
                dayOfWeek);

            return names[(number + weekNum) % names.Length];
        }


        public static Schedule CreatePaymentSchedule(IEnumerable<Person> people, DayOfWeek dayOfWeek, decimal amountInGBP)
        {

            return new Schedule
            {
                Name = "Demo Schedule",
                Tasks = new List<Task>
                {
                    new Task
                    {
                        AssignedTo = Name(people, dayOfWeek, 0),
                        Date = EmailReminderService.NextWeekday(DateTime.Now, dayOfWeek),
                        Operations = new List<Operation>
                        {
                            new PaymentOperation
                            {
                                AmountInGBP = amountInGBP,
                                Currency = Currency.GBP,
                                Type = PaymentType.Cleaning
                            }
                        }
                    },
                    new Task
                    {
                        AssignedTo =  Name(people, dayOfWeek, 1),
                        Date = EmailReminderService.NextWeekday(DateTime.Now, dayOfWeek).AddDays(7),
                        Operations = new List<Operation>
                        {
                            new PaymentOperation
                            {
                                AmountInGBP = amountInGBP,
                                Currency = Currency.GBP,
                                Type = PaymentType.Cleaning
                            }
                        }
                    },
                    new Task
                    {
                        AssignedTo =  Name(people, dayOfWeek, 2),
                        Date = EmailReminderService.NextWeekday(DateTime.Now, dayOfWeek).AddDays(14),
                        Operations = new List<Operation>
                        {
                            new PaymentOperation
                            {
                                AmountInGBP = amountInGBP,
                                Currency = Currency.GBP,
                                Type = PaymentType.Cleaning
                            }
                        }
                    }
                }
            };
        }
    }


}
