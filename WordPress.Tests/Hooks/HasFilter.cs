using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class HasFilter
    {
        [Fact]
        public async void TestHasFilter()
        {
            var hook = new WpHook();
            var priority = 10;

            hook.AddFilter("", Mocks.ReturnTrue, priority, 0);

            Assert.Equal(priority, hook.HasFilter("", Mocks.ReturnTrue));
        }

        [Fact]
        public async void TestHasFilterWithoutCallback()
        {
            var hook = new WpHook();
            var priority = 10;

            hook.AddFilter("", Mocks.ReturnTrue, priority, 0);

            Assert.True(hook.HasFilter());
        }

        [Fact]
        public async void TestNotHasFilterWithoutCallback()
        {
            var hook = new WpHook();
            Assert.False(hook.HasFilter());
        }

        [Fact]
        public async void TestNotHasFilterWithCallback()
        {
            var hook = new WpHook();
            Assert.Null(hook.HasFilter("", Mocks.ReturnTrue));
        }

        [Fact]
        public async void TestHasFilterWithWrongCallback()
        {
            var hook = new WpHook();
            var priority = 10;

            hook.AddFilter("", Mocks.ReturnTrue, priority, 0);

            Assert.Null(hook.HasFilter("", Mocks.ReturnFalse));
        }
    }
}
