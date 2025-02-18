###  <a name="defineValibleFromQuery" ></a> Given I have a <date|string> variable named '$<variableName>' defined from:
Patterns:
```regex
I have a (?<variableType>string|date) variable named '(?<variableName>\$.*)' defined from:
```

Parameters:

|Name | Type |
|---|---|
|variableType|either literal "string" or "date"|
|variableName| The name of the variable|
| query (multiline input)| The query used to define the variable|


Sets a variable that can be used later in the results.
When used in results, the ***$*** must be in front of the varable name

###  <a name="defineValibleAndValue" ></a> Given I have a <date|string> variable named '$<variableName>' with a value of '<value>'
Patterns:
```regex
I have a (?<variableType>string|date) variable named '(?<variableName>\$.*)' with a value of '(?<value>.*)'"
```

Parameters:

|Name | Type |
|---|---|
|variableType|either literal "string" or "date"|
|variableName| The name of the variable|
| valuee | The value to set the variable to|


Sets a variable that can be used later in the results.
When used in results, the ***$*** must be in front of the varable name