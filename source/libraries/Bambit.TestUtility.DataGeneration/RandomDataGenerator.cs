using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration.Attributes;

namespace Bambit.TestUtility.DataGeneration
{
    /// <summary>
    /// General implementation of the <see cref="IRandomDataGenerator"/> class
    /// </summary>
    public class RandomDataGenerator : IRandomDataGenerator
    {
        #region Lookup Data

        /// <summary>
        /// This list is (mostly) derived from the common first names in America at some point in time.
        /// It's not international (sorry) but if needed derived classes can use a different list
        /// </summary>
        public static readonly string[] FirstNames =
        [
            "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
            "David", "Barbara",
            "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy",
            "Daniel", "Margaret",
            "Matthew", "Lisa", "Anthony", "Betty", "Donald", "Dorothy", "Mark", "Sandra", "Paul", "Ashley", "Steven",
            "Kimberly", "Andrew", "Donna",
            "Kenneth", "Emily", "Joshua", "Michelle", "George", "Carol", "Kevin", "Amanda", "Brian", "Melissa",
            "Edward", "Deborah", "Ronald", "Stephanie",
            "Timothy", "Rebecca", "Jason", "Laura", "Jeffrey", "Sharon", "Ryan", "Cynthia", "Jacob", "Kathleen", "Gary",
            "Helen",
            "Nicholas", "Amy", "Eric", "Shirley", "Stephen", "Angela", "Orion", "Siana", "Jonathan", "Anna", "Larry",
            "Brenda", "Justin", "Pamela",
            "Scott", "Nicole", "Brandon", "Ruth", "Frank", "Katherine", "Benjamin", "Samantha", "Gregory", "Christine",
            "Samuel", "Emma",
            "Raymond", "Catherine", "Patrick", "Debra", "Alexander", "Virginia", "Jack", "Rachel", "Dennis", "Carolyn",
            "Jerry", "Janet",
            "Tyler", "Maria", "Aaron", "Heather", "Jose", "Diane", "Henry", "Julie", "Douglas", "Joyce", "Adam",
            "Victoria", "Peter", "Kelly",
            "Nathan", "Christina", "Zachary", "Joan", "Walter", "Evelyn", "Kyle", "Lauren", "Harold", "Judith", "Carl",
            "Olivia", "Jeremy", "Frances",
            "Keith", "Martha", "Roger", "Cheryl", "Gerald", "Megan", "Ethan", "Andrea", "Arthur", "Hannah", "Terry",
            "Jacqueline", "Christian", "Ann",
            "Sean", "Jean", "Lawrence", "Alice", "Austin", "Kathryn", "Joe", "Gloria", "Noah", "Teresa", "Jesse",
            "Doris", "Albert", "Sara",
            "Bryan", "Janice", "Billy", "Julia", "Bruce", "Marie", "Willie", "Madison", "Jordan", "Grace", "Dylan",
            "Judy", "Alan", "Theresa",
            "Ralph", "Beverly", "Gabriel", "Denise", "Roy", "Marilyn", "Juan", "Amber", "Wayne", "Danielle", "Eugene",
            "Abigail", "Logan", "Brittany",
            "Randy", "Rose", "Louis", "Diana", "Russell", "Natalie", "Vincent", "Sophia", "Philip", "Alexis", "Bobby",
            "Lori"
        ];

        /// <summary>
        /// This list is (mostly) derived from the common last names in America at some point in time.
        /// It's not international (sorry) but if needed derived classes can use a different list
        /// </summary>

        public static readonly string[] LastNames =
        [
            "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor",
            "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez",
            "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King",
            "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell",
            "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart",
            "Sanchez",
            "Morris", "Rogers", "Reed", "Cook", "Metzler", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper",
            "Richardson",
            "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly",
            "Sanders",
            "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long",
            "Patterson",
            "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander",
            "Russell", "Griffin",
            "Diaz", "Hayes"
        ];

        #endregion Lookup Data

        #region Static Fields

        /// <summary>
        /// A static implementation (lazy backer) for a default generator
        /// </summary>
        protected static readonly Lazy<RandomDataGenerator> LazyInstance = new(() => new RandomDataGenerator());
        
        /// <summary>
        /// Static implementation for default functionality
        /// </summary>
        public static readonly RandomDataGenerator Instance = LazyInstance.Value;

        
        /// <summary>
        /// The Random class is used for all random generators
        /// </summary>
        protected static readonly Random Random = new();


        #endregion Static Fields

        #region Fields

        /// <summary>
        /// Defines object types that, if found as properties, will be automatically generated.
        /// The key is the value to find, the value is the type to instantiate with. This allows
        /// for interfaces or super classes to be instantiated with specific types
        /// </summary>
        private readonly Dictionary<Type, Type> AutoProperties;

        
        /// <summary>
        /// Contains a mapping of field names to the generators that will produce the string.
        /// Used if a class has fields like "FirstName" or "State", you can use specific generator methods
        /// to produce more realistic data if you want
        /// </summary>
        private readonly Dictionary<string, Func<string>> FieldNamesToGenerators;

        /// <summary>
        /// Contains a mapping of types to specialized functions to generate them.  Allows for global customization
        /// of any given type.
        /// </summary>
        private readonly Dictionary<Type, Func<object>> MappedInitializeFunctions;

        /// <summary>
        /// The maximum number of child entries that will be recursed into
        /// </summary>
        public int MaxRecursion { get; set; } = 20;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Default ctor.  Protected as this should be derived or called via the <see cref="Instance"/>
        /// </summary>
        protected RandomDataGenerator()
        {
            AutoProperties = [];
            MappedInitializeFunctions = [];
            FieldNamesToGenerators = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion Fields

        #region IRandomDataGenerator Implementation

        #region Generation Methods

        #region Basic types
        
        /// <inheritdoc />
        public virtual bool GenerateBoolean()
        {
            return GenerateInt() % 2 == 0;
        }
        
        /// <inheritdoc />
        public virtual byte GenerateByte()
        {
            return Convert.ToByte(GenerateInt(0, 255));
        }

        /// <inheritdoc />
        public virtual DateTime GenerateDate(int daysAgoMinimum = -30, int daysFutureMaximum = 30)
        {
            return DateTime.Today.AddDays(GenerateInt(daysAgoMinimum, daysFutureMaximum));
        }

        
        /// <inheritdoc />
        public virtual DateTime GenerateDateTime(int daysAgoMinimum = -30, int daysFutureMaximum = 30)
        {
            return DateTime.Today.AddDays(GenerateInt(daysAgoMinimum, daysFutureMaximum))
                    .AddSeconds(GenerateDouble(0, 24 * 60 * 60))
                    .AddMilliseconds(GenerateDouble(0, 1000))
                    .AddMicroseconds(GenerateDouble(0, 1000))
                ;
        }

        /// <inheritdoc />
        public virtual decimal GenerateDecimal(decimal low = 0m, decimal high = 10000000000m)
        {
            double value = GenerateDouble((double)low, (double)high);
            return Convert.ToDecimal(value);
        }

        /// <inheritdoc />
        public virtual decimal GenerateDecimal(byte precision, byte scale)
        {
            if (precision - scale > 10)
            {
                precision = (byte)(scale > 10 ? scale : 10);
            }

            double high = Math.Pow(10, precision);
            double divisor = Math.Pow(10, scale);
            double value = Math.Floor(GenerateDouble(0, high));
            return Convert.ToDecimal(value / divisor);
        }
        
        /// <inheritdoc />
        public virtual double GenerateDouble(double low = 0, double high = double.MaxValue)
        {
            double range = high - low;
            double rand = Random.NextDouble();
            return range * rand + low;
        }
        
        /// <inheritdoc />
        public virtual double GenerateDouble(byte precision, byte scale)
        {
            if (precision - scale > 10)
            {
                precision = (byte)(scale > 10 ? scale : 10);
            }

            double high = Math.Pow(10, precision);
            double divisor = Math.Pow(10, scale);
            double value = Math.Floor(GenerateDouble(0, high));
            return value / divisor;
        }
        
        /// <inheritdoc />
        public virtual Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
          
        /// <inheritdoc />
        public virtual int GenerateInt(int low = 0, int high = int.MaxValue / 100)
        {
            return Random.Next(low, high);
        }

        /// <inheritdoc />
        public string GenerateFirstName()
        {
            return FirstNames[GenerateInt(0, FirstNames.Length)];
        }
        
        /// <inheritdoc />
        public string GenerateLastName()
        {
            return LastNames[GenerateInt(0, LastNames.Length)];
        }
        
        /// <inheritdoc />
        public string GenerateName()
        {
            return $"{GenerateFirstName()} {GenerateLastName()}";

        }
        
        /// <inheritdoc />
        public virtual string GenerateString(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Invalid number of characters", nameof(length));

            StringBuilder stringBuilder = new();
            for (int x = 0; x < length; x++)
            {
                stringBuilder.Append((char)Random.Next(97, 122));
            }

            return stringBuilder.ToString();

        }

        #endregion Basic types
        
        /// <inheritdoc />
        public virtual decimal GenerateMoney(decimal low = 0m, decimal high = 10000000000m)
        {
            return Math.Floor(GenerateDecimal(low, high) * 100) / 100;
        }
        #endregion Generation Methods

        #region Configuration 
        
        /// <inheritdoc />
        public void AddAutoProperties<T, T2>() where T2 : T, new()
        {
            Type t = typeof(T);
            Type t2 = typeof(T2);
            AutoProperties[t] = t2;
        }
        
        /// <inheritdoc />
        public void AddAutoProperties<T>() where T : new()
        {
            AddAutoProperties<T, T>();
        }
        
        /// <inheritdoc />
        public void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod) where T : notnull
        {
            Type t = typeof(T);
            if (!MappedInitializeFunctions.ContainsKey(t))
                MappedInitializeFunctions.Add(t, () => initMethod(this));
            else
                MappedInitializeFunctions[t] = () => initMethod(this);
        }
        
        /// <inheritdoc />
        public void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod, bool autoProperty)
            where T : notnull, new()
        {
            AddCustomObjectInitialization(initMethod);
            if (autoProperty)
                AddAutoProperties<T>();
        }
        
        /// <summary>
        /// Add a string generator for a specified type of name
        /// </summary>
        /// <param name="fieldName">The name of the field to match when generating</param>
        /// <param name="generator">The generator to use</param>
        public void AddStringFieldNameGenerator(string fieldName, Func<string> generator)
        {
            FieldNamesToGenerators[fieldName] = generator;
        }

        #endregion Configuration 
        
        #region Instantiation Methods
        
        /// <inheritdoc />
        public virtual object CreateObject(Type objectType)
        {
            object? newObject;
            if (MappedInitializeFunctions.TryGetValue(objectType, out Func<object>? function))
                newObject = function();
            else
            {

                newObject = Activator.CreateInstance(objectType);
                if (newObject == null)
                    throw new InvalidOperationException(
                        $"Could not create instance of object of type '{objectType}.  Nullable value not allowed");

                InitializeObject(newObject);
            }

            return newObject;
        }
        
        /// <inheritdoc />
        public virtual Dictionary<string, TValue> InitializeDictionary<TValue>(int numberItems)
            where TValue : notnull, new()
        {
            Dictionary<string, TValue> results = [];
            for (int x = 0; x < numberItems; x++)
            {
                results.Add(GenerateString(12), InitializeObject<TValue>());
            }

            return results;
        }
        
         /// <inheritdoc />
        public virtual Dictionary<TKey, TValue> InitializeDictionary<TKey, TValue>(int numberItems)
            where TKey : notnull, new()
            where TValue : notnull, new()
        {
            Dictionary<TKey, TValue> results = [];
            for (int x = 0; x < numberItems; x++)
            {
                results.Add(InitializeObject<TKey>(), InitializeObject<TValue>());
            }

            return results;
        }
       
        /// <inheritdoc />
        public virtual T InitializeObject<T>(Action<T>? modifierFunction) 
            where T :  notnull, new()
        {
            Type t = typeof(T);
            T newObject;
            if (MappedInitializeFunctions.TryGetValue(t, out Func<object>? function))
                newObject = (T)function();
            else
            {
                newObject = new T();
                InitializeObject(newObject);
            }

            modifierFunction?.Invoke(newObject);
            return newObject;
        }
        
        /// <inheritdoc />
        public virtual List<T> InitializeList<T>(int numberItems, Func<IRandomDataGenerator, T> initializeFunction)
            where T : new()
        {
            List<T> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                list.Add(initializeFunction(this));
            }

            return list;
        }
        
        /// <inheritdoc />
        public virtual IList<T> InitializeList<T>(int numberItems) where T : notnull, new()
        {
            List<T> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                list.Add(InitializeObject<T>());
            }

            return list;
        }
        
        /// <inheritdoc />
        public virtual IList<T> InitializeList<T>(int numberItems, Action<T> postCreate) 
            where T : notnull, new()
        {
            List<T> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                T entity = InitializeObject(postCreate);
                list.Add(entity);
            }

            return list;
        }
        
        /// <inheritdoc />
        public virtual IList<string> InitializeList(int numberItems, int maxLength)
        {
            List<string> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                list.Add(GenerateString(maxLength));
            }

            return list;
        }
        
         
        /// <inheritdoc />
        public virtual T InitializeObject<T>() where T : notnull, new()
        {
            return InitializeObject<T>(null);
        }
       
        
        /// <inheritdoc />
        public virtual T InitializeObject<T>(T objectToInitialize) 
            where T : notnull 
        {
            return InitializeObject(objectToInitialize, MaxRecursion);
        }
        
        /// <inheritdoc />
        public virtual T InitializeObject<T>(T objectToInitialize, Action<T>? modifierFunction)
            where T : notnull
        {
            Type objectType = objectToInitialize.GetType();
            PropertyInfo[] propertyInfos =
                objectType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                    .Where(p => p is { CanRead: true, CanWrite: true }).ToArray();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                AssignValueToProperty(objectToInitialize, propertyInfo, MaxRecursion);

            }

            modifierFunction?.Invoke(objectToInitialize);
            return objectToInitialize;
        }

      
        #endregion Instantiation Methods

        #region Misc Methods
        
        /// <inheritdoc />
        public virtual DateTime GetFirstDayOfMonth()
        {
            return GetFirstDayOfMonth(DateTime.Today);
        }
        /// <inheritdoc />
        public virtual DateTime GetFirstDayOfMonth(DateTime monthDate)
        {
            return new DateTime(monthDate.Year, monthDate.Month, 1);
        }

        /// <inheritdoc />
        public virtual DateTime GetLastDayOfMonth()
        {
            return GetLastDayOfMonth(DateTime.Today);
        }

        /// <inheritdoc />
        public virtual DateTime GetLastDayOfMonth(DateTime monthDate)
        {
            return GetFirstDayOfMonth(monthDate).AddMonths(1).AddDays(-1);
        }
        #endregion Misc Methods

        #region Selection methods
        /// <inheritdoc />
        public T GenerateEnum<T>() where T : struct, Enum, IConvertible
        {
           
            string[] names = Enum.GetNames(typeof(T));
            return (T)Enum.Parse(typeof(T), GetEntry(names)) ;
        }
        
        /// <inheritdoc />
        public T CoinToss<T>(T heads, T tails)
        {
            return GenerateInt(0, 2) == 0 ? heads : tails;
        }
        
        /// <inheritdoc />
        public T GetListEntry<T>(List<T> list)
        {
            return list[GenerateInt(0, list.Count)];
        }
        
        /// <inheritdoc />
        public T GetEntry<T>(T[] list)
        {
            return list[GenerateInt(0, list.Length)];
        }
        #endregion Selection Methods
        
        #endregion IRandomDataGenerator Implementation

        #region Protected Methods
        
        /// <summary>
        /// Assigns a value to a property, based on it's type
        /// </summary>
        /// <typeparam name="T">The type of object holding the property to be assigned</typeparam>
        /// <param name="objectToInitialize">The object to assign the property to</param>
        /// <param name="propertyInfo">Information about the property being assigned</param>
        /// <param name="maxRecursion">The maximum depth to descent for children</param>
        protected virtual void AssignValueToProperty<T>(T objectToInitialize, PropertyInfo propertyInfo,
            int maxRecursion)
        where T : notnull
        {
            if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                return;
            Type propertyType = propertyInfo.PropertyType;

            Type? underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (underlyingType != null)
            {
                propertyType = underlyingType;
            }

            if (AutoProperties.TryGetValue(propertyType, out Type? mappedType))
            {
                SetProperty(objectToInitialize, propertyInfo, mappedType, maxRecursion);
            }
            else if (propertyType == typeof(string))
            {
                SetStringProperty(objectToInitialize, propertyInfo);
            }
            else if (propertyType == typeof(short))
            {
                propertyInfo.SetValue(objectToInitialize, (short)GenerateInt(short.MinValue, short.MaxValue));
            }
            else if (propertyType == typeof(int))
            {
                propertyInfo.SetValue(objectToInitialize, GenerateInt());
            }
            else if (propertyType == typeof(long))
            {
                propertyInfo.SetValue(objectToInitialize, (long)GenerateInt());
            }
            else if (propertyType == typeof(DateTime))
            {
                propertyInfo.SetValue(objectToInitialize, GenerateDate());
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                propertyInfo.SetValue(objectToInitialize, new DateTimeOffset(GenerateDate()));
            }
            else if (propertyType == typeof(decimal))
            {
                DecimalPrecisionAttribute? precisionAttribute =
                    propertyInfo.GetCustomAttribute<DecimalPrecisionAttribute>();
                propertyInfo.SetValue(objectToInitialize,
                    precisionAttribute == null
                        ? GenerateMoney()
                        : GenerateDecimal(precisionAttribute.Precision, precisionAttribute.Scale));
            }
            else if (propertyType == typeof(double))
            {
                DecimalPrecisionAttribute? precisionAttribute =
                    propertyInfo.GetCustomAttribute<DecimalPrecisionAttribute>();
                propertyInfo.SetValue(objectToInitialize,
                    precisionAttribute == null
                        ? GenerateDouble()
                        : GenerateDouble(precisionAttribute.Precision, precisionAttribute.Scale));
            }
            else if (propertyType == typeof(bool))
            {
                propertyInfo.SetValue(objectToInitialize, GenerateBoolean());
            }
            else if (propertyType == typeof(Guid))
            {
                propertyInfo.SetValue(objectToInitialize, GenerateGuid());
            }
        }

        /// <summary>
        /// Generates a string for a given field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected virtual string? GenerateStringForFieldName(string fieldName)
        {
            return
                FieldNamesToGenerators.TryGetValue(fieldName, out Func<string>? generator)
                    ? generator.Invoke()
                    : null;
        }

        /// <summary>
        /// Populates the properties of the supplied object.
        /// </summary>
        /// <typeparam name="T">The type of object that will be initialized.</typeparam>
        /// <param name="objectToInitialize">The object which to have its properties initialize</param>
        /// <param name="maxRecursion">The maximum depth of child objects to create</param>
        /// <returns>The supplied objectToInitialize with properties initialized</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddCustomObjectInitialization{T}(Func{IRandomDataGenerator,T})">AddCustomObjectInitialization</see></remarks>
        protected virtual T InitializeObject<T>(T objectToInitialize, int maxRecursion) where T : notnull
        {
            if (--maxRecursion < 1)
                throw new InvalidOperationException("Maximum recursion level hit");
            Type objectType = objectToInitialize.GetType();
            PropertyInfo[] propertyInfos =
                objectType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                    .Where(p => p is { CanRead: true, CanWrite: true }).ToArray();
            
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                AssignValueToProperty(objectToInitialize, propertyInfo, maxRecursion);

            }

            return objectToInitialize;
        }


        #endregion Protected Methods

        #region private Methods

        /// <summary>
        /// Sets the property on an object to a mapped type instance.  Used often when the property is an interface of base type and a specific type is desired.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToInitialize">The object that the property belongs to</param>
        /// <param name="propertyInfo">Information about the property</param>
        /// <param name="mappedType">The mapped types to use.</param>
        /// <param name="maxRecursion">The maximum depth of recursion left to initialize child objects.</param>
        private void SetProperty<T>(T objectToInitialize, PropertyInfo propertyInfo, Type mappedType, int maxRecursion)
        {
            
            object? newObject;
            if (MappedInitializeFunctions.TryGetValue(mappedType, out Func<object>? function))
                newObject = function();
            else
            {
                newObject = Activator.CreateInstance(mappedType);
                InitializeObject(newObject!, maxRecursion);
            }

            propertyInfo.SetValue(objectToInitialize, newObject);
        }

        /// <summary>
        /// Sets a string property on an object.
        /// </summary>
        /// <param name="objectToInitialize">The object with the string property</param>
        /// <param name="propertyInfo">The information about the property to initialize</param>
        /// <remarks>
        /// This method will first try to assign a name if the field name suggests it (calling <see cref="GenerateStringForFieldName"/>.
        /// If not it will use a random length, unless specified by a <see cref="StringLengthAttribute"/> or <see cref="MaxLengthAttribute"/>
        /// </remarks>
        private void SetStringProperty(object objectToInitialize, PropertyInfo propertyInfo)
        {
            string? value = GenerateStringForFieldName(propertyInfo.Name);
            if (string.IsNullOrWhiteSpace(value))
            {
                int maxSize = GenerateInt(30, 50);

                StringLengthAttribute? stringLengthAttribute =
                    propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                if (stringLengthAttribute != null)
                    maxSize = stringLengthAttribute.MaximumLength;
                MaxLengthAttribute? maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttribute != null)
                    maxSize = maxLengthAttribute.Length;
                value = GenerateString(maxSize);
            }

            propertyInfo.SetValue(objectToInitialize, value);
        }

        
        #endregion private Methods


    }

}
