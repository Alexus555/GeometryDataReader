using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace GeometryDataReader.Support
{
    public class Queries : ConfigurationElementCollection
    {
        public Query this[int index]
        {
            get
            {
                return base.BaseGet(index) as Query;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new Query this[string responseString]
        {
            get { return (Query)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new Query();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((Query)element).Name;
        }
    }
    
}
