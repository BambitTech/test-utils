using System.Reflection;
using Bambit.TestUtility.DatabaseTools.Attributes;

namespace Bambit.TestUtility.DatabaseTools
{
    /// <summary>
    /// base class used when generating object from database tables via <see cref="ITableToClassBuilder"/>
    /// </summary>
    public abstract class DatabaseMappedClass
    {
        /// <summary>
        /// Assigns all properties on an object if they are not marked as computed
        /// </summary>
        /// <param name="values">A <see cref="Dictionary{TKey,TValue}"/> of name/values to assign</param>
        /// <exception cref="InvalidDataException">The supplied value was not valid for the targeted field</exception>
        public void AssignValuesIfDefined(Dictionary<string, object?> values)
        {
            Type t = GetType();
            PropertyInfo[] propertyInfos =
                t.GetProperties(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField |
                                BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {


                string name = $"{propertyInfo.Name}";
                if (values.TryGetValue(name, out object? value))
                {
                    AssignProperty(propertyInfo, value);
                }
            }
        }

        /// <summary>
        /// Retrieves the property on the class with the specified name
        /// </summary>
        /// <typeparam name="T">The data type of the property</typeparam>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="ArgumentException">The specified property name does not exist on the class</exception>
        /// <remarks>Used if not implementing class as a dynamic</remarks>
        public T?  GetValue<T>(string propertyName)
        {
            PropertyInfo propertyInfo = GetType().GetProperties().FirstOrDefault(p =>
                                            string.Compare(
                                            p.Name ,propertyName, StringComparison.CurrentCultureIgnoreCase)==0

                                            ) ??
                                         throw new ArgumentException("No such property exists", nameof(propertyName));
            object? value = propertyInfo.GetValue(this);
            return value == null ? default : (T)value;
        }
        
        /// <summary>
        /// Retrieves the property on the class with the specified name
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="ArgumentException">The specified property name does not exist on the class</exception>
        /// <remarks>Used if not implementing class as a dynamic</remarks>
        public object?  GetValue(string propertyName)
        {
            PropertyInfo propertyInfo = GetType().GetProperties().FirstOrDefault(p =>
                                            string.Compare(
                                                p.Name ,propertyName, StringComparison.CurrentCultureIgnoreCase)==0

                                        ) ??
                                        throw new ArgumentException("No such property exists", nameof(propertyName));
            return propertyInfo.GetValue(this);
        }
        /// <summary>
        /// Assigns the supplied value against the property info
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> defining the property to assign</param>
        /// <param name="value">The value to assign</param>
        /// <exception cref="InvalidDataException">The supplied value was not valid for the targeted field</exception>
        /// <remarks>If the targeted field has a <see cref="ComputedColumnAttribute"/>, it will not be set</remarks>
        protected void AssignProperty(  PropertyInfo propertyInfo, object? value )
        {
            if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                return ;
            
            try
            {
                propertyInfo.SetValue(this, value);
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"An error was thrown attempting to assign value '{propertyInfo.Name}' with value '{value}'", e);
            }

        }



    }
}
