Feature: Named Queries
	Verifies execution of queries that have been preloaded and named

Background: 
	Given I am working in the SqlTestDb database

Scenario: Simple Script - Given/When/Then
	Given the query named "Simple Script" is run
	When the query named "Simple Script" is run
	Then the results of the procedure will be the following:
	| Field1 | Field2 | Field5  |
	| 1      | two    | Chicken |

Scenario: Simple Script - Then
	Then the query named "Simple Script" returns:
	| Field1 | Field2 | Field5  |
	| 1      | two    | Chicken |
	
Scenario: Simple Script, named connection - Given/When/Then 
	Given the query named "Simple Script" is run in the SqlTestDb database
	When the query named "Simple Script" is run in the SqlTestDb database
	Then the results of the procedure will be:
	| Field1 | Field2 | Field5  |
	| 1      | two    | Chicken |

	
Scenario: Simple Script, named connection - Then
	Then the query named "Simple Script" run in the SqlTestDb database returns:
	| Field1 | Field2 | Field5  |
	| 1      | two    | Chicken |


Scenario: Simple Script With Parameters Given/When/Then
	Given the query named "Simple Script With Parameters" is run with the following parameters:
	| alpha | @beta | gamma |
	| abc   | 123   | 12.34 |
	When the query named "Simple Script With Parameters" is run with the following parameters:
	| alpha | @beta | gamma |
	| abc   | 123   | 12.34 |
	Then the results will be:
	| Field1 | Field2 | Field5 |
	| abc    | 123    | 12.34  |

	

Scenario: Embedded Script Given/When/Then
	Given the query named "Embedded Script" is run with the following parameters:
	| @id | @n    |
	| 3   | alpha |  
	When the query named "Embedded Script" is run with the following parameters:
	| @id | @n    |
	| 3   | alpha | 
	Then the results of the procedure will be the following:
	| id | n     |
	| 3  | alpha |
	
Scenario: Copied Script Given/When/Then
	Given the query named "Embedded Script" is run with the following parameters:
	| @id | @n    |
	| 3   | alpha |  
	When the query named "Embedded Script" is run with the following parameters:
	| @id | @n    |
	| 3   | alpha | 
	Then the results will be the following:
	| id | n     |
	| 3  | alpha |


Scenario: Multi line query
	Given the following query is run:
	"""
	select 1 [Field1], 'alpha' [Field2]
	union
	select 2 [Field1], 'beta' [Field2]
	"""
	Given the following query is run in the SqlTestDb database:
	"""
	select 1 [Field1], 'alpha' [Field2]
	union
	select 2 [Field1], 'beta' [Field2]
	"""
	When the following query is run:
	"""
	select 3 [Field1], 'gamma' [Field2]
	union
	select 5 [Field1], 'delta' [Field2]
	"""
	Then the results will be the following:
	| Field1 | Field2 |
	| 3      | gamma  |
	| 5      | delta  |