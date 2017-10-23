using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class HasFilters
    {
        [Fact]
        public async void TestWithCallback()
        {
            var hook = new WpHook();

            hook.AddFilter("", Mocks.ReturnTrue, 10, 1);

            Assert.True(hook.HasFilters());
        }

        [Fact]
        public async void TestWithouCallback()
        {
            var hook = new WpHook();
            Assert.False(hook.HasFilters());
        }

        [Fact]
        public async void TestRemovedCallback()
        {
            var hook = new WpHook();

            hook.AddFilter("", Mocks.ReturnTrue, 10, 1);
            hook.RemoveFilter("", Mocks.ReturnTrue, 10);

            Assert.False(hook.HasFilters());
        }
    }
}
