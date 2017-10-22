using System;
using System.Collections.Generic;
using System.Text;

namespace WordPress
{
    class WpBlogHeader
    {
        public WpBlogHeader(Compat compat)
        {
            compat._GLOBALS["wp_did_header"] = true;

            new WpLoad(compat);
        }
    }
}
