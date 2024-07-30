@integration

Feature: TableUtilitiesSqlServer
	Validates General Table utilities against SqlServer database

Background: 
	Given I am working in the SqlTestDb database



Scenario: Verify only speficed records exist full table matches full table records
	Given I am working in the SqlTestDb database
	And only the following records exist in the [Test].[TestTableAlpha] table:
	| TestTableAlphaId                     | [Name]                | [DateOfBirth] | Score | CheckSmall | CheckTiny | [MadMummyMoney] | [TightDecimal] | [NullableBit] |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Bob Spelled Backwards | 1979-11-11    | 12    | 4          | 1         | 123.45          | 345.34         | 1             |
	Then only the following records should exist in the [Test].[TestTableAlpha] table:
	| TestTableAlphaId                     | [Name]                | [DateOfBirth] | Score | CheckSmall | CheckTiny | [MadMummyMoney] | [TightDecimal] | [NullableBit] |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Bob Spelled Backwards | 1979-11-11    | 12    | 4          | 1         | 123.45          | 345.34         | 1             |
	
	

Scenario: Verify at least speficed records exist full table matches full table records

	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | Name          | DateOfBirth | Score | [NullableBit] |
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | John Smith    | 11/07/1975    | 100   | 1             |
| A0926EEA-876A-4C25-9004-2A4724D73F0C | Susan Johnson | 1/1/1980      | 90    | NULL          |
| 474A6CA3-FBEF-4C38-8C59-6E6AA18E9503 | Bob Obo       | 3/4/1956      | 250   | False         |

	Then the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | Name          | DateOfBirth | Score |
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | John Smith    | 11/07/1975    | 100   |


Scenario: Verify Records exist, matching only a subset of columns
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | 
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | 
| D8EE9AFC-9869-47FA-A348-66C5CB144ED9 | 
| A0926EEA-876A-4C25-9004-2A4724D73F0C | 
| 474A6CA3-FBEF-4C38-8C59-6E6AA18E9503 | 
	Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | 
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | 
| D8EE9AFC-9869-47FA-A348-66C5CB144ED9 | 
| A0926EEA-876A-4C25-9004-2A4724D73F0C | 
| 474A6CA3-FBEF-4C38-8C59-6E6AA18E9503 | 



Scenario: Ensure if there are duplicate rows, it matches as expected
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | Name          | DateOfBirth | Score |
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | John Smith    | 11/07/1975    | 100   |
| D8EE9AFC-9869-47FA-A348-66C5CB144ED9 | John Smith    | 11/07/1975    | 100   |
| A0926EEA-876A-4C25-9004-2A4724D73F0C | Susan Johnson | 1/1/1980      | 90    |
| 474A6CA3-FBEF-4C38-8C59-6E6AA18E9503 | Bob Obo       | 3/4/1956      | 250   |
	Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| Score |
| 100   |
| 100   |
| 90    |
| 250   |



Scenario: Ensure extra rows can be ignored

	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| Name          | DateOfBirth | Score |
| John Smith    | 11/07/1975    | 100   |
| John Smith    | 10/20/1975    | 75   |
| Susan Johnson | 1/1/1980      | 90    |
| Bob Obo       | 3/4/1956      | 250   |

	Then the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| Name          | 
| John Smith    | 


Scenario: Ensure Small Int Columns are matched as expected
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| CheckSmall |
| 1          |
| 3          |
| 5          |
| -1         |
	
	Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| CheckSmall |
| 1          |
| 3          |
| 5          |
| -1         |


Scenario: Ensure Tiny Int Columns are matched as expected
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| CheckTiny |
| 1          |
| 3          |
| 0          |
| 255         |	
	Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| CheckTiny |
| 1         |
| 3         |
| 0         |
| 255       |


Scenario: Query with bit defined
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| [NullableBit] |
| 1             |
| NULL          |
| False         |

	Then the query "select [NullableBit] from Test.TestTableAlpha" returns:
| [NullableBit] @boolean |
| 1                      |
| NULL                   |
| False                  |


Scenario: Query with BigInt
	
	Given only the following records exist in the Test.TestTableBigInt table in the SqlTestDb database:
	| Id | [Big] |
	| 1  | 100   |
	| 2  | 200   |

	Then the query "select [Big] from Test.TestTableBigInt" returns:
	| [Big] |
	| 100   |
	| 200   |



Scenario: Expected exceptions are verified
	Given only the following records exist in the Test.[TestTableEpsilon] table:
	| Name  |
	| ALpha |
	Then Inserting the following records in the Test.[TestTableEpsilon] table will throw an error:
	| Name  |
	| ALpha |
	And the last SQL exception will contain the phrase "Violation of UNIQUE KEY constraint"

	

Scenario: Expected exceptions are verified - Long Form
Given only the following records exist in the Test.[TestTableEpsilon] table:
| Name  |
| ALpha |
Then Inserting the following records in the Test.[TestTableEpsilon] table in the SqlTestDb database will throw an error:
| Name  |
| ALpha |
And the last SQL exception will contain the phrase "Violation of UNIQUE KEY constraint"




Scenario: Ensure Row Count statement works
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | 
| F6CC249B-BC42-46B9-A1CF-8D90690E4210 | 
| D8EE9AFC-9869-47FA-A348-66C5CB144ED9 | 
| A0926EEA-876A-4C25-9004-2A4724D73F0C | 
| 474A6CA3-FBEF-4C38-8C59-6E6AA18E9503 | 
	Then the Test.TestTableAlpha table in the SqlTestDb database will have 4 rows

	

Scenario: Test Row Count statement with no rows, no rows
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| TestTableAlphaId                     | 
	Then the Test.TestTableAlpha table in the SqlTestDb database will have no rows

Scenario: Empty strings treated like nulls
	Given I am working in the SqlTestDb database
	And I'm treating empty strings as null
	Given the table Test.TestTableAlpha is empty
	And the query 'insert into Test.TestTableAlpha (TestTableAlphaId, name, Score) values (newid(), 'Me',null)' is run in the SqlTestDb database
	Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| Name | DateOfBirth | Score |
| Me   |               |       |




Scenario: Ensure date transformations are processed as expected
Given the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| DateOfBirth        @date |
| today                   |
| yesterday               |
| tomorrow                   |
| first day of this month    |
| first day of current month |
| first day of month         |
| last day of this month     |
| last day of current month  |
| last day of month          |
| end of last month          |
| last day of last month     |
| last day of previous month |
| today       +1             |
| tomorrow  -1             |
	Then the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database:
| DateOfBirth        @date |
| today                      |
| yesterday                  |
| tomorrow                   |
| first day of this month    |
| first day of current month |
| first day of month         |
| last day of this month     |
| last day of current month  |
| last day of month          |
| end of last month          |
| last day of last month     |
| last day of previous month |
| tomorrow                   |
| today                      |



Scenario: Set a variable from the database
Given the table Test.TestTableAlpha is empty
And the following query is run:
"""
insert into  Test.TestTableAlpha (TestTableAlphaId, DateOfBirth) values (newid(), getdate())
"""
And I have a string variable named '$dbGuid' defined from:
"""
select top 1 cast( TestTableAlphaId as varchar(128)) from Test.TestTableAlpha
"""
And I have a date variable named '$dbDate' defined from:
"""
select top 1 DateOfBirth from Test.TestTableAlpha
"""
Then only the following records should exist in the Test.TestTableAlpha table:
| TestTableAlphaId  | DateOfBirth @date|
| $dbGuid          | $dbDate      |


Scenario: Query then statement
Then the query "select 'abc' [Col1], 123 [Col2], cast('2021-06-04' as datetime) [Col3]" returns:
| Col1 | Col2 | Col3 @date |
| abc  | 123  | 06/04/2021 |


Scenario: Results table has quoted values, persists spaces
Then the query "select '  abc  ' [Col1], 123 [Col2], cast('2021-06-04' as datetime) [Col3]" returns:
| Col1 @quoted | Col2 | Col3 @date |
| '  abc  '    | 123  | 06/04/2021 |


Scenario: Results table has quoted column but no quotes
Then the query "select 'abc' [Col1], 123 [Col2], cast('2021-06-04' as datetime) [Col3]" returns:
| Col1 @quoted | Col2 | Col3 @date |
| abc          | 123  | 06/04/2021 |



Scenario: Allow for timeout, does not throw exception
Given I have a query timeout of 70 seconds
When the following query is run in the SqlTestDb database:
"""
waitfor delay '00:01'
select 'ok' as [c1]
"""
Then the results will be:
| c1        |
| ok        |



Scenario: Timeouts breached, throws exception
Given I have a query timeout of 2 seconds
When the following query run in the SqlTestDb database should timeout:
"""
waitfor delay '00:01'
select 'ok' [check]
"""


Scenario: Negative numbers are handled as expected
	Given only the following records exist in the Test.TestTableAlpha table in the SqlTestDb database:
| [TightDecimal] |
| 1              |
Then only the following records should exist in the Test.TestTableAlpha table in the SqlTestDb database should fail:
| [TightDecimal] |
| -1             |



Scenario: Expected exception not thrown are verified
	Given only the following records exist in the Test.[TestTableEpsilon] table:
	| Name  |
	| ALpha |
	Then Inserting the following records in the Test.[TestTableEpsilon] table will throw an error (override):
	| Name  |
	| Beta |


Scenario: Expected is thrown when trying to assign invalid field
	Given only the following records exist in the Test.[TestTableEpsilon] table (override):
	| DoNotUse  |
	| ALpha |
	