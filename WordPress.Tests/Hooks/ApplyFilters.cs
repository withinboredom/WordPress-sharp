using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class ApplyFilters
    {
        [Fact]
        public async void TestApplyFiltersWithCallback()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Filter");
            var tag = "tag";
            var priority = 10;
            var acceptedArgs = 1;
            var arg = "arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);

            var result = await hook.ApplyFilters(arg, new object[] { arg });

            Assert.Equal(arg, result);
            Assert.Equal(1, a.GetCallCount());
        }

        [Fact]
        public async void TestApplyFiltersWithMultipleCalls()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Filter");
            var tag = "tag";
            var priority = 10;
            var acceptedArgs = 1;
            var arg = "arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);

            var result1 = await hook.ApplyFilters(arg, new object[] { arg });
            var result2 = await hook.ApplyFilters(result1, new [] {result1});

            Assert.Equal(arg, result2);
            Assert.Equal(2, a.GetCallCount());
        }
    }
}
