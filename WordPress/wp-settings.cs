using System;
using System.Collections.Generic;
using System.Text;
using WordPress.Includes;

namespace WordPress
{
    class WpSettings
    {
        public WpSettings(Compat compat)
        {
            new Load(compat);
        }
    }
}
