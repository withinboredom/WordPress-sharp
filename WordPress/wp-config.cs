using System;
using System.Collections.Generic;
using System.Text;

namespace WordPress
{
    class WpConfig
    {
        public WpConfig(Compat compat)
        {
            compat.Define("WP_CACHE", true);
            compat.Define("WP_DEBUG", true);
            compat.Define("WP_DEBUG_DEISPLAY", true);
            compat.Define("WP_DEBUG_LOG", true);
            compat.Define("DB_NAME", "wordpress");
            compat.Define("DB_USER", "root");
            compat.Define("DB_PASSWORD", "password");
            compat.Define("DB_HOST", "localhost");
            compat.Define("DB_CHARSET", "utf8");
            compat.Define("DB_COLLATE", "");
            compat.Define("AUTH_KEY", "bb6R25@DG|=OAhBu{Jl-M90JT,6XyF6`;KBL,^O0T`8tmN;#mLOmgqZ[s}KU$b^-");
            compat.Define("SECURE_AUTH_KEY", "/^c3-]^XG~*|~4+6}``p -g%cb0y8jCuHmfIkycZbZTADFLw-INVomD<}qnHDy}R");
            compat.Define("LOGGED_IN_KEY", " mS[yEMYW:;:St?:6V~z{okun3DYJWY+uKJN%|:+6.}+Fv/@;f;klY;kO-Wd+|Ki");
            compat.Define("NONCE_KEY", "/MQEi2tl2kcPz<rCi:-1R+K6e0VH[Nu^,|Sg~1QA9{a956CC#B(ZkZK=/RBn!IcR");
            compat.Define("AUTH_SALT", "n]Y#!%Bql..~c-w?xihxy+!~hZI^)fm;|I~ew7{ncR7-WK07&aUD+`QV 7+OsA +");
            compat.Define("SECURE_AUTH_SALT", "<.rg)s|bcfwEy4i2Vk/B}tO%eTsL$%NEO@L+:Yq$=6lx]G%$|am#EHZ+M|]j{4&8");
            compat.Define("LOGGED_IN_SALT", " ?L8Xg-Ijv$O9;:cUeov8-O_)Vxij_+(zXo.|g+{^[sQknh0dN?E<C;E@86RI.22");
            compat.Define("NONCE_SALT", "g-rK@k-j-)xlUf_-C/&oT-.Cr+acGST,0tEt|c]}sy1&|W)*DSsw#az+/|rvKG~F");
            compat._GLOBALS["table_prefix"] = "wp_";

            new WpSettings(compat);
        }
    }
}
