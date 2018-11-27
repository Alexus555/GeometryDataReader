// C#

using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using GeometryDataReader.Support;
using System.Linq;

class DataReader
{
    static string path = "";

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.GetEncoding(1251);

        Boolean autoRun = args.Length > 0 && args[0].ToLower() == "-auto";

        path = ConfigurationManager.AppSettings.Get("PathToSaveResult");
        
        if(String.IsNullOrEmpty(path))
            path = AppDomain.CurrentDomain.BaseDirectory;

        Console.WriteLine("Path for saving results: {0}", path);

        Queries queries = GetQueries();

        string constr = ConfigurationManager.ConnectionStrings["constr"].ToString();

        using (OracleConnection conn = new OracleConnection(constr))
        {
            conn.ConnectionString = constr;
            try
            {
                Console.WriteLine("Open connection...");

                conn.Open();

                foreach (Query query in queries)
                {
                    DoAction(conn, query);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        if (!autoRun)
        {
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }



    public static Queries GetQueries()
    {
        var config = QueriesForActionConfig.GetConfig();
        return config.Queries;
    }

    private static void DoAction(OracleConnection conn, Query query)
    {
        if (String.IsNullOrWhiteSpace(query.Body))
            return;

        Console.WriteLine("Do query '{0}'...", query.Name);

        string fileName = Path.Combine(path, query.Name + ".csv");

        string queryBody = 
            ConstructQueryBody(query, fileName);

        RunCommand(conn, query, fileName, queryBody);
    }

    private static string ConstructQueryBody(Query query, string fileName)
    {
        int startIndex = 0;

        string queryBody = query.Body;

        if (!String.IsNullOrEmpty(query.Where))
            queryBody = String.Concat(queryBody, " WHERE ", query.Where);

        if (query.Action.ToLower() == "add")
        {
            startIndex = GetStartIndex(fileName);
            if (startIndex == 0 && !String.IsNullOrEmpty(query.AdditionalCondition))
                queryBody = String.Concat(queryBody, query.AdditionalCondition);
        }

        if (!String.IsNullOrEmpty(query.OrderBy))
            queryBody = String.Concat(queryBody, " ORDER BY ", query.OrderBy);

        queryBody = queryBody.Replace("indexField", query.IndexField);
        queryBody = queryBody.Replace("startIndex", startIndex.ToString());

        return queryBody;
    }

    private static void RunCommand(OracleConnection conn, Query query, string fileName, string queryBody)
    {
        Console.WriteLine("File {0}", fileName);

        Console.WriteLine("Command text = {0}", queryBody);
        OracleCommand cmd = new OracleCommand();
        cmd.Connection = (OracleConnection)conn;

        cmd.CommandText = queryBody;
        Console.WriteLine("Execute reader...");

        OracleDataReader reader = cmd.ExecuteReader();

        StringBuilder sb = new StringBuilder();

        string indexField = query.IndexField;

        int fieldCount = reader.FieldCount - 1;

        int indexFieldID = -1;
        for (int i = 0; i <= fieldCount; i++)
        {
            if (reader.GetName(i) == indexField)
            {
                indexFieldID = i;
                break;
            }
        };

        if ((query.Action.ToLower() == "add" && !File.Exists(fileName)) || query.Action.ToLower() == "replace")
        {
            if (indexFieldID >= 0)
                sb.Append(String.Format("{0};", reader.GetName(indexFieldID)));

            for (int i = 0; i <= fieldCount; i++)
            {
                if (i == indexFieldID)
                    continue;

                sb.Append(String.Format("{0};", reader.GetName(i)));
            };
            sb.AppendLine();
        }

        //Console.WriteLine("Retrieved {0} records", reader.RecordsAffected);
        
        while (reader.Read())
        {
            if (indexFieldID >= 0)
                sb.Append(String.Format("{0};", reader[indexFieldID]));

            for (int i = 0; i <= fieldCount; i++)
            {
                if (i == indexFieldID)
                    continue;
                sb.Append(String.Format("{0};", reader[i]));
            };
            sb.AppendLine();

        };

        if (query.Action.ToLower() == "replace")
        {
            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
            Console.WriteLine("File {0} was replaced", fileName);
        }
        else
        {
            File.AppendAllText(fileName, sb.ToString());
            Console.WriteLine("File {0} was added", fileName, Encoding.UTF8);
        }

        reader.Dispose();
        cmd.Dispose();

        Console.WriteLine("Success!");
        Console.WriteLine("********************************");
    }

    private static int GetStartIndex(string fileName)
    {
        int index = 0;
        try
        {
            if (File.Exists(fileName))
            {
                string lastLine = File.ReadLines(fileName).LastOrDefault();
                int.TryParse(lastLine.Split(';')[0], out index);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        return index;
    }
}


