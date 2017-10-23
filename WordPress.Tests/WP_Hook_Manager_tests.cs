using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPress.Includes;
using Xunit;

namespace WordPress.Tests
{
    public class WP_Hook_Manager_Tests
    {
        [Fact]
        public async void TestAddRemoveFilter()
        {
            var manager = new WpHookManager();
            manager.AddFilter("testAddFilter", IdentityCallback);
            Assert.True(manager.HasFilter("testAddFilter"));
            Assert.True(manager.HasFilter("testAddFilter", null).HasValue);
            Assert.True(manager.HasFilter("testAddFilter", null) > 0);
            Assert.True(manager.HasFilter("testAddFilter", IdentityCallback) > 0);
            Assert.False(manager.HasFilter("nothere"));
            Assert.False(manager.HasFilter("nothere", null).HasValue);

            manager.RemoveFilter("testAddFilter", IdentityCallback);
            Assert.False(manager.HasFilter("testAddFilter", null).HasValue);
            Assert.False(manager.HasFilter("testAddFilter", null) > 0);
            Assert.False(manager.HasFilter("testAddFilter", IdentityCallback) > 0);

            manager.RemoveFilter("testAddFilter", IdentityCallback);
        }

        [Fact]
        public async void TestFiltersAndActionsIntegration()
        {
            var didAction = false;
            var manager = new WpHookManager();
            Func<IEnumerable<object>, Task<object>> action = async (act) =>
            {
                Assert.Equal("testAction", manager.CurrentAction());
                didAction = true;
                return null;
            };

            Func<IEnumerable<object>, Task<object>> filter = async (act) =>
            {
                var ar = (List<object>)act;
                await manager.DoAction("testAction");
                return (int)ar[0] + 1;
            };

            manager.AddAction("testAction", action);
            manager.AddFilter("SumTest", filter);
            var result = await manager.ApplyFilters("SumTest", 1);
            Assert.Equal(2, result);
        }

        private async Task<object> IdentityCallback(IEnumerable<object> array)
        {
            return array.FirstOrDefault();
        }
    }
}
