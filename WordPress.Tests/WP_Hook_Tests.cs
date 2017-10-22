using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests
{
    public class WpHookTest
    {
        [Fact]
        public async void TestIdentity()
        {
            var hook = new WpHook();
            var result = (bool)await hook.ApplyFilters(true);
            Assert.False(hook.HasFilters());
            Assert.False(hook.HasFilter("", IdentityCallback).HasValue);
            Assert.False(hook.HasFilter(""));
            //result.Wait();
            Assert.True(result);
        }

        [Fact]
        public async void TestSimpleHook()
        {
            var hook = new WpHook();
            hook.AddFilter("test", IdentityCallback, 10, 0);
            Assert.True(hook.HasFilters());
            var priority = hook.HasFilter("test", IdentityCallback);
            Assert.True(priority.HasValue);
            Assert.Equal(10, priority.Value);
            var result = (bool)await hook.ApplyFilters(true);
            Assert.True(result);
        }

        [Fact]
        public async void Sum()
        {
            var hook = new WpHook();
            hook.AddFilter("", SumCallback, 10, 0);
            hook.AddFilter("", SumCallback, 15, 0);
            Assert.Equal(2, (int)await hook.ApplyFilters(0));
            hook.AddFilter("", FalseCallback, 20, 0);
            Assert.False((bool)await hook.ApplyFilters(0));
        }

        [Fact]
        public async void Nesting()
        {
            var hook = new WpHook();
            hook.AddFilter("", SumCallback, 10, 0);
            hook.AddFilter("", Nest(hook), 10, 0);
            var result = await hook.ApplyFilters(0);
            Assert.Equal(2, result);
        }

        [Fact]
        public async void Action()
        {
            var exec = false;
            var hook = new WpHook();
            hook.AddFilter("", async array => exec = true, 10, 0);
            await hook.DoAction();
            Assert.True(exec);
        }

        [Fact]
        public async void Priority()
        {
            var hook = new WpHook();
            hook.AddFilter("", async a =>
            {
                Assert.Equal(1, hook.CurrentPriority());
                return hook.CurrentPriority();
            }, 1, 0);
            hook.AddFilter("", async a =>
            {
                Assert.Equal(2, hook.CurrentPriority());
                return hook.CurrentPriority();
            }, 2, 0);
            await hook.DoAction();
        }

        private Func<IEnumerable, Task<object>> Nest(WpHook hook)
        {
            return async a =>
            {
                var ar = (Array)a;
                if ((int)ar.GetValue(0) < 2)
                {
                    return await hook.ApplyFilters(ar.GetValue(0));
                }

                return ar.GetValue(0);
            };
        }

        private async Task<object> IdentityCallback(IEnumerable array)
        {
            return ((Array)array).GetValue(0);
        }

        private async Task<object> FalseCallback(IEnumerable array)
        {
            return false;
        }

        private async Task<object> SumCallback(IEnumerable array)
        {
            return (int)((Array)array).GetValue(0) + 1;
        }
    }
}
