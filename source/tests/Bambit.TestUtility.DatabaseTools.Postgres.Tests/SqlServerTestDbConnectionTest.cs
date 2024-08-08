using System.Data;
using System.Diagnostics;
using System.Reflection;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Bambit.TestUtility.DatabaseTools.Postgres.Tests;

[TestClass]
public class PostgreSqlTestDbConnectionTest
{
    protected class InvalidMappedClass : DatabaseMappedClass
    {
        public int X { get; set; }
    }
    protected IConfiguration Configuration { get; set; } = null!;
        
    protected DatabaseMappedClass GenerateMappedClass()
    {
        TestDatabaseFactoryOptions options = Configuration.GetSection("DatabaseFactory").Get<TestDatabaseFactoryOptions>()!;
            
        TableToClassBuilder builder = new(new TestDatabaseFactory(options));
        return builder.GenerateObjectFromTable("Test1", "test", "testTableNullable")!;

    }

    [TestInitialize]
    public void TestInitialize()
    {
        Configuration=new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .AddJsonFile("appSettings.local.json", true)
            .Build();
    }

    protected PostgreSqlTestDbConnection GetConnection()
    {
        IDbConnection dbConnection = new NpgsqlConnection( Configuration.GetConnectionString("IntegrationTestDatabase")!);

        PostgreSqlTestDbConnection connection = new(dbConnection );
        connection.MessageReceived += (_, s) => Trace.WriteLine(s);
        return connection;
    }
    [TestMethod]
    public void ExecuteScalar_ReturnsExpected()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        testConnection.ExecuteScalar<int>("select 1 + 3").Should().Be(4);
    }

    [TestMethod]
    public void Persist_MappedClassMissingAttributes_ThrowsException()
    {
        InvalidMappedClass test = new();
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        testConnection.Invoking(t => t.Persist(test))
            .Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Persist_FullObject_WritesToDatabase()
    {
        DatabaseMappedClass mappedClass = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass);
        PostgreSqlTestDbConnection testConnection = GetConnection();
            
        testConnection.Open();
        string purgeQuery = "delete from test.testTableNullable";
        testConnection.ExecuteQuery(purgeQuery);
            
        testConnection.Persist(mappedClass);
        string selectQuery = "select * from test.testTableNullable";
        using IDataReader executeReader = testConnection.ExecuteReader(selectQuery);
        executeReader.Read().Should().BeTrue("1 row should exist");
        int fieldCount = executeReader.FieldCount;
        for (int x = 0; x < fieldCount; x++)
        {
            object? testObject=executeReader.IsDBNull(x)?null:executeReader.GetValue(x);
            string name = executeReader.GetName(x);
                

            PropertyInfo propertyInfo = mappedClass.GetType().GetProperties().FirstOrDefault(p => p.Name == name)!;
            if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                continue;
            object? expectedObject = propertyInfo.GetValue(mappedClass);
                
                

            testObject.Should().BeEquivalentTo(expectedObject,
                $"Field {name} value of '{testObject}' should be '{expectedObject}");
        }
    }
        
    [TestMethod]
    public void Persist_ConnectionNotOpen_OpensConnection()
    {
            
        TestDatabaseFactoryOptions options = Configuration.GetSection("DatabaseFactory").Get<TestDatabaseFactoryOptions>()!;
            
        TableToClassBuilder builder = new(new TestDatabaseFactory(options));
        DatabaseMappedClass mappedClass = builder.GenerateObjectFromTable("Test1", "test", "testTableNullable")!;
        RandomDataGenerator.Instance.InitializeObject(mappedClass);
        PostgreSqlTestDbConnection testConnection = GetConnection();
            
            
        testConnection.Persist(mappedClass);
        string selectQuery = $"select * from test.testTableNullable where uniqueidentifierField='{mappedClass.GetValue<Guid>("uniqueidentifierField")}' ";
        using IDataReader executeReader = testConnection.ExecuteReader(selectQuery);
        executeReader.Read().Should().BeTrue("1 row should exist");
        int fieldCount = executeReader.FieldCount;
        for (int x = 0; x < fieldCount; x++)
        {
            object? testObject=executeReader.IsDBNull(x)?null:executeReader.GetValue(x);
            string name = executeReader.GetName(x);
                

            PropertyInfo propertyInfo = mappedClass.GetType().GetProperties().FirstOrDefault(p => p.Name == name)!;
            if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                continue;
            object? expectedObject = propertyInfo.GetValue(mappedClass);
                
                

            testObject.Should().BeEquivalentTo(expectedObject,
                $"Field {name} value of '{testObject}' should be '{expectedObject}");
        }
    }
    [TestMethod]
    public void CompareResults_EmptyExpectedAndActual_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        object?[][] rows = [];
        testConnection.CompareResults(["A", "B"], ["varchar", "varchar"],
            rows, rows).IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void CompareResults_DoNotAllowUnexpectedRows_ValuesMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] rows = [["Alpha", "Beta"], ["Alpha1", "Beta2"], ["Alpha2", "Beta1"]];
        testConnection.CompareResults(["A", "B"], ["varchar", "varchar"],
            rows, rows).IsSuccess.Should().BeTrue();
    }


    [TestMethod]
    public void CompareResults_NullValuesMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        object?[][] expected = [["Alpha", null]];
        object?[][] actual = [["Alpha", null]];
        testConnection.CompareResults(["A", "B"], ["varchar", "date"],
            expected, actual).IsSuccess.Should().BeTrue();
    }


    [TestMethod]
    public void CompareResults_DateField_ValuesMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        DateTime date = RandomDataGenerator.Instance.GenerateDate();
        object?[][] expected = [["Alpha", date]];
        object?[][] actual = [["Alpha", date]];
        testConnection.CompareResults(["A", "B"], ["varchar", "date"],
            expected, actual).IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void CompareResults_BitField_ValuesMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        object?[][] expected = [["Alpha", true], ["Beta", false]];
        object?[][] actual = [["Alpha", true], ["Beta", false]];
        testConnection.CompareResults(["A", "B"], ["varchar", "bit"],
            expected, actual).IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void CompareResults_DoNotAllowUnexpectedRows_ExpectedHasMoreRows_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] expected = [["Alpha", "Beta"], ["Alpha1", "Beta2"], ["Alpha2", "Beta1"]];
        string?[][] actual= [["Alpha", "Beta"], ["Alpha1", "Beta2"]];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) = testConnection.CompareResults(
            ["A", "B"], ["varchar", "varchar"],
            expected, actual);
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(1);
        numberRowsNotExpected.Should().Be(0);
    }

        
    [TestMethod]
    public void CompareResults_DataMismatch_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] expected = [["Alpha", "Beta"], ["Alpha2", "Beta2"], ["Alpha1", "Beta1"]];
        string?[][] actual= [["Alpha", "Beta"], ["Alpha1", "Beta2"], ["Alpha2", "Beta1"]];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) = testConnection.CompareResults(["A", "B"], ["varchar", "varchar"],
            expected, actual);
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(2);
        numberRowsNotExpected.Should().Be(2);
    }
    [TestMethod]
    public void CompareResults_DoNotAllowUnexpectedRows_ExpectedHasFewerRows_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] expected = [
            ["Alpha", "Beta"], 
            ["Alpha1", "Beta2"]
        ];
        string?[][] actual = [
            ["Alpha", "Beta"], 
            ["Alpha1", "Beta2"], 
            ["Alpha2", "Beta1"]
        ];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) = testConnection.CompareResults(
            ["A", "B"], ["varchar", "varchar"],
            expected, actual);
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(0);
        numberRowsNotExpected.Should().Be(1);
    }
    [TestMethod]
    public void CompareResults_AllowUnexpectedRows_ExpectedHasFewerRows_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] expected = [
            ["Alpha", "Beta"], 
            ["Alpha1", "Beta2"]
        ];
        string?[][] actual = [
            ["Alpha", "Beta"], 
            ["Alpha1", "Beta2"], 
            ["Alpha2", "Beta1"]
        ];
        testConnection.CompareResults(["A", "B"], ["varchar", "varchar"],
            expected, actual, true).IsSuccess.Should().BeTrue();
    }

        
    [TestMethod]
    public void CompareResults_ActualMissingColumns_ThrowsException()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
        string?[][] expected = [
            ["Alpha", "Beta"], 
            ["Alpha1", "Beta2"]
        ];
        string?[][] actual = [
            ["Alpha", "Beta"], 
            ["Alpha1"], 
            ["Alpha2", "Beta1"]
        ];
        testConnection.Invoking(t => t.CompareResults(["A", "B"], ["varchar", "varchar"],
            expected, actual, true)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void CompareTableToDataset_NoRecordsInEither_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";
        testConnection.ExecuteQuery(purgeQuery);
        testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], []).IsSuccess.Should().BeTrue();
    }


    [TestMethod]
    public void CompareTableToDataset_RecordsMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();

        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [[mappedClass2.GetValue<long>("bigIntField")], [mappedClass1.GetValue<long>("bigIntField")]];
        testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected).IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void CompareTableToDataset_RecordsDoNotMatch_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();

        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [RandomDataGenerator.Instance.GenerateInt()] ,
            [RandomDataGenerator.Instance.GenerateInt()] ];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) =
            testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected);
                
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(2);
        numberRowsNotExpected.Should().Be(2);
    }

        
    [TestMethod]
    public void CompareTableToDataset_ActualHasMoreRows_AllowUnexpectedFalse_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [mappedClass2.GetValue<long>("bigIntField")]
            
        ];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) =
            testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected);
                
            
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(0);
        numberRowsNotExpected.Should().Be(1);
    }
        
    [TestMethod]
    public void CompareTableToDataset_MissingColumns_ThrowsException()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [mappedClass2.GetValue<long>("bigIntField"),1],
            [mappedClass1.GetValue<long>("bigIntField"),2]
            
        ];
        testConnection.Invoking(t=>t .CompareTableToDataset("test", "testTableNullable", ["bigIntField", "Purple"], expected)).Should().Throw<NpgsqlException>();
    }
    [TestMethod]
    public void CompareTableToDataset_ActualHasFewerRows_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [mappedClass2.GetValue<long>("bigintfield")],
            [mappedClass1.GetValue<long>("bigintfield")]
            
        ];
        (bool isSuccess, int numberRowsMissing, int numberRowsNotExpected) =
            testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected);
            
        isSuccess.Should().BeFalse();
        numberRowsMissing.Should().Be(1);
        numberRowsNotExpected.Should().Be(0);
    }
        
    [TestMethod]
    public void CompareTableToDataset_ActualHasMoreRows_AllowUnexpectedTrue_ReturnsSuccess()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [mappedClass2.GetValue<long>("bigIntField")]
            
        ];
        testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected,
            true).IsSuccess.Should().BeTrue();
    }
    [TestMethod]
    public void CompareTableToDataset_EmptyData_ReturnsError()
    {
        PostgreSqlTestDbConnection testConnection = GetConnection();
        testConnection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";

        testConnection.ExecuteQuery(purgeQuery);
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        DatabaseMappedClass mappedClass2 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        RandomDataGenerator.Instance.InitializeObject(mappedClass2);
        testConnection.Persist(mappedClass2);
        testConnection.Persist(mappedClass1);
        object?[][] expected = [
            [mappedClass2.GetValue<long>("bigIntField")]
            
        ];
        testConnection.CompareTableToDataset("test", "testTableNullable", ["bigIntField"], expected,
            true).IsSuccess.Should().BeTrue();
    }

    #region Base members


    [TestMethod]
    public void ConnectionString_Gets_ReturnsExpected()
    {
        GetConnection().ConnectionString.Should().Be(Configuration.GetConnectionString("IntegrationTestDatabase"));
    }

    [TestMethod]
    public void Database_Gets_ReturnsExpected()
    {
        GetConnection().Database.Should().Be(Configuration.GetValue<string>("Settings:PrimaryDatabaseName")!);
    }

    [TestMethod]
    public void ConnectionTimeout_Gets_ReturnsExpected()
    {
        GetConnection().ConnectionTimeout.Should().Be(Configuration.GetValue<int>("Settings:ConnectionTimeout")!);
    }

    [TestMethod]
    public void ConnectionString_Set_AdjustValue()
    {
        string testString = "Server=localhost; Database=Sample2";
        PostgreSqlTestDbConnection sqlServerTestDbConnection = GetConnection();
        sqlServerTestDbConnection.ConnectionString=testString ;
        sqlServerTestDbConnection.ConnectionString.Should().Be(testString);
    }
        


    [TestMethod]
    public void Close_ClosesOpenConnection()
    {
        PostgreSqlTestDbConnection connection = GetConnection();
        connection.Open();
        connection.State.Should().Be(ConnectionState.Open);
        connection.Close();
        connection.State.Should().Be(ConnectionState.Closed);
    }
        
    [TestMethod]
    public void ChangeDatabase_AssignsNewDatabase()
    {
        string value = Configuration.GetValue<string>("Settings:AlternativeDatabaseName")!;
        PostgreSqlTestDbConnection connection = GetConnection();
        connection.Open();
        connection.ChangeDatabase(value);
        connection.ExecuteScalar<string>("SELECT current_database();").Should().BeEquivalentTo(value);
            
    }
        
    [TestMethod]
    public void Transactions_Rollback()
    {
        _ = Configuration.GetValue<string>("Settings:AlternativeDatabaseName")!;
        PostgreSqlTestDbConnection connection = GetConnection();
        connection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";
            
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        connection.ExecuteQuery(purgeQuery);
        connection.Persist(mappedClass1);

        using IDbTransaction transaction = connection.BeginTransaction();
        IDbCommand command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = purgeQuery;
        command.ExecuteNonQuery();
        command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = "select count(1) from test.testTableNullable";
        command.ExecuteScalar().Should().Be(0);
        transaction.Rollback();
        connection.ExecuteScalar<long>("select count(1) from test.testTableNullable").Should().Be(1);
            
    }
        
        
    [TestMethod]
    public void Transactions_Commit()
    {
        _ = Configuration.GetValue<string>("Settings:AlternativeDatabaseName")!;
        PostgreSqlTestDbConnection connection = GetConnection();
        connection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";
            
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        connection.ExecuteQuery(purgeQuery);
        connection.Persist(mappedClass1);

        using IDbTransaction transaction = connection.BeginTransaction();
        IDbCommand command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = purgeQuery;
        command.ExecuteNonQuery();
        command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = "select count(1) from test.testTableNullable";
        command.ExecuteScalar().Should().Be(0);
        transaction.Commit();
        connection.ExecuteScalar<long>("select count(1) from test.testTableNullable").Should().Be(0);
            
    }
        
        
    [TestMethod]
    public void Transactions_PassIsolationLevel_SetsIsolationLevel()
    {
        _ = Configuration.GetValue<string>("Settings:AlternativeDatabaseName")!;
        PostgreSqlTestDbConnection connection = GetConnection();
        connection.Open();
            
        string purgeQuery = "delete from test.testTableNullable";
            
        DatabaseMappedClass mappedClass1 = GenerateMappedClass();
        RandomDataGenerator.Instance.InitializeObject(mappedClass1);
        connection.ExecuteQuery(purgeQuery);
        connection.Persist(mappedClass1);

        using IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
        IDbCommand command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = purgeQuery;
        command.ExecuteNonQuery();
        command = connection.CreateCommand();    
        command.Transaction=transaction;
        command.CommandText = "select count(1) from test.testTableNullable";
        command.ExecuteScalar().Should().Be(0);
        transaction.Rollback();
        connection.ExecuteScalar<long>("select count(1) from test.testTableNullable").Should().Be(1);
            
    }
        

    #endregion Base members
}