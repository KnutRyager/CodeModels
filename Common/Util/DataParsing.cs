using Common.Typing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using static Common.Reflection.ReflectionUtil;

namespace Common.Util
{
    public static class DataParsing
    {
        public static List<string> GetColumns<T>() => typeof(T).BaseType == typeof(ValueType) ?
            typeof(T).GenericTypeArguments.Select(x => x.FullName!).ToList() :
            GetPropertyAttributes<T, DisplayNameAttribute>().Select(x => x.DisplayName).ToList();

        public static List<(T1, T2)> ParseTuple<T1, T2>(IEnumerable<IEnumerable<object>> valuesIn)
            => CollectionUtil.SelectRows(valuesIn).Select(x => ValueTuple.Create(ParseAs<T1>(x[0]), ParseAs<T2>(x[1]))).ToList();
        public static List<(T1, T2, T3)> ParseTuple<T1, T2, T3>(IEnumerable<IEnumerable<object>> valuesIn)
            => CollectionUtil.SelectRows(valuesIn).Select(x => ValueTuple.Create(ParseAs<T1>(x[0]), ParseAs<T2>(x[1]), ParseAs<T3>(x[2]))).ToList();
        public static List<(T1, T2, T3, T4)> ParseTuple<T1, T2, T3, T4>(IEnumerable<IEnumerable<object>> valuesIn)
            => CollectionUtil.SelectRows(valuesIn).Select(x => ValueTuple.Create(ParseAs<T1>(x[0]), ParseAs<T2>(x[1]), ParseAs<T3>(x[2]), ParseAs<T4>(x[3]))).ToList();
        public static List<(T1, T2, T3, T4, T5)> ParseTuple<T1, T2, T3, T4, T5>(IEnumerable<IEnumerable<object>> valuesIn)
            => CollectionUtil.SelectRows(valuesIn).Select(x => ValueTuple.Create(ParseAs<T1>(x[0]), ParseAs<T2>(x[1]), ParseAs<T3>(x[2]), ParseAs<T4>(x[3]), ParseAs<T5>(x[4]))).ToList();

        public static List<T> Parse<T>(IEnumerable<IEnumerable<object>> valuesIn, IEnumerable<string>? columnsIn = null)
        where T : new()
        {
            var values = valuesIn.Select(x => x.ToArray()).ToList();
            var columns = columnsIn?.ToList() ?? values[0].Select(x => (string)x).ToList();
            var results = new List<T>();
            var displayNames = GetPropertyAttributes<T, DisplayNameAttribute>().Select(x => x.DisplayName).ToList();
            //var columnHeader = values[0].Select(x => (string)x).Where(x => displayNames.Contains(x)).ToArray();
            var columnHeader = values[0].Select(x => (string)x).ToArray();
            for (var i = 0; i < columnHeader.Length; i++)
            {
                if (!displayNames.Contains(columnHeader[i])) throw new ArgumentException($"Couldn't find column '{columnHeader[i]}' in '{typeof(T).Name}' with properties '{string.Join(", ", displayNames)}'.");
            }
            var properties = columnHeader.Select(GetPropertyOfDisplayName<T>).ToList();
            for (var i = 1; i < values.Count; i++)
            {
                var row = values[i];
                var current = new T();
                for (var j = 0; j < columnHeader.Length; j++)
                {
                    var columnName = columnHeader[j];
                    var property = properties[j];
                    var stringValue = j >= row.Length ? "" : row[j];
                    var value = Parse(property.PropertyType, stringValue);
                    property.SetValue(current, value);
                }
                results.Add(current);
            }
            return results;
        }

        public static IEnumerable<IEnumerable<object>> DeParse<T>(IEnumerable<T>? entries, bool includeHeader)
        {
            var columns = GetColumns<T>();
            var entriesArray = entries?.ToArray() ?? Array.Empty<T>();
            var results = new List<IEnumerable<object>>();
            if (includeHeader) results.Add(columns);
            var displayNames = GetPropertyAttributes<T, DisplayNameAttribute>().Select(x => x.DisplayName).ToList();
            var properties = displayNames.Select(GetPropertyOfDisplayName<T>).ToList();
            for (var i = 0; i < entriesArray.Length; i++)
            {
                var entry = entriesArray[i];
                var entryValues = new List<string>();
                for (var j = 0; j < properties.Count; j++)
                {
                    var property = properties[j];
                    var value = property.GetValue(entry);
                    entryValues.Add(value?.ToString() ?? "");
                }
                results.Add(entryValues);
            }
            return results;
        }

        public static IEnumerable<object> DeParse<T>(T entry, IEnumerable<string>? columnsIn = null)
        {
            var columns = columnsIn?.ToList() ?? GetColumns<T>();
            var displayNames = GetPropertyAttributes<T, DisplayNameAttribute>().Select(x => x.DisplayName).ToList();
            var values = new List<string>();
            foreach (var columnName in displayNames)
            {
                var property = GetPropertyOfDisplayName<T>(columnName);
                var value = property.GetValue(entry);
                values.Add(value?.ToString() ?? "");
            }
            return values;
        }

        public static T ParseSingle<T>(IEnumerable<string> columnsIn, IEnumerable<object> valuesIn)
        where T : new()
        {
            var columns = columnsIn.ToList();
            var values = valuesIn.ToList();
            var displayNames = GetPropertyAttributes<T, DisplayNameAttribute>().Select(x => x.DisplayName).Where(x => columns.Contains(x)).ToList();
            //var displayNames = GetPropertyAttributes<T, DisplayNameAttribute>().Where(x => columns.Contains(x)).ToList();
            foreach (var columnName in displayNames)
            {
                if (!columns.Contains(columnName)) throw new ArgumentException($"Couldn't find column '{columnName}' of '{typeof(T).Name}' in '{string.Join(", ", displayNames)}'.");
            }
            var result = new T();
            foreach (var columnName in displayNames)
            {
                var property = GetPropertyOfDisplayName<T>(columnName);
                var excelStringValue = Value(columnName, columns, values);
                var value = Parse(property.PropertyType, excelStringValue);
                property.SetValue(result, value);
            }
            return result;
        }

        private static T ParseAs<T>(object value) => (T)Parse(typeof(T), value)!;
        private static object? Parse<T>(object value) => Parse(typeof(T), value);
        public static object? Parse(Type type, object? value)
        {
            if (value == null) return null;
            var underlyingType = GetUnderlyingType(type);
            if (underlyingType != null)
            {
                var stringValue = (string)value;
                if (stringValue == "") return null;
                type = underlyingType;
            }
            return type switch
            {
                _ when type == typeof(string) => value,
                _ when type == typeof(int) => ParseInt(value),
                _ when type == typeof(long) => ParseLong(value),
                _ when type == typeof(short) => ParseShort(value),
                _ when type == typeof(byte) => ParseByte(value),
                _ when type == typeof(float) => ParseFloat(value),
                _ when type == typeof(double) => ParseDouble(value),
                _ when type == typeof(decimal) => ParseDecimal(value),
                _ when type == typeof(DateTime) => ParseDateTime(value),
                _ when type.IsEnum => Enum.Parse(type, (string)value),
                _ when IsTypeCompatibile(typeof(D_string<Placeholder>), type) => value,
                _ when IsTypeCompatibile(typeof(D_int<Placeholder>), type) => ParseInt(value),
                _ when type == typeof(D_long<>) => ParseLong(value),
                _ when type == typeof(D_short<>) => ParseShort(value),
                _ when type == typeof(D_byte<>) => ParseByte(value),
                _ when type == typeof(D_float<>) => ParseFloat(value),
                _ when type == typeof(D_double<>) => ParseDouble(value),
                _ when type == typeof(D_decimal<>) => ParseDecimal(value),
                _ when type == typeof(D_DateTime<>) => ParseDateTime(value),
                _ => throw new ArgumentException($"Unhandled property type: '{type.Name}'")
            };
        }

        private static string CleanNumberForParse(string s)
        {
            s = Regex.Replace(s, @"-", "");
            s = Regex.Replace(s, @",", ".");
            s = Regex.Replace(s, @"\s+", "");
            return s.Length == 0 ? "0" : s;
        }

        private static string CleanNumberForIntParse(string s)
        {
            s = Regex.Replace(s, @"-", "");
            s = Regex.Replace(s, @",", "");
            s = Regex.Replace(s, @"\s+", "");
            if (s.EndsWith(".0")) s = s[0..^2];
            return s.Length == 0 ? "0" : s;
        }

        public static string ParseString(object? value) => value switch
        {
            null => "",
            string str => str,
            var obj => obj.ToString() ?? ""
        };

        public static int ParseInt(object value) => value switch
        {
            null => 0,
            int number => number,
            string str => string.IsNullOrWhiteSpace(str) ? 0 : int.Parse(CleanNumberForIntParse(str)),
            _ => throw new ArgumentException($"Cannot parse int: {value}")
        };

        public static long ParseLong(object value) => value switch
        {
            null => 0,
            long number => number,
            string str => string.IsNullOrWhiteSpace(str) ? 0 : long.Parse(CleanNumberForIntParse(str)),
            _ => throw new ArgumentException($"Cannot parse long: {value}")
        };

        public static short ParseShort(object value) => value switch
        {
            null => 0,
            short number => number,
            string str => string.IsNullOrWhiteSpace(str) ? (short)0 : short.Parse(CleanNumberForIntParse(str)),
            _ => throw new ArgumentException($"Cannot parse short: {value}")
        };

        public static byte ParseByte(object value) => value switch
        {
            null => 0,
            byte number => number,
            string str => string.IsNullOrWhiteSpace(str) ? (byte)0 : byte.Parse(CleanNumberForIntParse(str)),
            _ => throw new ArgumentException($"Cannot parse int: {value}")
        };

        public static float ParseFloat(object value) => value switch
        {
            null => 0,
            float number => number,
            string str => string.IsNullOrWhiteSpace(str) ? 0 : float.Parse(CleanNumberForParse(str)),
            _ => throw new ArgumentException($"Cannot parse float: {value}")
        };

        public static double ParseDouble(object value) => value switch
        {
            null => 0,
            double number => number,
            string str => string.IsNullOrWhiteSpace(str) ? 0 : double.Parse(CleanNumberForParse(str)),
            _ => throw new ArgumentException($"Cannot parse float: {value}")
        };

        public static decimal ParseDecimal(object value) => value switch
        {
            null => 0,
            decimal number => number,
            string str => string.IsNullOrWhiteSpace(str) ? 0 : (str).Contains('%') ? ParsePercent(CleanNumberForParse(str)) : decimal.Parse(CleanNumberForParse(str)),
            _ => throw new ArgumentException($"Cannot parse decimal: {value}")
        };

        public static decimal ParsePercent(object value) => 0.01m * decimal.Parse(((string)value).Split('%')[0].Trim());

        public static DateTime ParseDateTime(object value) => value switch
        {
            DateTime date => date,
            int number => new DateTime(1900, 1, 1).AddDays(number),
            string str => string.IsNullOrWhiteSpace(str) ? default : IsInt(str) ? new DateTime(1900, 1, 1).AddDays(int.Parse(str)) : CanParseNorwegianDate(str) ? ParseNorwegianDateTime(str) : (str.Contains('.') ? ParseDateOfNorwegianDateOrder(str) : DateTime.Parse(str)),
            _ => throw new ArgumentException($"Cannot parse date: {value}")
        };

        private static DateTime ParseDateOfNorwegianDateOrder(string str)
        {
            var splitted = str.Split('.');
            var date = splitted[0];
            var month = splitted[1];
            var year = splitted[2];
            return DateTime.Parse($"{month}/{date}/{year}");
        }

        private static object Value(string columnName, IList<string> columns, IReadOnlyList<object> values)
        {
            var index = columns.IndexOf(columnName);
            return index < 0 ? "" : values[index];
        }

        public static DateTime ParseNorwegianDateTime(string value)
        {
            value = value.Replace("_", " ");
            var splitted = value.Split(' ');
            var yearIndex0 = int.TryParse(splitted[0], out int year);
#pragma warning disable CA1806 // Do not ignore method results
            if (!yearIndex0) int.TryParse(splitted[1], out year);
#pragma warning restore CA1806 // Do not ignore method results
            var month = 1 + Array.IndexOf(MONTHS_NORWEGIAN, splitted[yearIndex0 ? 1 : 0].Trim().ToLower());
            if (year < 100) year += 2000;
            return new DateTime(year, month, 1);
        }

        private static bool IsInt(string s) => int.TryParse(s, out _);

        public static string FormatPercent(string value)
        {
            var number = ParseDecimal(value);
            var percents = number * 100;
            return $"{percents} %";
        }
        //public static DateTime ParseNorwegianDateTime(string value)
        //{
        //    var splitted = value.Split(" ");
        //    var month = 1 + Array.IndexOf(MONTHS_NORWEGIAN, splitted[0].Trim().ToLower());
        //    var dayOfMonth = int.Parse(splitted[1]);
        //    return new DateTime(2020, month, dayOfMonth);
        //}

        private static bool CanParseNorwegianDate(string date)
        {
            date = date.Trim();
            var splitted = date.Split(' ');
            if (splitted.Length != 2) return false;
            var dato = splitted[0].ToLower();
            if (!MONTHS_NORWEGIAN.Contains(dato)) return false;
            return true;
        }

        //public static string DeParseNorwegianDateTime(DateTime date)
        //{
        //    var month = MONTHS_NORWEGIAN[date.Month - 1];
        //    var splitted = value.Split(" ");
        //    var month = 1 + Array.IndexOf(MONTHS_NORWEGIAN, splitted[0].Trim().ToLower());
        //    var dayOfMonth = int.Parse(splitted[1]);
        //    return new DateTime(2020, month, dayOfMonth);
        //}

        private static readonly string[] MONTHS_NORWEGIAN = new[] { "januar", "februar", "mars", "april", "mai", "juni", "juli", "august", "september", "oktober", "november", "desember" };
    }
}