using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Bambit.TestUtility.DataGeneration.Attributes;

namespace Bambit.TestUtility.DataGeneration
{
    public class RandomDataGenerator : IRandomDataGenerator
    {
        #region Lookup Data

        /// <summary>
        /// This list is (mostly) derived from the common first names in America at some point in time.
        /// It's not international (sorry) but if needed derived classes can use a different list
        /// </summary>
        public static readonly string[] FirstNames =
        {
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
        };

        /// <summary>
        /// This list is (mostly) derived from the common last names in America at some point in time.
        /// It's not international (sorry) but if needed derived classes can use a different list
        /// </summary>

        public static readonly string[] LastNames =
        {
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
        };

        #endregion Lookup Data

        #region Static Fields

        /// <summary>
        /// The Random class is used for all random generators
        /// </summary>
        protected static Random Random = new Random();

        /// <summary>
        /// A static implementation (lazy backer) for a default generator
        /// </summary>
        protected static Lazy<RandomDataGenerator> LazyInstance = new(() => new RandomDataGenerator());

        /// <summary>
        /// Static implementation for default functionality
        /// </summary>
        public static RandomDataGenerator Instance = LazyInstance.Value;

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
        /// Contains a mapping of types to specialized functions to generate them.  Allows for global customaization
        /// of any given type.
        /// </summary>
        private readonly Dictionary<Type, Func<object>> MappedInitializeFunctions;


        public int MaxRecursion { get; set; } = 20;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Default ctor.  Protected as this should be derived or called via the <see cref="Instance"/>
        /// </summary>
        protected RandomDataGenerator()
        {
            AutoProperties = new Dictionary<Type, Type>();
            MappedInitializeFunctions = new Dictionary<Type, Func<object>>();
            FieldNamesToGenerators = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion Fields

        #region IRandomDataGenerator Implementation

        #region Generation Methods

        #region Basic types

        public virtual string GenerateString(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Invalid number of characters", nameof(length));

            StringBuilder stringBuilder = new StringBuilder();
            for (int x = 0; x < length; x++)
            {
                stringBuilder.Append((char)Random.Next(97, 122));
            }

            return stringBuilder.ToString();

        }

        public virtual double GenerateDouble(double low = 0, double high = double.MaxValue)
        {
            double range = high - low;
            double rand = Random.NextDouble();
            return range * rand + low;
        }

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

        public virtual int GenerateInt(int low = 0, int high = int.MaxValue / 100)
        {
            return Random.Next(low, high);
        }

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
        
        public virtual Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        public virtual bool GenerateBoolean()
        {
            return GenerateInt() % 2 == 0;
        }

        public virtual decimal GenerateDecimal(decimal low = 0m, decimal high = 10000000000m)
        {
            double value = GenerateDouble((double)low, (double)high);
            return Convert.ToDecimal(value);
        }
        public virtual DateTime GenerateDate(int daysAgoMinimum = -30, int daysFutureMaximum = 30)
        {
            return DateTime.Today.AddDays(GenerateInt(daysAgoMinimum, daysFutureMaximum));
        }


        public virtual DateTime GenerateDateTime(int daysAgoMinimum = -30, int daysFutureMaximum = 30)
        {
            return DateTime.Today.AddDays(GenerateInt(daysAgoMinimum, daysFutureMaximum))
                    .AddSeconds(GenerateDouble(0, 24 * 60 * 60))
                    .AddMilliseconds(GenerateDouble(0, 1000))
                    .AddMicroseconds(GenerateDouble(0, 1000))
                ;
        }

        
        public string GenerateFirstName()
        {
            return FirstNames[GenerateInt(0, FirstNames.Length)];
        }

        public string GenerateLastName()
        {
            return LastNames[GenerateInt(0, LastNames.Length)];
        }

        public string GenerateName()
        {
            return $"{GenerateFirstName()} {GenerateLastName()}";

        }

        #endregion Basic types

        public virtual decimal GenerateMoney(decimal low = 0m, decimal high = 10000000000m)
        {
            return Math.Floor(GenerateDecimal(low, high) * 100) / 100;
        }
        #endregion Generation Methods

        #region Configuration 

        public void AddStringFieldNameGenerator(string fieldName, Func<string> generator)
        {
            FieldNamesToGenerators[fieldName] = generator;
        }

        public void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod) where T : notnull
        {
            Type t = typeof(T);
            if (!MappedInitializeFunctions.ContainsKey(t))
                MappedInitializeFunctions.Add(t, () => initMethod(this));
            else
                MappedInitializeFunctions[t] = () => initMethod(this);
        }

        public void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod, bool autoProperty)
            where T : notnull, new()
        {
            AddCustomObjectInitialization(initMethod);
            if (autoProperty)
                AddAutoProperties<T>();
        }

        public void AddAutoProperties<T, T2>() where T2 : T, new()
        {
            Type t = typeof(T);
            Type t2 = typeof(T2);
            AutoProperties[t] = t2;
        }

        public void AddAutoProperties<T>() where T : new()
        {
            AddAutoProperties<T, T>();
        }

        #endregion Configuration 
        
        #region Instantiation Methods
        public virtual Dictionary<string, TValue> InitializeDictionary<TValue>(int numberItems)
            where TValue : notnull, new()
        {
            Dictionary<string, TValue> results = new();
            for (int x = 0; x < numberItems; x++)
            {
                results.Add(GenerateString(12), InitializeObject<TValue>());
            }

            return results;
        }

        public virtual Dictionary<TKey, TValue> InitializeDictionary<TKey, TValue>(int numberItems)
            where TKey : notnull, new()
            where TValue : notnull, new()
        {
            Dictionary<TKey, TValue> results = new();
            for (int x = 0; x < numberItems; x++)
            {
                results.Add(InitializeObject<TKey>(), InitializeObject<TValue>());
            }

            return results;
        }

        public virtual T InitializeObject<T>() where T : notnull, new()
        {
            return InitializeObject<T>(null);
        }

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

        public virtual IList<T> InitializeList<T>(int numberItems) where T : notnull, new()
        {
            List<T> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                list.Add(InitializeObject<T>());
            }

            return list;
        }

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

        public virtual IList<string> InitializeList(int numberItems, int maxLength)
        {
            List<string> list = new(numberItems);
            for (int x = 0; x < numberItems; x++)
            {
                list.Add(GenerateString(maxLength));
            }

            return list;
        }

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
                        $"Could not create instance of object of type '{objectType}.  Verify a default constructor exists");

                InitializeObject(newObject);
            }

            return newObject;
        }
        
        public virtual T InitializeObject<T>(T objectToInitialize) 
            where T : notnull 
        {
            return InitializeObject(objectToInitialize, MaxRecursion);
        }

        public virtual T InitializeObject<T>(T objectToInitialize, int maxRecursion) where T : notnull
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

        public virtual DateTime GetFirstDayOfMonth()
        {
            return GetFirstDayOfMonth(DateTime.Today);
        }
        public virtual DateTime GetFirstDayOfMonth(DateTime monthDate)
        {
            return new DateTime(monthDate.Year, monthDate.Month, 1);
        }

        public virtual DateTime GetLastDayOfMonth()
        {
            return GetLastDayOfMonth(DateTime.Today);
;        }
        public virtual DateTime GetLastDayOfMonth(DateTime monthDate)
        {
            return GetFirstDayOfMonth(monthDate).AddMonths(1).AddDays(-1);
        }
        #endregion Misc Methods

        #region Selection methods
        public T GenerateEnum<T>() where T : struct, Enum, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum");
            string[] names = Enum.GetNames(typeof(T));
            Enum.TryParse(GetEntry(names), out T result);
            return result;
        }

        public T CoinToss<T>(T heads, T tails)
        {
            return GenerateInt(0, 2) == 0 ? heads : tails;
        }

        public T GetListEntry<T>(List<T> list)
        {
            return list[GenerateInt(0, list.Count)];
        }

        public T GetEntry<T>(T[] list)
        {
            return list[GenerateInt(0, list.Length)];
        }
        #endregion Selection Methods
        
        #endregion IRandomDataGenerator Implementation

        #region Protected Methods
        protected virtual string? GenerateStringForFieldName(string fieldName)
        {
            return
                FieldNamesToGenerators.TryGetValue(fieldName, out Func<string>? generator)
                    ? generator.Invoke()
                    : null;
        }
        
        protected virtual void AssignValueToProperty<T>(T objectToInitialize, PropertyInfo propertyInfo,
            int maxRecursion)
        {
            Type propertyType = propertyInfo.PropertyType;

            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (underlyingType != null)
            {
                propertyType = underlyingType;
            }

            if (AutoProperties.TryGetValue(propertyType, out var mappedType))
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
                var precisionAttribute =
                    propertyInfo.GetCustomAttribute<DecimalPrecisionAttribute>();
                propertyInfo.SetValue(objectToInitialize,
                    precisionAttribute == null
                        ? GenerateMoney()
                        : GenerateDecimal(precisionAttribute.Precision, precisionAttribute.Scale));
            }
            else if (propertyType == typeof(double))
            {
                var precisionAttribute =
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
        #endregion Protected Methods

        #region private Methods

        private void SetProperty<T>(T objectToInitialize, PropertyInfo propertyInfo, Type mappedType, int maxRecursion)
        {
            
            object? newObject;
            if (MappedInitializeFunctions.TryGetValue(mappedType, out Func<object>? function))
                newObject = function();
            else
            {
                newObject = Activator.CreateInstance(mappedType);
                if (newObject == null)
                    throw new InvalidOperationException(
                        $"Could not create instance of object of type '{mappedType}.  Verify a default constructor exists");
                InitializeObject(newObject, maxRecursion);
            }

            propertyInfo.SetValue(objectToInitialize, newObject);
        }

        private void SetStringProperty<T>(T objectToInitialize, PropertyInfo propertyInfo)
        {
            var value = GenerateStringForFieldName(propertyInfo.Name);
            if (string.IsNullOrWhiteSpace(value))
            {
                int maxSize = GenerateInt(30, 50);

                var stringLengthAttribute =
                    propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                if (stringLengthAttribute != null)
                    maxSize = stringLengthAttribute.MaximumLength;
                var maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttribute != null)
                    maxSize = maxLengthAttribute.Length;
                value = GenerateString(maxSize);
            }

            propertyInfo.SetValue(objectToInitialize, value);
        }

        
        #endregion private Methods


    }

}
