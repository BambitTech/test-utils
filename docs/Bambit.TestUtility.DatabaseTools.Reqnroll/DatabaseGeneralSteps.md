###  <a name="SetCurrentDatabase" ></a> Given I Am Working In The connectionName Database
Patterns:
```regex
Given I am working in the (?:<connectionName>.*) database
```

Parameters:

|Name | Type |
|---|---|
|connectionName|string|

Set the current database connection name.
This value will be used for any future call that does not explicitly specify a `connectionName`
See [connections](overview.md#connections) for more information