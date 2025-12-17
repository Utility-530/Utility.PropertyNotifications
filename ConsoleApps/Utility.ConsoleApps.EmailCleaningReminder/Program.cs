// See https://aka.ms/new-console-template for more information

using SQLite;
using Utility.Entities;
using Utility.Persists;

SQLitePCL.Batteries.Init();
using var conn = new SQLiteConnection(Utility.Constants.Paths.DefaultModelsFilePath, true);
var people = conn.Table<Person>().Where(a => a.FirstName == "Declan" || a.FirstName == "Fergus").ToList();
people.Reverse();
var schedule = Utility.Services.ScheduleFactory.CreatePaymentSchedule(people, DayOfWeek.Monday, 70);
var message = Utility.Services.EmailReminderService.ToMessage(schedule);
Utility.Services.EmailReminderService.Send(message, people);
Console.WriteLine("Finished");
