using System;
using System.Collections.Generic;
using System.Text;
using Utility.Enums;

namespace Utility.Entities
{
    public class Reminder
    {

    }

    public class Operation
    {

    }

    public enum PaymentType
    {
        Cleaning,
        Rent,
        Utilities
    }

    public class PaymentOperation : Operation
    {
        public decimal AmountInGBP { get; set; }
        public Currency Currency { get; set; }
        public PaymentType Type { get; set; }

        public override string ToString()
        {
            return $"payment ({CurrencySymbols.Map[Currency]}{AmountInGBP}) due for {Type.ToString()}";
        }
    }

    public class Task
    {
        public string AssignedTo { get; set; }
        public DateTime Date { get; set; }

        public List<Operation> Operations { get; set; }

        public override string ToString()
        {
            var ops = string.Join(", ", Operations.Select(o => o.ToString()));
            return $"{AssignedTo} {ops} on {ToString(Date)}";
        }

        static string ToString(DateTime dayOfWeek)
        {
            return (DateTime.Now.AddDays(1) == dayOfWeek) ? "tomorrow" : dayOfWeek.ToString();
        }
    }

    public class Schedule
    {
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }

    }
}
