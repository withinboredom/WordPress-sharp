using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace WordPress.Includes
{
    public class WpHookManager
    {
        public Dictionary<string, WpHook> Hooks;
        public Dictionary<string, int> Actions;
        public Stack<string> CurrentFilterStack;

        public WpHookManager()
        {
            Hooks = new Dictionary<string, WpHook> { ["all"] = new WpHook() };
            CurrentFilterStack = new Stack<string>();
            Actions = new Dictionary<string, int>();
        }

        public bool AddFilter(string tag, Func<IEnumerable, Task<object>> callback, int priority = 10,
            int acceptedArgs = 1)
        {
            if (!Hooks.ContainsKey(tag))
            {
                Hooks[tag] = new WpHook();
            }

            Hooks[tag].AddFilter(tag, callback, priority, acceptedArgs);
            return true;
        }

        public int? HasFilter(string tag, Func<IEnumerable, Task<object>> callback)
        {
            if (callback != null) return !Hooks.ContainsKey(tag) ? null : Hooks[tag].HasFilter(tag, callback);

            if (HasFilter(tag))
            {
                return 1;
            }

            return null;
        }

        public bool HasFilter(string tag)
        {
            return Hooks.ContainsKey(tag) && Hooks[tag].HasFilter(tag);
        }

        private async Task CallAllHook(IEnumerable args)
        {
            await Hooks["all"].DoAllHook(args);
        }

        private async Task<object> ReallyApplyFilters(string tag, object value = null, object[] args = null)
        {
            if (args == null)
            {
                args = new object[0];
            }

            CurrentFilterStack.Push(tag);

            await CallAllHook(args.Prepend(value));

            if (!HasFilter(tag))
            {
                CurrentFilterStack.Pop();
                return value;
            }

            var result = await Hooks[tag].ApplyFilters(value, args);
            CurrentFilterStack.Pop();

            return result;
        }

        public async Task<T> ApplyFilters<T>(string tag, object value = null, params object[] args) where T : class
        {
            return await ReallyApplyFilters(tag, value, args) as T;
        }

        public Task<object> ApplyFilters(string tag, object value = null, params object[] args)
        {
            return ReallyApplyFilters(tag, value, args);
        }

        public bool RemoveFilter(string tag, Func<IEnumerable, Task<object>> callback, int priority = 10)
        {
            bool result;
            if (!Hooks.ContainsKey(tag)) return false;

            result = Hooks[tag].RemoveFilter(tag, callback, priority);
            if (!Hooks[tag].HasFilters())
            {
                Hooks.Remove(tag);
            }

            return result;
        }

        public bool RemoveAllFilters(string tag, int? priority = null)
        {
            if (Hooks.ContainsKey(tag))
            {
                if (priority.HasValue)
                {
                    Hooks[tag].RemoveAllFilters(priority.Value);
                }
                else
                {
                    Hooks[tag].RemoveAllFilters();
                }
                if (!Hooks[tag].HasFilters())
                {
                    Hooks.Remove(tag);
                }
            }

            return true;
        }

        public string CurrentFilter()
        {
            return CurrentFilterStack.Peek();
        }

        public string CurrentAction()
        {
            return CurrentFilter();
        }

        public bool DoingFilter(string filter = null)
        {
            if (filter == null)
            {
                return CurrentFilterStack.Count > 0;
            }

            return CurrentFilterStack.Contains(filter);
        }

        public bool DoingAction(string action = null)
        {
            return DoingFilter(action);
        }

        public bool AddAction(string tag, Func<IEnumerable, Task<object>> callback, int priority = 10,
            int acceptedArgs = 1)
        {
            return AddFilter(tag, callback, priority, acceptedArgs);
        }

        public async Task DoAction(string tag, params object[] args)
        {
            if (Actions.ContainsKey(tag))
            {
                ++Actions[tag];
            }
            else
            {
                Actions[tag] = 1;
            }

            CurrentFilterStack.Push(tag);

            await CallAllHook(args);

            await Hooks[tag].DoAction(args);

            CurrentFilterStack.Pop();
        }

        public int? DidAction(string tag)
        {
            if (Actions.ContainsKey(tag))
            {
                return Actions[tag];
            }

            return null;
        }

        public int? HasAction(string tag, Func<IEnumerable, Task<object>> callback)
        {
            return HasFilter(tag, callback);
        }

        public bool HasAction(string tag)
        {
            return HasFilter(tag);
        }

        public bool RemoveAction(string tag, Func<IEnumerable, Task<object>> callback, int priority = 10)
        {
            return RemoveFilter(tag, callback, priority);
        }

        public bool RemoveAllActions(string tag, int? priority)
        {
            return RemoveAllFilters(tag, priority);
        }
    }
}
