using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using GeometryDataReader;

namespace GeometryDataReader.Support
{
    public class QueriesForActionConfig : ConfigurationSection
    {
        public static QueriesForActionConfig GetConfig()
        {
            return (QueriesForActionConfig)System.Configuration.ConfigurationManager.GetSection("QueriesForAction") ?? new QueriesForActionConfig();
        }

        [System.Configuration.ConfigurationProperty("Queries")]
        [ConfigurationCollection(typeof(Queries), AddItemName = "Query")]
        public Queries Queries
        {
            get
            {
                object o = this["Queries"];
                return o as Queries;
            }
        }
    }
}
