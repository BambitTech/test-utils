# Bambit.TestUtility.DataGeneration

Provides methods to automatically generated random data and populate objects.

## Getting started

Install:
```shell
PM> Install-Package Bambit.TestUtility.DataGeneration

```

## Usage
The class `RandomDataGenerator` provides a static instance that can be used for all calls.
All implemenation methods of `IRandomDataGenerator` are implemented as `virtual` to allow for extending the functionality.
### Generating Basic Types

```csharp
// Generate string with 10 characters
string string10Characters = RandomDataGenerator.Instance.GenerateString(10);
// Generate string with 20 characters
string string10Characters = RandomDataGenerator.Instance.GenerateString(20);

// Generate an int between 1 and 100
int randomInt = RandomDataGenerator.Instance.GenerateString(0,100);

// Generate a first name
string firstName =  RandomDataGenerator.Instance.GenerateFirstName();

```
For more types that can be generated, see `IRandomDataGenerator`

### Instaniating Objects

```csharp
// Populate properties on existing object
TestClass testClass=new();
RandomDataGenerator.Instance.InitializeObject(testClass);

// Create new object and initialize properties
TestClass testClass= RandomDataGenerator.Instance.InitializeObject<TestClass>();

// Call method after initializing an object
Person testClass= RandomDataGenerator.Instance.InitializeObject<Person>((generatedObject)=>{
   generatedObject.FirstName = "Freddy";
   generatedObject.LastName = "Kruger"
});

// Initialize a colleciton of objets, this creates a list of 4 new objects
IList<Person> initializeList = RandomDataGenerator.Instance.InitializeList<Person>(4);

```

## Additional documentation

Coming Soon!

## Feedback
  * [Github](https://github.com/bambit/test-utils/issues)
