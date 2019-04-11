using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Helper
{
    class FileOperation
    {
        public static string DataFilePath { get; set; }
        public static string RuleFilePath { get; set; }
        public static DataTable ReadDataFile()
        {
            while (true)
            {
                Console.WriteLine("Enter Name of Data File:");
                string path = Console.ReadLine();

                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), path)))
                {
                    DataFilePath = path;
                    return ReadDataFile(path);
                }
                else
                {
                    Console.WriteLine("Incorrect File Name or File Not Exists");
                }
            }
        }


        public static DataTable ReadDataFile(string path)
        {
            DataTable data = new DataTable();
            if (File.Exists(path))
            {
                int decision = 0;
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                            switch (line[0])
                            {
                                case '<':
                                    decision = GetDecisionIndex(line);
                                    break;
                                case '!':
                                    break;
                                case '[':
                                    ParseHeaders(line, data, decision);
                                    break;
                                default:
                                    ParseLine(line, data, decision);
                                    break;
                            }
                    }
                }

            }
            else
            {
                throw new Exception("Data File Missing - " + Path.GetFileName(path));
            }
            return data;
        }

        private static int GetDecisionIndex(string line)
        {
            var dataFormat = line.Trim().Replace('<', ' ').Replace('>', ' ').Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();

            return dataFormat.FindIndex(t => t.ToLower() == "d");
        }

        private static void ParseHeaders(string line, DataTable data, int decisionIndex)
        {
            var colHeaders = line.Trim().Replace('[', ' ').Replace(']', ' ').Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();
            colHeaders.ForEach(t => data.Columns.Add(new DataColumn(t, typeof(string))));

            //if (decisionIndex != null)
            data.Columns[decisionIndex].SetOrdinal(data.Columns.Count - 1);
            data.Columns.Add(new DataColumn("ID", typeof(string)));
        }
        private static void ParseLine(string line, DataTable data, int decisionIndex)
        {
            if(!string.IsNullOrWhiteSpace(line))
            {
                var values = line.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();

                var item = values[decisionIndex];

                values.RemoveAt(decisionIndex);

                values.Add(item);
                values.Add(data.Rows.Count + 1 + "");

                if (values.Count == data.Columns.Count)
                {
                    data.Rows.Add(values.ToArray());
                }
                else
                    throw new Exception("Column and Data count Mismatch");
            }

        }
    }
}
