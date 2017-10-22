using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;

namespace WordPress
{
    class Compat
    {
        private Dictionary<string, object> Defines = new Dictionary<string, object>();

        public void Define<T>(string var, T val)
        {
            Defines[var] = val;
        }

        public Dictionary<string, object> _GLOBALS = new Dictionary<string, object>();
        public Dictionary<string, object> _SERVER = new Dictionary<string, object>();

        public WpHookManager Hook = new WpHookManager();
    }
}
