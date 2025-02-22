﻿using Bambit.TestUtility.DatabaseTools.Reqnroll.Tools;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Tests.Hooks;

[Binding]
public class TestInitialize
{
    [BeforeTestRun(Order = 60)]
    public static void BeforeTestRun()
    {
        QueryHelpers.RegisterNamedScript("Simple Script", "Select 1 [Field1], 'two' [Field2], 'Chicken' [Field5]");
        QueryHelpers.RegisterNamedScript("Simple Script With Parameters", "Select @alpha [Field1], @beta [Field2], @gamma [Field5]");
        QueryHelpers.RegisterNamedScriptFromEmbeddedFile("Embedded Script", typeof(TestInitialize).Assembly, "Bambit.TestUtility.DatabaseTools.Reqnroll.Tests.Scripts.EmbeddedScript.sql");
        QueryHelpers.RegisterNamedScriptFromFile("Script File",@"Scripts/CopiedScript.sql");
        Config.Randomizer.RegisterTableFieldGenerator("SqlTestDb", "dbo",
            "[TestTableWithConstraints]",
            "Constrained",
            (g) => g.CoinToss("A", "B")
        );
    }
}