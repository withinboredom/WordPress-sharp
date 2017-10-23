using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests.Hooks
{
    public class AddFilter
    {
        [Fact]
        public async void TestAddFilterWithFunction()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;
            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);

            Assert.True(hook.HasFilters());
            Assert.Equal(10, hook.HasFilter(tag, Mocks.ReturnTrue));
            Assert.True(hook.HasFilter(""));
        }

        [Fact]
        public async void TestAddTwoFilterWithSamePriority()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            hook.AddFilter(tag, Mocks.ReturnFalse, priority, acceptedArgs);

            Assert.Equal(10, hook.HasFilter(tag, Mocks.ReturnTrue));
            Assert.Equal(10, hook.HasFilter(tag, Mocks.ReturnFalse));
        }

        [Fact]
        public async void TestAddTwoFilterWithDifferentPriority()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            Assert.Equal(priority, hook.HasFilter(tag, Mocks.ReturnTrue));

            hook.AddFilter(tag, Mocks.ReturnFalse, priority + 1, acceptedArgs);
            Assert.Equal(priority + 1, hook.HasFilter(tag, Mocks.ReturnFalse));
        }

        [Fact]
        public async void TestReaddFilter()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            Assert.Equal(priority, hook.HasFilter(tag, Mocks.ReturnTrue));

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            Assert.Equal(priority, hook.HasFilter(tag, Mocks.ReturnTrue));
        }

        [Fact]
        public async void TestReaddFilterWithDifferentPriority()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            Assert.Equal(priority, hook.HasFilter(tag, Mocks.ReturnTrue));

            hook.AddFilter(tag, Mocks.ReturnTrue, priority + 1, acceptedArgs);
            Assert.Equal(priority, hook.HasFilter(tag, Mocks.ReturnTrue));
        }

        [Fact]
        public async void TestSortAfterAddFilter()
        {
            var hook = new WpHook();
            var tag = "test";
            var priority = 10;
            var acceptedArgs = 1;

            hook.AddFilter(tag, Mocks.ReturnTrue, priority, acceptedArgs);
            hook.AddFilter(tag, Mocks.ReturnTrue, priority - 5, acceptedArgs);
            hook.AddFilter(tag, Mocks.ReturnTrue, priority - 3, acceptedArgs);

            Assert.Equal(priority - 5, hook.HasFilter(tag, Mocks.ReturnTrue));
        }

        [Fact]
        public async void TestRemoveAndAdd()
        {
            var hook = new WpHook();
            var tag = "remove_and_add";

            hook.AddFilter(tag, Mocks.ReturnEmptyString, 10, 0 );
            hook.AddFilter(tag, Mocks.RemoveAndAdd2, 11, 1);
            hook.AddFilter(tag, Mocks.AddString(4), 12, 1);

            var value = await hook.ApplyFilters("", new object[] { hook });

            Assert.Equal("24", value);
        }

        [Fact]
        public async void TestRemoveAndRecurseAndAdd()
        {
            var hook = new WpHook();
            var tag = "remove_and_add";

            hook.AddFilter(tag, Mocks.ReturnEmptyString, 10, 0);
            hook.AddFilter(tag, Mocks.AddString(1), 11, 2);
            hook.AddFilter(tag, Mocks.RemoveAndRecurseAndAdd2, 11, 2);
            hook.AddFilter(tag, Mocks.AddString(3), 11, 2);

            hook.AddFilter(tag, Mocks.AddString(4), 12, 2);

            var value  = await hook.ApplyFilters("", new object[] {hook});

            Assert.Equal("1-134-234", value);
        }
    }
}
