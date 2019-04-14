using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Helper
{
    public static class Extensions
    {
        public static IEnumerable<TSource> IntersectAll<TSource>(
        this IEnumerable<IEnumerable<TSource>> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break;

                var set = new HashSet<TSource>(enumerator.Current);
                while (enumerator.MoveNext())
                    set.IntersectWith(enumerator.Current);
                foreach (var item in set)
                    yield return item;
            }
        }

        public static List<int> FindRecords(this EnumerableRowCollection<DataRow> dataEnumer, DataColumn column, string value)
        {
            var records = from rows in dataEnumer
                             where rows.Field<string>(column) == value
                             select rows.Field<string>("ID");

            return records.Select(int.Parse).ToList();
        }

        public static List<TSource> FindDistinctValues<TSource>(this DataTable dataSet,int index)
        {
            return dataSet.AsEnumerable().Select(r => r.Field<TSource>(index)).Distinct().ToList();
        }

        public static List<TSource> FindDistinctValues<TSource>(this DataTable dataSet, string columnName)
        {
            return dataSet.AsEnumerable().Select(r => r.Field<TSource>(columnName)).Distinct().ToList();
        }

        public static List<TSource> FindDistinctValues<TSource>(this EnumerableRowCollection<DataRow> dataEnumer, DataColumn column)
        {
            return dataEnumer.Select(r => r.Field<TSource>(column)).Distinct().ToList();
        }

        public static bool CheckSubset(this List<int> SuperSet, List<int> SubSet)
        {
            return !SubSet.Except(SuperSet).Any();
        }
        public static void WriteToCsvFile(this DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            string fileName = $"Result{DateTime.Now.ToFileTime()}.csv";
            System.IO.File.WriteAllText(Path.Combine(filePath, fileName), fileContent.ToString());
        }

    }
}
