using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class DoAllHook
    {
        [Fact]
        public async void TestDoAllHookWithMultipleCalls()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Action");
            var tag = "all";

            hook.AddFilter(tag, callback, 10, 1);
            await hook.DoAllHook(new object[0]);
            await hook.DoAllHook(new object[0]);

            Assert.Equal(2, a.GetCallCount());
        }
    }
}
