using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace GeometryDataReader.Support
{
    public class Query : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }
        [ConfigurationProperty("body", IsRequired = true)]
        public string Body
        {
            get
            {
                return this["body"] as string;
            }
        }
        [ConfigurationProperty("indexField", IsRequired = false)]
        public string IndexField
        {
            get
            {
                return this["indexField"] as string;
            }
        }
        [ConfigurationProperty("additionalCondition", IsRequired = false)]
        public string AdditionalCondition
        {
            get
            {
                return this["additionalCondition"] as string;
            }
        }
        [ConfigurationProperty("Where", IsRequired = false)]
        public string Where
        {
            get
            {
                return this["Where"] as string;
            }
        }
        [ConfigurationProperty("OrderBy", IsRequired = false)]
        public string OrderBy
        {
            get
            {
                return this["OrderBy"] as string;
            }
        }
        [ConfigurationProperty("action", IsRequired = false)]
        public string Action
        {
            get
            {
                string action = this["action"] as string;
                if (String.IsNullOrWhiteSpace(action))
                    action = "replace";
                return action;
            }
        }
    }
}
