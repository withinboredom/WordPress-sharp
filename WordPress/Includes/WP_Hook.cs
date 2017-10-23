using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPress.Includes
{
    internal sealed class HookIteration : IEnumerator<int>
    {
        private List<int>.Enumerator _enumerator;
        public bool IsIterating { get; private set; }
        public List<int> Iterations { get; }

        public HookIteration(IEnumerable<int> keys)
        {
            Iterations = new List<int>(keys);
            _enumerator = Iterations.GetEnumerator();
            IsIterating = false;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return IsIterating = _enumerator.MoveNext();
        }

        public void Reset()
        {
            IsIterating = false;
            _enumerator.Dispose();
            _enumerator = Iterations.GetEnumerator();
        }

        public int Current => _enumerator.Current;
        object IEnumerator.Current => Current;
    }

    internal sealed class HookCallback :
        IEquatable<HookCallback>,
        IEquatable<Func<IEnumerable<object>, Task<object>>>
    {
        public Func<IEnumerable<object>, Task<object>> Callback { get; }
        public int AcceptedArgs { get; }

        public HookCallback(Func<IEnumerable<object>, Task<object>> callback, int acceptedArgs)
        {
            Callback = callback;
            AcceptedArgs = acceptedArgs;
        }

        public static implicit operator HookCallback(Func<IEnumerable<object>, Task<object>> cb)
        {
            return new HookCallback(cb, 0);
        }

        public override int GetHashCode()
        {
            return Callback != null ? Callback.GetHashCode() : 0;
        }

        public bool Equals(HookCallback other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Callback, other.Callback);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is HookCallback && Equals((HookCallback)obj);
        }

        public bool Equals(Func<IEnumerable<object>, Task<object>> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Callback, other);
        }
    }

    public sealed class WpHook
    {
        internal SortedDictionary<int, List<HookCallback>> Callbacks = new SortedDictionary<int, List<HookCallback>>();
        internal Dictionary<int, HookIteration> Iterations = new Dictionary<int, HookIteration>();
        internal Dictionary<int, int> CurrentPriorityDictionary = new Dictionary<int, int>();
        private int _nestingLevel = 0;
        private bool _isAction = false;

        public void AddFilter(string tag, Func<IEnumerable<object>, Task<object>> callback, int priority, int acceptedArgs)
        {
            if (!Callbacks.ContainsKey(priority))
                Callbacks[priority] = new List<HookCallback>();

            Callbacks[priority].Add(new HookCallback(callback, acceptedArgs));
        }

        public bool RemoveFilter(string tag, Func<IEnumerable<object>, Task<object>> callback, int priority)
        {
            if (!Callbacks.ContainsKey(priority))
                return false;

            return Callbacks[priority].Contains(callback) && Callbacks[priority].Remove(callback);
        }

        public int? HasFilter(string tag, Func<IEnumerable<object>, Task<object>> callback)
        {
            var found = from c in Callbacks
                        where c.Value.Contains(callback)
                        select c.Key;

            return !found.Any() ? new int?() : found.FirstOrDefault();
        }

        public bool HasFilter(string tag = "")
        {
            return HasFilters();
        }

        public bool HasFilters()
        {
            return Callbacks.Any(c => c.Value.Count > 0);
        }

        public void RemoveAllFilters(int priority)
        {
            Callbacks[priority] = new List<HookCallback>();
        }

        public void RemoveAllFilters()
        {
            Callbacks = new SortedDictionary<int, List<HookCallback>>();
        }

        public Task<object> ApplyFilters(object value)
        {
            return ApplyFilters(value, new object[0]);
        }

        public async Task<object> ApplyFilters(object value, IEnumerable<object> args)
        {
            if (!Callbacks.Any(e => e.Value.Count > 0))
                return value;

            var nest = _nestingLevel++;
            var iterator = Iterations[nest] = new HookIteration(Callbacks.Keys);
            var numArgs = args.Count();

            while (iterator.MoveNext())
            {
                var priority = CurrentPriorityDictionary[nest] = iterator.Current;

                if (!Callbacks.ContainsKey(priority)) continue;

                var callbacks = Callbacks[priority].ToArray();

                foreach (var callback in callbacks)
                {
                    var expectedArgs = callback.AcceptedArgs;
                    var newArgs = new List<object>(expectedArgs);
                    if (!_isAction)
                    {
                        if (expectedArgs > 0)
                        {
                            newArgs.Add(value);
                        }
                        if (expectedArgs > 1)
                        {
                            newArgs.AddRange(args.Take(expectedArgs - 1));
                        }
                    }
                    else
                    {
                        if (expectedArgs > 0)
                        {
                            newArgs.AddRange(args.Take(expectedArgs));
                        }
                    }

                    value = await callback.Callback(newArgs);
                }
            }

            iterator.Dispose();
            Iterations.Remove(nest);
            CurrentPriorityDictionary.Remove(nest);
            _nestingLevel--;

            return value;
        }

        public async Task DoAction(IEnumerable<object> args)
        {
            _isAction = true;
            await ApplyFilters("", args);

            if (_nestingLevel == 0)
                _isAction = false;
        }

        public Task DoAction()
        {
            return DoAction(new object[0]);
        }

        public async Task DoAllHook(IEnumerable<object> args)
        {
            var nestingLevel = _nestingLevel++;
            var iterator = Iterations[nestingLevel] = new HookIteration(Callbacks.Keys);

            while (iterator.MoveNext())
            {
                var priority = iterator.Current;
                foreach (var the_ in Callbacks[priority])
                    await the_.Callback(args);
            }

            iterator.Dispose();
            Iterations.Remove(nestingLevel);
            _nestingLevel--;
        }

        public int? CurrentPriority()
        {
            if (!Iterations.Any())
                return null;

            return CurrentPriorityDictionary[_nestingLevel - 1];
        }
    }
}
