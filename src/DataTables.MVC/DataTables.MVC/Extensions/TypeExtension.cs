
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Data;
using DataTables.MVC.Utilities;

namespace DataTables.MVC.Extensions
{
    public static class TypeExtension
    {
        #region String Extension
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string Enclose(this string value, string start, string end)
        {
            return string.Format("{0}{1}{2}", start, value, end);
        }

        public static string AppendHtmlLineBreak(this string value, string lineBreak = "<br/>", string replace = "")
        {
            if (string.IsNullOrEmpty(value))
                return value;
            string result = value.Replace(Environment.NewLine, lineBreak).Replace("\n", lineBreak);
            if (!string.IsNullOrEmpty(replace))
                result = result.Replace(replace, lineBreak);
            return result;
        }

        public static string Append(this string value, string appendValue, string separator)
        {
            string result = value;
            if (!String.IsNullOrEmpty(appendValue))
            {
                result = string.Format("{0} {1} {2}", value, separator, appendValue);
            }
            return result;
        }

        public static List<string> CsvToList(this string value, string separator)
        {
            return value.Split(separator.ToCharArray()).ToList();
        }

        public static List<string> MultiLineToList(this string multilineString, string separator = "\r\n")
        {
            if (string.IsNullOrEmpty(multilineString))
                return new List<string>();
            return multilineString.Split(separator.ToCharArray())
                .Select(x => x.SanitizeEndLine())
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }


        public static string SanitizeEndLine(this string value)
        {
            return value.Replace(Environment.NewLine, "")
                .Replace("\r", "")
                .Replace("\n", "");
        }

        public static bool Match(this string str, string regexStr)
        {
            Regex regex = new Regex(regexStr);
            Match match = regex.Match(str);
            return match.Success;
        }

        public static string RemoveLast(this string str, int length = 1)
        {
            if (str.Length > 0)
            {
                str.Remove(str.Length - length, length);
            }
            return str;
        }

        public static string ToHtmlAscii(this string str)
        {
            string converted = string.Empty;
            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(str);
            asciiBytes.ToList().ForEach(x => converted += string.Format("&#{0};", (int)x));
            return converted;
        }

        public static string Truncate(this string s, int length, bool atWord, bool addEllipsis)
        {
            // Return if the string is less than or equal to the truncation length
            if (s == null || s.Length <= length)
                return s;

            // Do a simple tuncation at the desired length
            string s2 = s.Substring(0, length);

            // Truncate the string at the word
            if (atWord)
            {
                // List of characters that denote the start or a new word (add to or remove more as necessary)
                List<char> alternativeCutOffs = new List<char>() { ' ', ',', '.', '?', '/', ':', ';', '\'', '\"', '\'', '-' };

                // Get the index of the last space in the truncated string
                int lastSpace = s2.LastIndexOf(' ');

                // If the last space index isn't -1 and also the next character in the original
                // string isn't contained in the alternativeCutOffs List (which means the previous
                // truncation actually truncated at the end of a word),then shorten string to the last space
                if (lastSpace != -1 && (s.Length >= length + 1 && !alternativeCutOffs.Contains(s.ToCharArray()[length])))
                    s2 = s2.Remove(lastSpace);
            }

            // Add Ellipsis if desired
            if (addEllipsis)
                s2 += "...";

            return s2;
        }
        #endregion

        #region StringBuilder Extension

        public static void RemoveLastCharacter(this StringBuilder sb, int length = 1)
        {
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - length, length);
            }
        }

        #endregion

        #region List Extension

        public static void AddToFront<T>(this List<T> list, T item)
        {
            // omits validation, etc.
            list.Insert(0, item);
        }

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        public static string ConcatAsString(this List<string> list, string delimit, bool includeEmpty = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String str in list)
            {
                if (!includeEmpty && string.IsNullOrEmpty(str))
                    continue;
                sb.AppendFormat("{0}{1}", str, delimit);
            }
            sb.RemoveLastCharacter(delimit.Length);
            return sb.ToString();

        }
        

        public static DataTable ToDataTable<T>(this List<T> list, string tableName = null
            , List<Expression<Func<T, object>>> primaryKeys = null
            , params Expression<Func<T, object>>[] properties)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(T).Name;

            DataTable dataTable = new DataTable(tableName);

            //Get all the properties
            PropertyInfo[] Props = properties == null || properties.Length == 0 ? typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                : properties.ToList().Select(x => ObjectUtility.GetProperty(x)).ToArray();

            if(primaryKeys == null)
                primaryKeys = new List<Expression<Func<T,object>>>();
            List<PropertyInfo> pkPropList = primaryKeys.Select(x => ObjectUtility.GetProperty(x)).ToList();

            List<DataColumn> pkCols = new List<DataColumn>();
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
                if (pkPropList.Exists(x => x.Name == prop.Name))
                    pkCols.Add(dataTable.Columns[prop.Name]);
            }
            if (pkCols.Count > 0)
                dataTable.PrimaryKey = pkCols.ToArray();

            foreach (T item in list)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                try
                {
                    dataTable.Rows.Add(values);
                }
                catch (ConstraintException cex)
                {
                    Console.WriteLine(cex.Message);
                }
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #endregion


        public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
        
        public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source
            , string propertyName, bool isDescending)
        {
            //if (isDescending)
            //    return source.OrderByDescending(x => typeof(TSource).GetProperty(propertyName).GetValue(x));
            //else
            //    return source.OrderBy(x => typeof(TSource).GetProperty(propertyName).GetValue(x));
            if (isDescending)
                return source.OrderByDescending(x => GetOrderValue(propertyName, x));
            else
                return source.OrderBy(x => GetOrderValue(propertyName, x));

        }

        private static object GetOrderValue<TSource>(string propertyName, TSource obj)
        {
            PropertyInfo prop = typeof(TSource).GetProperty(propertyName);
            object val = prop.GetValue(obj);
            object returnVal = val;
            return returnVal;
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, bool isDescending)
        {
            if (isDescending)
                return source.OrderByDescending(keySelector);
            else
                return source.OrderBy(keySelector);
        }


        public static T Clone<T>(this T source)
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            ObjectUtility.CopyProperties(source, result);
            return result;
           
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }
        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        } 

    }

    public static class DateTimeExtensions
    {
        static string timeZoneId = ConfigurationManager.AppSettings["TimeZoneId"] ?? "W. Europe Standard Time";

        public static DateTime ToLocalTime(this DateTime dt)
        {
            // dt.DateTimeKind should be Utc!
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dt, DateTimeKind.Utc), tzi);
        }

        public static DateTime ToUtcTime(this DateTime dt)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
        }

        public static DateTime RoundDown(this DateTime dateTime, int minutes)
        {
            return new DateTime(dateTime.Year, dateTime.Month,
                 dateTime.Day, dateTime.Hour, (dateTime.Minute / minutes) * minutes, 0);
        }
    }

}
