using System;
using SampleBot.Services;
using Xunit;

namespace SampleBot.UnitTests
{
    public class TestHogeService
    {
        [Fact]
        public void TestIsHoge()
        {
            var svc = new HogeService();
            var result = svc.IsHoge();
            Assert.True(result);
        }
    }
}
