using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordPress.Includes;

namespace WordPress.Tests
{
    static class Mocks
    {
        public static async Task<object> ReturnFalse(IEnumerable args)
        {
            return false;
        }

        public static async Task<object> ReturnTrue(IEnumerable args)
        {
            return true;
        }

        public static async Task<object> ReturnNull(IEnumerable args)
        {
            return null;
        }

        public static async Task<object> ReturnEmptyString(IEnumerable args)
        {
            return string.Empty;
        }

        public static Func<IEnumerable, Task<object>> AddString(object added)
        {
            return async (i) =>
            {
                var str = (Array)i;
                return (string)str.GetValue(0) + added.ToString();
            };
        }

        public static async Task<object> RemoveAndAdd2(IEnumerable args)
        {
            var str = ((Array)args).GetValue(0).ToString();
            var hook = ((Array)args).GetValue(1) as WpHook;

            hook.RemoveFilter("remove_and_add", Mocks.RemoveAndAdd2, 11);
            hook.AddFilter("remove_and_add", Mocks.RemoveAndAdd2, 11, 2);

            return str + "2";
        }

        public static async Task<object> RemoveAndRecurseAndAdd2(IEnumerable args)
        {
            var str = ((Array)args).GetValue(0).ToString();
            var hook = ((Array)args).GetValue(1) as WpHook;

            hook.RemoveFilter("remove_and_add", Mocks.RemoveAndRecurseAndAdd2, 11);

            str += "-" + (await hook.ApplyFilters("", new object[0])) + "-";

            hook.AddFilter("remove_and_add", Mocks.RemoveAndRecurseAndAdd2, 11, 2);

            return str + "2";
        }
    }
}
