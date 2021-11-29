using LanguageExt;
using LanguageExt.TypeClasses;
using System;
using System.Linq;
using static LanguageExt.Prelude;

namespace CustomerPreferenceCentre.Csharp
{
    class Program
    {
        static void Main(string[] _)
        {
            var report = Reports.Create90DayReport(
                customerContactRecurrence: Map<Customer, Option<IRecurrence>>(
                    (Customer.New('A'), EveryDay.New()),
                    (Customer.New('B'), MonthDay.New(10)),
                    (Customer.New('C'), WeekDays.NewArgs(WeekDay.New(DayOfWeek.Tuesday), WeekDay.New(DayOfWeek.Friday)))
                    ),
                starting: new DateTime(2018, 4, 1));

            foreach (var (Date, Customers) in report)
            {
                Console.WriteLine($"{Date}   {string.Join(", ", Customers.Map(c => c.Value))}");
            }
        }
    }

    public static class Reports // Customer could be generic
    {
        public static Map<DateTime, Seq<Customer>> Create90DayReport(Map<Customer, Option<IRecurrence>> customerContactRecurrence, DateTime starting) =>
            Enumerable.Range(0, 90).
            Map(Convert.ToDouble).
            Map(starting.AddDays).
            ToDictionary(
                date => date,
                date => CustomersToContact(customerContactRecurrence, date)).
            ToMap();

        private static Seq<Customer> CustomersToContact(Map<Customer, Option<IRecurrence>> customerContactRecurrence, DateTime date) =>
            customerContactRecurrence.Fold(
                Seq<Customer>(),
                (customers, customer, recurrence) =>
                    customers.Concat(RecurrenceIncludesDate(recurrence, date) ? Seq1(customer) : Seq<Customer>()));

        private static bool RecurrenceIncludesDate(Option<IRecurrence> recurrence, DateTime date) =>
            recurrence.Map(r =>
                r switch
                {
                    EveryDay => true,
                    WeekDays wds => wds.Value.Exists(wd => wd.Value == date.DayOfWeek),
                    MonthDay md => md.Value == date.Day,
                    _ => false // could be better to throw an exception here so if new IRecurrence types are added, we get an exception to prompt adding a new case here
                }).IfNone(false);
    }

    public class Customer : NewType<Customer, char>
    {
        public Customer(char value) : base(value)
        {
        }
    }

    public interface IRecurrence
    {

    }

    public class MonthDay : NewType<MonthDay, int, MonthDayRange>, IRecurrence
    {
        public MonthDay(int value) : base(value)
        {
        }
    }

    public struct MonthDayRange : Pred<int>
    {
        public static readonly MonthDayRange Is = default;

        public bool True(int value) =>
            value >= 1 && value <= 28;
    }

    public class WeekDays : NewType<WeekDays, Seq<WeekDay>>, IRecurrence
    {
        public WeekDays(Seq<WeekDay> value) : base(value)
        {
        }

        public static WeekDays NewArgs(params WeekDay[] values) => New(values.ToSeq());
    }

    public class WeekDay : NewType<WeekDay, DayOfWeek>, IRecurrence
    {
        public WeekDay(DayOfWeek value) : base(value)
        {
        }
    }

    public class EveryDay : IRecurrence
    {
        public static EveryDay New() => new EveryDay();
    }
}
