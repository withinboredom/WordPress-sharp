using System;
using System.Collections.Generic;
using System.Text;

namespace WordPress
{
    class Index
    {
        public static void Main()
        {
            var compat = new Compat();
            compat.Define("WP_USE_THEMES", true);

            new WpBlogHeader(compat);
        }
    }
}
