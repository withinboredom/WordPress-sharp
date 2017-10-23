using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static Func<IEnumerable<object>, Task<object>> AddString(object added)
        {
            return async (i) =>
            {
                var str = i.First().ToString();
                return str + added.ToString();
            };
        }

        public static async Task<object> RemoveAndAdd2(IEnumerable<object> args)
        {
            var str = args.First().ToString();
            var hook = args.Last() as WpHook;
            
            hook.RemoveFilter("remove_and_add", Mocks.RemoveAndAdd2, 11);
            hook.AddFilter("remove_and_add", Mocks.RemoveAndAdd2, 11, 2);

            return str + "2";
        }

        public static async Task<object> RemoveAndRecurseAndAdd2(IEnumerable<object> args)
        {
            var str = args.First().ToString();
            var hook = args.Last() as WpHook;

            hook.RemoveFilter("remove_and_add", Mocks.RemoveAndRecurseAndAdd2, 11);

            str += "-" + (await hook.ApplyFilters("", new object[0])) + "-";

            hook.AddFilter("remove_and_add", Mocks.RemoveAndRecurseAndAdd2, 11, 2);

            return str + "2";
        }

        public class MockAction
        {
            public List<Event> Events;
            public int Debug;
            public WpHook Hook;
            public WpHookManager Hooks;

            public StringBuilder Output;

            public struct Event
            {
                public string Func;
                public string Tag;
                public IEnumerable<object> Args;
            }

            public MockAction(int debug, WpHook hook)
            {
                Reset();
                Debug = debug;
                Hook = hook;
            }

            public MockAction(int debug, WpHookManager hook)
            {
                Reset();
                Debug = debug;
                Hooks = hook;
            }

            public void Reset()
            {
                Events = new List<Event>();
            }

            public string CurrentFilter()
            {
                return Hooks != null ? Hooks.CurrentFilter() : "Manual Filter";
            }

            public Func<IEnumerable<object>, Task<object>> Generate(string func, Func<object, Task<object>> callback = null)
            {
                return async (args) =>
                {
                    Events.Add(new Event
                    {
                        Func = func,
                        Tag = CurrentFilter(),
                        Args = args
                    });

                    var value = args.Any() ? args.First() : null;

                    return callback != null ? await callback(value) : value;
                };
            }

            public Func<IEnumerable<object>, Task<object>> FilterAppend(string toAppend = "_append")
            {
                return Generate("FilterAppend", async o =>
                {
                    Output.Append(toAppend);
                    return o.ToString() + toAppend;
                });
            }

            public int GetCallCount()
            {
                return Events.Count;
            }

            public int GetCallCount(string tag)
            {
                if (tag == null) return GetCallCount();

                return (from e in Events
                        where e.Tag == tag
                        select e).Count();
            }

            public IEnumerable<string> GetTags()
            {
                return from e in Events
                    select e.Tag;
            }

            public IEnumerable<IEnumerable> GetArgs()
            {
                return from e in Events
                    select e.Args;
            }
        }
    }
}
