Feature: Variables
Background: 
	Given I am working in the SqlTestDb database
	
Scenario: Simple Variable replacement
	Given I have a string variable named '$Test' with a value of 'ABC'
	When the query "Select 'ABC' [Test]" is run
	Then the results will be:
	| Test  |
	| $Test |


Scenario: Variable replacement With Strings
	Given I have a string variable named '$Test' with a value of '  ABC  '
	When the query "Select '  ABC  ' [Test]" is run
	Then the results will be:
	| Test  |
	| $Test |