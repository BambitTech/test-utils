using System;
using System.Collections.Generic;


namespace Bambit.TestUtility.DataGeneration
{
    /// <summary>
    /// Provides methods to create random data for testing against.
    /// </summary>
    public interface IRandomDataGenerator
    {
        
        #region Configuration Methods
        /// <summary>
        /// Adds a type, <see cref="T"/>, that will be instantiated by type <see cref="T2"/> of if it is a property of an object that is being instantiated.
        /// </summary>
        /// <typeparam name="T">The type that, if a property of an object is one, will be instantiated</typeparam>
        /// <typeparam name="T2">The type to used to instantiate <see cref="T"/></typeparam>
        void AddAutoProperties<T, T2>() where T2 : T, new();

        /// <summary>
        /// Adds a <see cref="Func{IRandomDataGenerator, TResult}"/> that will be called to create a new object of type <see cref="T"/> 
        /// </summary>
        /// <remarks>When <see cref="CreateObject"/>, <see cref="InitializeList{T}(int)"/> or <see cref="InitializeObject{T}()"/> is called, this method instead of just
        /// using a CTOR</remarks>
        /// <summary>
        /// Adds a type, <see cref="T"/>, that will be instantiated if it is a property of an object that is being instantiated.
        /// </summary>
        /// <typeparam name="T">The type that, if a property of an object is one, will be instantiated</typeparam>
        void AddAutoProperties<T>() where T : new();
    
        void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod)
#if NET6_0_OR_GREATER
            where T : notnull
#endif
            ;

        
        /// <summary>
        /// Adds a <see cref="Func{IRandomDataGenerator, TResult}"/> that will be called to create a new object of type <see cref="T"/> 
        /// </summary>
        /// <typeparam name="T">Tne type to instantiate</typeparam>
        /// <param name="autoProperty">Whether to auto initialize properties of this type.</param>
        /// <param name="initMethod">The method to use when instantiating an object of type <see cref="T"/> </param>
        /// <remarks>   </remarks>
        void AddCustomObjectInitialization<T>(Func<IRandomDataGenerator, T> initMethod, bool autoProperty)  where T:
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();
 
        #endregion Configuration Methods
  
        #region Generation Methods
        /// <summary>
        /// Generates a boolean value
        /// </summary>
        /// <returns></returns>
        bool GenerateBoolean();

        /// <summary>
        /// Generates a random DateTime between <see cref="daysAgoMinimum"/> and <see cref="daysFutureMaximum"/>
        /// </summary>
        /// <param name="daysAgoMinimum">The minimum days from today for which to generate DateTime. Negative numbers represent the past.  By default, this is -30</param>
        /// <param name="daysFutureMaximum">The maximum days from today for which to generate a DateTime. Negative numbers represent the past.  By default, this is 30</param>
        /// <returns>A random date between <see cref="daysAgoMinimum"/> and <see cref="daysFutureMaximum"/> days from today</returns>
        /// <remarks>
        /// The returned DateTime object will always be at midnight.
        /// </remarks>
        DateTime GenerateDate(int daysAgoMinimum = -30, int daysFutureMaximum = 30);

        /// <summary>
        /// Generates a random DateTime between <see cref="daysAgoMinimum"/> and <see cref="daysFutureMaximum"/>
        /// </summary>
        /// <param name="daysAgoMinimum">The minimum days from today for which to generate DateTime. Negative numbers represent the past.  By default, this is -30</param>
        /// <param name="daysFutureMaximum">The maximum days from today for which to generate a DateTime. Negative numbers represent the past.  By default, this is 30</param>
        /// <returns>A random date between <see cref="daysAgoMinimum"/> and <see cref="daysFutureMaximum"/> days from today</returns>
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
        /// <param name="T">The Type of enum to generate.  
        /// Random <see cref="T"/> enum
        /// </param>
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
        /// <exception cref="InvalidOperationException">The <see cref="objectType"/> did not have a parameterless constructor</exception>
        object CreateObject(Type objectType);

        /// <summary>
        /// Initializes a new Dictionary with string keys and <see cref="TValue"/> values
        /// </summary>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="numberItems">The number of random entries to create</param>
        /// <returns>A new Dictionary{String,TValue} with <see cref="numberItems"/> entries</returns>
        Dictionary<string, TValue> InitializeDictionary<TValue>(int numberItems)
            where TValue :
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();

        /// <summary>
        /// Initializes a new Dictionary with <see cref="TKey"/> key sand <see cref="TValue"/> values
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="numberItems">The number of random entries to create</param>
        /// <returns>A new Dictionary{String,TValue} with <see cref="numberItems"/> entries</returns>
        Dictionary<TKey, TValue> InitializeDictionary<TKey, TValue>(int numberItems)
            where TKey : 
#if NET6_0_OR_GREATER
            notnull, 
#endif
             new()
            where TValue : 
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();

        /// <summary>
        /// Initializes a random list of  <see cref="numberItems"/> strings, each with a length no greater than <see cref="maxLength"/>
        /// </summary>
        /// <param name="numberItems">The number of strings to generate</param>
        /// <param name="maxLength">The maximum length of each string</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="numberItems"/> strings</returns>
        IList<string> InitializeList(int numberItems, int maxLength);

        /// <summary>
        /// Creates a list of random object of type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The type of objects to generate</typeparam>
        /// <param name="numberItems">The number of items to generate</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="numberItems"/> object of type <see cref="T"/></returns>
        IList<T> InitializeList<T>(int numberItems) where T : 
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();

        /// <summary>
        /// Creates a list of random object of type <see cref="T"/>, calling <see cref="postCreate"/> on each before returning
        /// </summary>
        /// <typeparam name="T">The type of objects to generate</typeparam>
        /// <param name="postCreate">The <see cref="Action{T}"/> to call on each entity before returning.</param>
        /// <param name="numberItems">The number of items to generate</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="numberItems"/> object of type <see cref="T"/></returns>
        IList<T> InitializeList<T>(int numberItems, Action<T> postCreate) where T : 
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();

        /// <summary>
        /// Creates and initializes an object of type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The type of object to generate.  It must have a parameterless constructor</typeparam>
        /// <returns>A new object of type <see cref="T"/></returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddAutoProperties{T,T2}">AddAutoProperties</see></remarks>
        T InitializeObject<T>() where T : 
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();

        /// <summary>
        /// Creates and initializes an object of type <see cref="T"/>, calling the supplied <see cref="modifierFunction"/> on it before returning.
        /// </summary>
        /// <typeparam name="T">The type of object to generate.  It must have a parameterless constructor</typeparam>
        /// <param name="modifierFunction">An <see cref="Action{T}"/> that will be called on the generated object.</param>
        /// <returns>A new object of type <see cref="T"/></returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddAutoProperties{T,T2}">AddAutoProperties</see></remarks>
        T InitializeObject<T>(
                Action<T> modifierFunction
            ) where T :
#if NET6_0_OR_GREATER
            notnull, 
#endif
            new();


        /// <summary>
        /// Populates the properties of the supplied object.
        /// </summary>
        /// <typeparam name="T">The type of object that will be initialized.</typeparam>
        /// <param name="objectToInitialize">The object which to have its properties initialize</param>
        /// <returns>The supplied <see cref="objectToInitialize"/> with properties initialized</returns>
        /// <remarks>By default, simple value type properties (int, decimal, string, etc.) will be initialized with random values.  Object type properties
        /// will not be initialized unless a function has been added with
        /// <see cref="AddCustomObjectInitialization{T}(System.Func{Bambit.TestUtility.DataGeneration.IRandomDataGenerator,T})">AddCustomObjectInitialization</see></remarks>
        T InitializeObject<T>(T objectToInitialize)  
#if NET6_0_OR_GREATER
            where T : notnull 
#endif
        ;

        T InitializeObject<T>(T objectToInitialize, Action<T> modifierFunction)   
#if NET6_0_OR_GREATER
            where T : notnull 
#endif
        ;
        #endregion Instantiation Methods

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