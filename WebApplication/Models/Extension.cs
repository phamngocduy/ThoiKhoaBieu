using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static class Extension
    {
        public static int ToInteger(this object @object)
        {
            return int.Parse(@object.ToString());
        }
    }
}