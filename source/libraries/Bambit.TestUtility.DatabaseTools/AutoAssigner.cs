using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Bambit.TestUtility.DatabaseTools.Attributes;

namespace Bambit.TestUtility.DatabaseTools
{
    /// <summary>
    /// Provides methods to help assign properties on objects
    /// </summary>
    public static partial class AutoAssigner
    {
        /// <summary>
        /// Used to determine if a property is null
        /// </summary>
        private static readonly NullabilityInfoContext NullabilityInfoContext = new();

        /// <summary>
        /// A dictionary with conversion from string to other types
        /// </summary>

        public static readonly IReadOnlyDictionary<Type, Func<string, object>> Converters =
            new Dictionary<Type, Func<string, object>>
            {
                { typeof(int), (s) => int.Parse(s) },
                { typeof(uint), (s) => uint.Parse(s) },
                { typeof(long), (s) => long.Parse(s) },
                { typeof(decimal), (s) => decimal.Parse(s) },
                { typeof(double), (s) => double.Parse(s) },
                { typeof(float), (s) => float.Parse(s) },
                { typeof(byte), (s) => byte.Parse(s) },
                { typeof(short), (s) => short.Parse(s) },
                { typeof(string), (s) => s },
                { typeof(Guid), (s) => Guid.Parse(s) },
                { typeof(DateTime), (s) => ParseDateExtended(s) },
                { typeof(DateTimeOffset), (s) => DateTimeOffset.Parse(s) },
                {
                    typeof(bool),
                    (value) =>
                    {
                        string lowerValue = value.ToLower();
                        return lowerValue.Length > 0 &&
                               (lowerValue[0] == 'y' || lowerValue == "true" || lowerValue[0] == '1');
                    }
                },
                {typeof(PhysicalAddress), PhysicalAddress.Parse},
                {typeof(IPAddress), IPAddress.Parse},
                {typeof(BitArray), input =>
                {
                    bool[] bits = new bool[input.Length];
                    int i = 0;
                    foreach (char c in input)
                    {
                        bits[i] = c == '1';
                        i++;
                    }

                    return new BitArray(bits);
                }},
            };

        /// <summary>
        /// Parses our a string, allowing custom day types (e.g., today, yesterday) along with adding time spans (hours, days, etc.)
        /// </summary>
        /// <param name="stringValue">The value to parse.</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="ArgumentException">The supplied string could not be parsed</exception>
        public static DateTime ParseDateExtended(string stringValue)
        {
            if (DateTime.TryParse(stringValue, out DateTime parsedDate))
                return parsedDate;
            string dateValue = stringValue.Replace(" ", "").ToLower();

            switch (dateValue)
            {
                case "today":
                    return DateTime.Today;
                case "yesterday":
                    return DateTime.Today.AddDays(-1);
                case "tomorrow":
                    return DateTime.Today.AddDays(1);
                case "now":
                    return DateTime.Now;
                case "firstdayofthismonth":
                case "firstdayofcurrentmonth":
                case "firstdayofmonth":
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                case "lastdayofthismonth":
                case "lastdayofcurrentmonth":
                case "lastdayofmonth":
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
                case "endoflastmonth":
                case "lastdayoflastmonth":
                case "lastdayofpreviousmonth":
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            }

            Regex patternMatch = DatePatternMatch();
            Match match = patternMatch.Match(dateValue);
            if (!match.Success)
                throw new ArgumentException($"unable to parse date value '{stringValue}'", nameof(stringValue));
            string[] values =
            [
                match.Groups[1].Value,
                match.Groups[2].Value,
                match.Groups[3].Value,
                match.Groups[4].Value
            ];
            DateTime firstDate = ParseDateExtended(values[0]);
            if (!int.TryParse(values[2], out int addition))
                throw new ArgumentException($"unable to parse date value '{stringValue}'", nameof(stringValue));
            if (values[1] == "-")
                addition *= -1;
            if (values[3].StartsWith("month"))
                return firstDate.AddMonths(addition);
            if (values[3].StartsWith("hour"))
                return firstDate.AddHours(addition);
            if (values[3].StartsWith("second"))
                return firstDate.AddSeconds(addition);
            if (values[3].StartsWith("minute"))
                return firstDate.AddMinutes(addition);
            if (values[3].StartsWith("year"))
                return firstDate.AddYears(addition);
            return firstDate.AddDays(addition);

        }
        
        /// <summary>
        /// Applies a string dictionary and assigns the values to the target object if the properties match.  Returns an array of properties that matched
        /// </summary>
        /// <param name="valueDictionary">A <see cref="IDictionary{TKey,TValue}"/> of string values against string keys.  The keys will be matched against the properties of the assignee, while the
        /// values will be parsed out to the correct type</param>
        /// <param name="assignee">The object to assign the values on</param>
        /// <returns>A list of properties (keys) that were assigned</returns>
        public static string[] AssignValuesIfDefined(this IDictionary<string,string?> valueDictionary, object assignee
            ) 
        {

            Type t = assignee.GetType();
            PropertyInfo[] propertyInfos =
                t.GetProperties(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField |
                                BindingFlags.Public);

            List<string> assignedColumns = [];
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {

                string? propertyName = AssignProperty(propertyInfo, assignee, valueDictionary);
                if (!string.IsNullOrWhiteSpace(propertyName))
                    assignedColumns.AddRange(propertyName.Split("|".ToCharArray()));
            }

            return [.. assignedColumns];
        }

        #region Private Methods

        private static string? AssignProperty(PropertyInfo propertyInfo, object assignee,IDictionary<string,string?> valueDictionary)
        {
            Type propertyType = propertyInfo.PropertyType;
            if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                return null;
            bool nullable = false;
            Type? underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            string name = $"{propertyInfo.Name}";
            if (underlyingType != null)
            {
                propertyType = underlyingType;
                nullable = true;
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                NullabilityInfo nullabilityInfo = NullabilityInfoContext.Create(propertyInfo);
                nullable = nullabilityInfo.WriteState is NullabilityState.Nullable ||
                           propertyInfo.GetCustomAttributes(typeof(DatabaseNullableAttribute)).Any();
            }
            if (Converters.TryGetValue(propertyType, out var converter))
            {

                return AssignIfDefined(assignee, name, propertyInfo, nullable,valueDictionary, converter) ? name : null;
            }

            return null;
        }

        private static bool AssignIfDefined(object assignee, string columnName, PropertyInfo propertyInfo, bool allowNull,
            IDictionary<string,string?> valueDictionary,
            Func<string, object> converter)
        {

            if (!valueDictionary.TryGetValue(columnName, out string? stringValue))
                return false;


            if (string.IsNullOrWhiteSpace(stringValue))
            {
                if (!allowNull)
                    throw new NullReferenceException(
                        $"Column '{columnName}' has a null value, but the field is marked as non-null field");
                propertyInfo.SetValue(assignee, null);
            }
            else
            {

                object value = converter(stringValue);
                propertyInfo.SetValue(assignee, value);
            }

            return true;
        }

        [GeneratedRegex(@"(.*)([+-])(\d{1,})(.*)?")]
        private static partial Regex DatePatternMatch();

        #endregion Private Methods

    }
}