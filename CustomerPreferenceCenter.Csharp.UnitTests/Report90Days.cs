using LanguageExt;
using LanguageExt.UnitTesting;
using Shouldly;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace CustomerPreferenceCentre.Csharp.UnitTests
{
    public class Report90Days
    {
        [Fact]
        public void Contains90Days()
        {
            Reports.Create90DayReport(
                customerContactRecurrence: Map<Customer, Option<IRecurrence>>(),
                starting: new DateTime(2018, 4, 1)).Length.ShouldBe(90);
        }

        [Fact]
        public void StartDate()
        {
            Reports.Create90DayReport(
                customerContactRecurrence: Map<Customer, Option<IRecurrence>>(),
                starting: new DateTime(2018, 4, 1)).Head().Key.ShouldBe(new DateTime(2018, 4, 1));
        }

        [Fact]
        public void EndDate()
        {
            Reports.Create90DayReport(
                customerContactRecurrence: Map<Customer, Option<IRecurrence>>(),
                starting: new DateTime(2018, 4, 1)).LastOrNone().ShouldBeSome(last => last.Key.ShouldBe(new DateTime(2018, 6, 29)));
        }

        [Fact]
        public void First14Records()
        {
            Reports.Create90DayReport(
                customerContactRecurrence: Map<Customer, Option<IRecurrence>>(
                    (Customer.New('A'), EveryDay.New()),
                    (Customer.New('B'), MonthDay.New(10)),
                    (Customer.New('C'), WeekDays.NewArgs(WeekDay.New(DayOfWeek.Tuesday), WeekDay.New(DayOfWeek.Friday)))
                    ),
                starting: new DateTime(2018, 4, 1)).ShouldSatisfyAllConditions(
                    res => res[new DateTime(2018, 4, 1)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 2)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 3)].ShouldBe(Seq(Customer.New('A'), Customer.New('C'))),
                    res => res[new DateTime(2018, 4, 4)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 5)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 6)].ShouldBe(Seq(Customer.New('A'), Customer.New('C'))),
                    res => res[new DateTime(2018, 4, 7)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 8)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 9)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 10)].ShouldBe(Seq(Customer.New('A'), Customer.New('B'), Customer.New('C'))),
                    res => res[new DateTime(2018, 4, 11)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 12)].ShouldBe(Seq1(Customer.New('A'))),
                    res => res[new DateTime(2018, 4, 13)].ShouldBe(Seq(Customer.New('A'), Customer.New('C'))),
                    res => res[new DateTime(2018, 4, 14)].ShouldBe(Seq1(Customer.New('A')))
                    );
        }
    }
}
