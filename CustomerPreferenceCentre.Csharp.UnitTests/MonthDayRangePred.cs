using Shouldly;
using Xunit;

namespace CustomerPreferenceCentre.Csharp.UnitTests
{
    public class MonthDayRangePred
    {
        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(14, true)]
        [InlineData(28, true)]
        [InlineData(29, false)]
        public void Pred(int day, bool expected)
        {
            MonthDayRange.Is.True(day).ShouldBe(expected);
        }
    }
}
