using System;
using System.Collections.Generic;
using System.Text;

namespace WordPress
{
    class WpLoad
    {
        public WpLoad(Compat compat)
        {
            new WpConfig(compat);
        }
    }
}
