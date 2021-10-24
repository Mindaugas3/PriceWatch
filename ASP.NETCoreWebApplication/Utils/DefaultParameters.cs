using System;
using System.Collections.Generic;

namespace ASP.NETCoreWebApplication.Utils
{
    public class DefaultParameters
    {
        public Dictionary<string, dynamic> defaultMapping;

        public DefaultParameters(Dictionary<string, dynamic> defaultM)
        {
            this.defaultMapping = defaultM;
        }

        public void ReplaceKey(string key, dynamic value)
        {
            defaultMapping[key] = value;
        }

        public dynamic this[string key]
        {
            get { return defaultMapping[key]; }
        }
    }
}