using System;
using System.Collections.Generic;
// ReSharper disable UnusedMemberInSuper.Global


namespace Bambit.TestUtility.DataGeneration
{
    /// <summary>
    /// Provides methods to create random data for testing against.
    /// </summary>
    public interface IRandomDataGenerator
    {
        
        #region Configuration Methods
        /// <summary>
        /// Adds a type, t, that will be instantiated by type T2 of if it is a property of an object that is being instantiated.
        /// </summary>
        /// <typeparam name="T">The type that, if a property of an object is one, will be instantiated</typeparam>
        /// <typeparam name="T2">The type to used to instantiate t</typeparam>
        void AddAutoProperties<T, T2>() 
            where T2 : T, new();

        /// <summary>
        /// Adds a <see cref="Func{IRandomDataGenerator, TResult}"/> that will be called to create a new object of type t 
        /// </summary>
        /// <remarks>When <see cref="CreateObject"/>, <see cref="InitializeList{T}(int)"/> or <see cref="InitializeObject{T}()"/> is called, this method instead of just
        /// using a CTOR</remarks>
        /// <summary>
        /// Adds a type, t, that will be instantiated if it is a property of an object that is being instantiated.
        /// </summary>
        /// <typeparam name="T">The type that, if a property of an object is one, will be instantiated</typeparam>
        void AddAutoProperties<T>() 
            where T : new();
    
        /// <summary>
        /// Defines a function that will be used to initialize a specific object type
        /// </summary>
        /// <typeparam name="T">The type of object that will use the generated function</typeparam>
        /// <param name="initMethod">The <see cref="Func{T,TResult}"/> that will be used to initialize the object</param>
        void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod)
            where T : notnull;

        
        /// <summary>
        /// Adds a <see cref="Func{IRandomDataGenerator, TResult}"/> that will be called to create a new object of type t 
        /// </summary>
        /// <typeparam name="T">Tne type to instantiate</typeparam>
        /// <param name="autoProperty">Whether to auto initialize properties of this type.</param>
        /// <param name="initMethod">The method to use when instantiating an object of type t </param>
        /// <remarks>   </remarks>
        void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod, bool autoProperty)  where T:
            notnull, new();
 
        #endregion Configuration Methods
  
        #region Generation Methods
        /// <summary>
        /// Generates a boolean value
        /// </summary>
        /// <returns></returns>
        bool GenerateBoolean();
        
        /// <summary>
        /// Generates a random <see cref="Byte"/> value
        /// </summary>
        /// <returns>A random <see cref="Byte"/></returns>
        byte GenerateByte();

        /// <summary>
        /// Generates a random DateTime between daysAgoMinimum and daysFutureMaximum
        /// </summary>
        /// <param name="daysAgoMinimum">The minimum days from today for which to generate DateTime. Negative numbers represent the past.  By default, this is -30</param>
        /// <param name="daysFutureMaximum">The maximum days from today for which to generate a DateTime. Negative numbers represent the past.  By default, this is 30</param>
        /// <returns>A random date between daysAgoMinimum and daysFutureMaximum days from today</returns>
        /// <remarks>
        /// The returned DateTime object will always be at midnight.
        /// </remarks>
        DateTime GenerateDate(int daysAgoMinimum = -30, int daysFutureMaximum = 30);

        /// <summary>
        /// Generates a random DateTime between daysAgoMinimum and daysFutureMaximum
        /// </summary>
        /// <param name="daysAgoMinimum">The minimum days from today for which to generate DateTime. Negative numbers represent the past.  By default, this is -30</param>
        /// <param name="daysFutureMaximum">The maximum days from today for which to generate a DateTime. Negative numbers represent the past.  By default, this is 30</param>
        /// <returns>A random date between daysAgoMinimum and daysFutureMaximum days from today</returns>
        /// <remarks>
        /// Similar to <see cref="GenerateDate"/> except the time value is randomly generated.
        /// </remarks>
        DateTime GenerateDateTime(int daysAgoMinimum = -30, int daysFutureMaximum = 30);

        /// <summary>
        /// Generates a random Decimal with a given precision and scale
        /// </summary>
        /// <param name="precision">The precision (number of significant figures) to generate</param>
        /// <param name="scale">The magnitude of the resultant decimal</param>
        /// <returns>A randomly generated Decimal</returns>
        decimal GenerateDecimal(byte precision, byte scale);

        /// <summary>
        /// Generates a random Decimal within a given range.
        /// </summary>
        /// <param name="low">The minimum value to generate</param>
        /// <param name="high">The maximum value to generate</param>
        /// <returns>A randomly generated Decimal</returns>
        decimal GenerateDecimal(decimal low = 0, decimal high = 1000000000000);

        /// <summary>
        /// Generates a random Decimal within a given range.
        /// </summary>
        /// <param name="low">The minimum value to generate</param>
        /// <param name="high">The maximum value to generate</param>
        /// <returns>A randomly generated Decimal</returns>
        double GenerateDouble(double low = 0, double high = double.MaxValue);

        /// <summary>
        /// Generates a random Decimal with a given precision and scale
        /// </summary>
        /// <param name="precision">The precision (number of significant figures) to generate</param>
        /// <param name="scale">The magnitude of the resultant decimal</param>
        /// <returns>A randomly generated Decimal</returns>
        double GenerateDouble(byte precision, byte scale);

        /// <summary>
        /// Generates a random enum value fo the specified type
        /// </summary>
        /// <typeparam name="T">The Type of enum to generate.  
        /// </typeparam>>
        /// <returns>A random value of the supplied Enum</returns>
        T GenerateEnum<T>() where T : struct,Enum, IConvertible ;

        /// <summary>
        /// Generates a random first name from a predefined list of first names.
        /// </summary>
        /// <returns>A random string name from a predefined list.</returns>
        string GenerateFirstName();

        /// <summary>
        /// Generates a randomGuid
        /// </summary>
        /// <returns>A randomGuid</returns>
        Guid GenerateGuid();
        
        /// <summary>
        /// Generates a random integer between the low and high values.
        /// </summary>
        /// <param name="low">The minimum value to generates.  By default, this is 0</param>
        /// <param name="high">The maximum value to generate.  By default, this is equal to <see cref="Int16.MaxValue">Int.MaxValue</see></param>
        /// <returns>A randomly generated integer</returns>
        int GenerateInt(int low = 0, int high = int.MaxValue);

        /// <summary>
        /// Generated a random last name from a predefined list of last names.
        /// </summary>
        /// <returns>A random string from a predefined list of last names</returns>
        string GenerateLastName();
        
        /// <summary>
        /// Generates a random decimal with 2 decimals of precision.
        /// </summary>
        /// <param name="low">The minimum value to generate. By default, this is 0</param>
        /// <param name="high">The maximum value to generates.  By default, this is 1000000000000</param>
        /// <returns>A random decimal with 2 decimals of precision</returns>
        decimal GenerateMoney(decimal low = 0, decimal high = 1000000000000);
        
        /// <summary>
        /// Generates a random string of the specified length
        /// </summary>
        /// <param name="length">The length of the string to generate</param>
        /// <returns>A random string of characters.</returns>
        string GenerateString(int length);

        /// <summary>
        /// Generates a string containing a random First Name, a space and a last name.  The first and last names are selected randomly from a pre-defined list
        /// </summary>
        /// <returns>A random string in the form of "{FirstName} {LastMame}"</returns>
        string GenerateName();

        #endregion Generation Methods

        #region Instantiation Methods
        /// <summary>
        /// Creates a new object of a specific type
        /// </summary>
        /// <param name="objectType">The Type of object to generate.  The type must have a
        /// parameterless constructor</param>
        /// <returns>A newly created object</returns>
        /// <exception cref="InvalidOperationException">The objectType did not have a parameterless constructor</exception>
        object CreateObject(Type objectType);

        /// <summary>
        /// Initializes a new Dictionary with string keys and TValue values
        /// </summary>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="numberItems">The number of random entries to create</param>
        /// <returns>A new Dictionary{String,TValue} with numberItems entries</returns>
        Dictionary<string, TValue> InitializeDictionary<TValue>(int numberItems)
            where TValue : notnull, new();

        /// <summary>
        /// Initializes a new Dictionary with TKey key sand TValue values
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="numberItems">The number of random entries to create</param>
        /// <returns>A new Dictionary{String,TValue} with numberItems entries</returns>
        Dictionary<TKey, TValue> InitializeDictionary<TKey, TValue>(int numberItems)
            where TKey : notnull, new()
            where TValue : notnull, new();

        /// <summary>
        /// Initializes a random list of  numberItems strings, each with a length no greater than maxLength
        /// </summary>
        /// <param name="numberItems">The number of strings to generate</param>
        /// <param name="maxLength">The maximum length of each string</param>
        /// <returns>A <see cref="IList{T}"/> of numberItems strings</returns>
        IList<string> InitializeList(int numberItems, int maxLength);

        /// <summary>
        /// Creates a list of random object of type t
        /// </summary>
        /// <typeparam name="T">The type of objects to generate</typeparam>
        /// <param name="numberItems">The number of items to generate</param>
        /// <returns>A <see cref="IList{T}"/> of numberItems object of type t</returns>
        IList<T> InitializeList<T>(int numberItems) 
            where T : notnull, new();

        /// <summary>
        /// Creates a list of random objects of type T, calling an initialize function on each as they are generated
        /// </summary>
        /// <typeparam name="T">Type of object to initialize</typeparam>
        /// <param name="numberItems">The number of items to initialize</param>
        /// <param name="initializeFunction">The Function to call on each item after initialization</param>
        /// <returns>A <see cref="IList{T}"/> of numberItems object of type t</returns>
        List<T> InitializeList<T>(int numberItems, Func<IRandomDataGenerator, T> initializeFunction)
            where T : new();

        /// <summary>
        /// Creates a list of random object of type t, calling postCreate on each before returning
        /// </summary>
        /// <typeparam name="T">The type of objects to generate</typeparam>
        /// <param name="postCreate">The <see cref="Action{T}"/> to call on each entity before returning.</param>
        /// <param name="numberItems">The number of items to generate</param>
        /// <returns>A <see cref="IList{T}"/> of numberItems object of type t</returns>
        IList<T> InitializeList<T>(int numberItems, Action<T> postCreate) 
            where T : notnull, new();

        /// <summary>
        /// Creates and initializes an object of type t
        /// </summary>
        /// <typeparam name="T">The type of object to generate.  It must have a parameterless constructor</typeparam>
        /// <returns>A new object of type t</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddAutoProperties{T,T2}">AddAutoProperties</see></remarks>
        T InitializeObject<T>() 
            where T : notnull, new();

        /// <summary>
        /// Creates and initializes an object of type t, calling the supplied modifierFunction on it before returning.
        /// </summary>
        /// <typeparam name="T">The type of object to generate.  It must have a parameterless constructor</typeparam>
        /// <param name="modifierFunction">An <see cref="Action{T}"/> that will be called on the generated object.</param>
        /// <returns>A new object of type t</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddAutoProperties{T,T2}">AddAutoProperties</see></remarks>
        T InitializeObject<T>(
                Action<T> modifierFunction
            ) 
            where T : notnull, new();


        /// <summary>
        /// Populates the properties of the supplied object.
        /// </summary>
        /// <typeparam name="T">The type of object that will be initialized.</typeparam>
        /// <param name="objectToInitialize">The object which to have its properties initialize</param>
        /// <returns>The supplied objectToInitialize with properties initialized</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddCustomObjectInitialization{T}(Func{IRandomDataGenerator,T})">AddCustomObjectInitialization</see></remarks>
        T InitializeObject<T>(T objectToInitialize) 
            where T : notnull;

        /// <summary>
        /// Populates the properties of the supplied object, calling modifierFunction on each populated object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToInitialize">The object to initialize</param>
        /// <param name="modifierFunction">An <see cref="Action{T}"> </see></param>
        /// <returns>The supplied objectToInitialize with fields set randomly.</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddCustomObjectInitialization{T}(Func{IRandomDataGenerator,T})">AddCustomObjectInitialization</see></remarks>
        T InitializeObject<T>(T objectToInitialize, Action<T> modifierFunction)   
            where T : notnull;
        #endregion Instantiation Methods

        #region Misc Methods

        /// <summary>
        /// Gets the first day of the supplied month.
        /// </summary>
        /// <param name="monthDate">The date to retrieve the first date for</param>
        /// <returns>A <see cref="DateTime"/> with the <see cref="DateTime.Day"/> value being 1 and other values matching the supplied monthDate</returns>
        DateTime GetFirstDayOfMonth(DateTime monthDate);
        /// <summary>
        ///  Gets the first day of the current month
        /// </summary>
        /// <returns>A <see cref="DateTime"/> with the <see cref="DateTime.Day"/> value being 1 and other values matching <see cref="DateTime.Today"/></returns>
        DateTime GetFirstDayOfMonth();

        /// <summary>
        /// Gets the last day of the supplied month.
        /// </summary>
        /// <param name="monthDate">The date to retrieve the last date for</param>
        /// <returns>A <see cref="DateTime"/> with the <see cref="DateTime.Day"/> value being the last day of the supplied monthDate, and other value matches</returns>
        DateTime GetLastDayOfMonth(DateTime monthDate );
        /// <summary>
        ///  Gets the last day of the current month
        /// </summary>
        /// <returns>A <see cref="DateTime"/> with the <see cref="DateTime.Day"/> value being the last day of the supplied <see cref="DateTime.Today"/></returns>
        DateTime GetLastDayOfMonth();

        #endregion Misc Methods
        #region Selection methods

        /// <summary>
        /// Returns one of the two values randomly
        /// </summary>
        /// <typeparam name="T">The Type  of value that will be returned</typeparam>
        /// <param name="heads">First possible value</param>
        /// <param name="tails">Second possible value</param>
        /// <returns></returns>
        T CoinToss<T>(T heads, T tails);

        /// <summary>
        /// Geta a random item from an array of items
        /// </summary>
        /// <typeparam name="T">The type of items in the array</typeparam>
        /// <param name="list">The array of items from which to select</param>
        /// <returns>A random item from the supplied array</returns>
        T GetEntry<T>(T[] list);

        /// <summary>
        /// Geta a random item from a list of items
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="list">The list of items from which to select</param>
        /// <returns>A random item from the supplied list</returns>
        T GetListEntry<T>(List<T> list);

        #endregion Selection methods


    }
}