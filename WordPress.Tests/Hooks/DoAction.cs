using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class DoAction
    {
        [Fact]
        public async void TestDoActionWithCallback()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Action");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(1, a.GetCallCount());
        }

        [Fact]
        public async void TestActionWithMultipleCalls()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Action");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });
            await hook.DoAction(new object[] { arg });

            Assert.Equal(2, a.GetCallCount());
        }

        [Fact]
        public async void TestActionWithMultipleCallbacksOnSamePriority()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var b = new Mocks.MockAction(0, hook);
            var callbackOne = a.Generate("Filter");
            var callbackTwo = b.Generate("Filter");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, callbackOne, priority, acceptedArgs);
            hook.AddFilter(tag, callbackTwo, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(1, a.GetCallCount());
            Assert.Equal(1, b.GetCallCount());
        }

        [Fact]
        public async void TestActionWithMultipleCallbacksOnDifferentPriority()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var b = new Mocks.MockAction(0, hook);
            var callbackOne = a.Generate("Filter");
            var callbackTwo = b.Generate("Filter");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, callbackOne, priority, acceptedArgs);
            hook.AddFilter(tag, callbackTwo, priority + 1, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(1, a.GetCallCount());
            Assert.Equal(1, b.GetCallCount());
        }

        [Fact]
        public async void TestDoActionWithNoAcceptedArgs()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Filter");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(0, a.Events[0].Args.Count());
        }

        [Fact]
        public async void TestDoAcionWithOneAcceptedArg()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Action");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 1;
            var arg = "_arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(1, a.Events[0].Args.Count());
        }

        [Fact]
        public async void TestDoActionWithManyAcceptedArgs()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var callback = a.Generate("Action");
            var tag = "Tag";
            var priority = 10;
            var acceptedArgs = 100;
            var arg = "_arg";

            hook.AddFilter(tag, callback, priority, acceptedArgs);
            await hook.DoAction(new object[] { arg });

            Assert.Equal(1, a.Events[0].Args.Count());
        }

        [Fact]
        public async void TestActionDoesntChangeValue()
        {
            var hook = new WpHook();
            var a = new Mocks.MockAction(0, hook);
            var output = new StringBuilder();
            var a1 = a.Generate("_filter_do_action_doesnt_change_value1",async o =>
            {
                output.Append($"{o}1");
                return "x1";
            });
            Func<IEnumerable<object>, Task<object>> a2 = null;
            a2 = a.Generate("_filter_do_action_doesnt_change_value2", async o =>
            {
                hook.RemoveFilter("do_action_doesnt_change_value", a2, 10);

                output.Append("-");
                await hook.DoAction(new object[] {"b"});
                output.Append("-");

                hook.AddFilter("do_action_doesnt_change_value", a2, 10, 1);

                output.Append($"{o}2");

                return "x2";
            });
            var a3 = a.Generate("_filter_do_action_doesnt_change_value3", async o =>
            {
                output.Append($"{o}3");
                return "x3";
            });
            var tag = "do_action_doesnt_change_value";
            var priority = 10;
            var acceptedArgs = 0;
            var arg = "_arg";

            hook.AddFilter(tag, a1, 10, 1);
            hook.AddFilter(tag, a2, 10, 1);
            hook.AddFilter(tag, a3, 11, 1);

            await hook.DoAction(new object[] {"a"});

            Assert.Equal("a1-b1b3-a2a3", output.ToString());
        }
    }
}
