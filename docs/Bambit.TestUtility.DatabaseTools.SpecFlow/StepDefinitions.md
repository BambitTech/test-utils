# Step List
The following list comprises the database steps available:
 * Database General Steps
   * [Given I am working in the (?<connectionName>.*) database](DatabaseGeneralSteps.md#SetCurrentDatabase)
 * Utility Steps
   * [Given I have a <date|string> variable named '$<variableName>' defined from:](DatabaseGeneralSteps.md#SetCurrentDatabase)
 * Utility Steps
   * [Given I have a <date|string> variable named '$<variableName>' with a value of '<value>'](UtilitySteps.md#defineValibleAndValue)

  * Given(@"[Tt]he query named ""(?<queryName>.*)"" is run with the (?:following )?parameters:")
 * Given(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database with the (?:following ) parameters:")
 * Given(@"[Tt]he query ""(?<query>.*)"" is run in the (?<connectionName>.*) database with the (?:following )? parameters:")
 * Given(@"[Tt]he query '(?<query>.*)' is run in the (?<connectionName>.*) database")
 * Given(@"[Tt]he following query is run:")
 * Given(@"[Tt]he query named ""(?<queryName>.*)"" is run")
 * Given(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database")
 * When(@"[Tt]he query named ""(?<queryName>.*)"" is run")
 * When(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database")
 * When(@"[Tt]he following query is run in the (?<connectionName>.*) database:")
 * When(@"[Tt]he query ""(?<query>.*)"" is run in the (?<connectionName>.*) database")
 * When(@"the query named ""(?<queryName>.*)"" is run with the (?:following )?parameters:")
 * When(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database with the (?:following) parameters:")
 * When(@"the following query is run:")
 * When(@"the query ""(?<query>.*)"" is run")
 * Then(@"[Tt]he query ""(?<query>.*)"" returns:")
 * Then(@"[Tt]he query ""(?<query>.*)"" in database {?<connectionName>..*} returns:")
 * Then(@"[Tt]he results(?: of the procedure)? will be(?: the following)?:")
 * Then(@"[Tt]he query named ""(?<queryName>.*)"" returns:")
 * Then(@"[Tt]he query named ""(?<queryName>.*)"" run in the (?<connectionName>.*) database returns:")
 * Given(@"[Tt]he following query is run in the (?<connectionName>.*) database:")
 