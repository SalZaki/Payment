﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Payment.Application.AcceptanceTests.Features.Wallets
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class ContributeWalletFeature : object, Xunit.IClassFixture<ContributeWalletFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "ContributeWallet.feature"
#line hidden
        
        public ContributeWalletFeature(ContributeWalletFeature.FixtureData fixtureData, Payment_Application_AcceptanceTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Wallets", "Contribute Wallet", "![Wallet] (../assets/wallet.svg)\n\n    In order to use wallets in the system\n\n    " +
                    "As a wallet service\n\n    I want to facilitate the contributions to the wallets", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 11
    #line hidden
            TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                        "UserId",
                        "Fullname"});
            table17.AddRow(new string[] {
                        "6871AEE6-1814-4EBE-9C0F-EDB7201DAD64",
                        "John Doe"});
            table17.AddRow(new string[] {
                        "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                        "François Dupont"});
            table17.AddRow(new string[] {
                        "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                        "Sara MonaLisa"});
#line 12
      testRunner.Given("following users in the system", ((string)(null)), table17, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table18 = new TechTalk.SpecFlow.Table(new string[] {
                        "WalletId",
                        "UserId",
                        "Currency",
                        "Amount",
                        "ShareCount",
                        "TotalSharesAmount"});
            table18.AddRow(new string[] {
                        "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                        "6871AEE6-1814-4EBE-9C0F-EDB7201DAD64",
                        "GBP",
                        "20.22",
                        "0",
                        "0.00"});
#line 17
      testRunner.And("following wallet with no shares in the system", ((string)(null)), table18, "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="[1/4] Contribute to a user\'s wallet with a single share")]
        [Xunit.TraitAttribute("FeatureTitle", "Contribute Wallet")]
        [Xunit.TraitAttribute("Description", "[1/4] Contribute to a user\'s wallet with a single share")]
        [Xunit.TraitAttribute("Category", "wallet")]
        [Xunit.TraitAttribute("Category", "happy-paths-with-no-shares")]
        [Xunit.InlineDataAttribute("3DF951E4-7317-4F17-AB4A-D9EF924D84BF", new string[0])]
        public void _14ContributeToAUsersWalletWithASingleShare(string walletId, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "wallet",
                    "happy-paths-with-no-shares"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("WalletId", walletId);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("[1/4] Contribute to a user\'s wallet with a single share", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 37
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 11
    this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table19 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table19.AddRow(new string[] {
                            "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                            "EUR",
                            "62.52",
                            "Italy"});
#line 38
      testRunner.Given("the following share to be contributed to user\'s wallet", ((string)(null)), table19, "Given ");
#line hidden
#line 41
      testRunner.And(string.Format("wallet id {0}", walletId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 42
      testRunner.When("I submit a single share request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 43
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "ShareCount",
                            "Currency",
                            "TotalSharesAmount"});
                table20.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                            "1",
                            "EUR",
                            "62.52"});
#line 44
      testRunner.And("the wallet should have", ((string)(null)), table20, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="[2/4] Contribute to a user\'s wallet with a share by a contributor in same currenc" +
            "y")]
        [Xunit.TraitAttribute("FeatureTitle", "Contribute Wallet")]
        [Xunit.TraitAttribute("Description", "[2/4] Contribute to a user\'s wallet with a share by a contributor in same currenc" +
            "y")]
        [Xunit.TraitAttribute("Category", "wallet")]
        [Xunit.TraitAttribute("Category", "happy-paths-with-shares-by-a-contributor")]
        [Xunit.InlineDataAttribute("3DF951E4-7317-4F17-AB4A-D9EF924D84BF", new string[0])]
        public void _24ContributeToAUsersWalletWithAShareByAContributorInSameCurrency(string walletId, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "wallet",
                    "happy-paths-with-shares-by-a-contributor"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("WalletId", walletId);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("[2/4] Contribute to a user\'s wallet with a share by a contributor in same currenc" +
                    "y", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 54
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 11
    this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table21 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table21.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "TND",
                            "262.22",
                            "Tunisia"});
#line 55
      testRunner.Given("the following share are already added to user\'s wallet", ((string)(null)), table21, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table22 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table22.AddRow(new string[] {
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "TND",
                            "30.00",
                            "Tunisia"});
#line 58
      testRunner.And("the following shares to be contributed to user\'s wallet", ((string)(null)), table22, "And ");
#line hidden
#line 61
      testRunner.And(string.Format("wallet id {0}", walletId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
      testRunner.When("I submit a multi share request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 63
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table23 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "ShareCount",
                            "Currency",
                            "TotalSharesAmount"});
                table23.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "2",
                            "TND",
                            "292.22"});
#line 64
      testRunner.And("the wallet should have", ((string)(null)), table23, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="[3/4] Contribute to a user\'s wallet with shares by a contributor")]
        [Xunit.TraitAttribute("FeatureTitle", "Contribute Wallet")]
        [Xunit.TraitAttribute("Description", "[3/4] Contribute to a user\'s wallet with shares by a contributor")]
        [Xunit.TraitAttribute("Category", "wallet")]
        [Xunit.TraitAttribute("Category", "happy-paths-with-shares-by-a-contributor")]
        [Xunit.InlineDataAttribute("3DF951E4-7317-4F17-AB4A-D9EF924D84BF", new string[0])]
        public void _34ContributeToAUsersWalletWithSharesByAContributor(string walletId, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "wallet",
                    "happy-paths-with-shares-by-a-contributor"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("WalletId", walletId);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("[3/4] Contribute to a user\'s wallet with shares by a contributor", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 74
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 11
    this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table24.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "TND",
                            "262.22",
                            "Tunisia"});
#line 75
      testRunner.Given("the following share are already added to user\'s wallet", ((string)(null)), table24, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table25 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table25.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "NZD",
                            "31.24",
                            "New Zealand"});
                table25.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "TND",
                            "30.00",
                            "Tunisia"});
#line 78
      testRunner.And("the following shares to be contributed to user\'s wallet", ((string)(null)), table25, "And ");
#line hidden
#line 82
      testRunner.And(string.Format("wallet id {0}", walletId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 83
      testRunner.When("I submit a multi share request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 84
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table26 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "ShareCount",
                            "Currency",
                            "TotalSharesAmount"});
                table26.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "2",
                            "TND",
                            "292.22"});
                table26.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "1",
                            "NZD",
                            "31.24"});
#line 85
      testRunner.And("the wallet should have", ((string)(null)), table26, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="[4/4] Contribute to user\'s wallet with shares by other contributors")]
        [Xunit.TraitAttribute("FeatureTitle", "Contribute Wallet")]
        [Xunit.TraitAttribute("Description", "[4/4] Contribute to user\'s wallet with shares by other contributors")]
        [Xunit.TraitAttribute("Category", "wallet")]
        [Xunit.TraitAttribute("Category", "happy-paths-with-shares-by-contributors")]
        [Xunit.InlineDataAttribute("3DF951E4-7317-4F17-AB4A-D9EF924D84BF", new string[0])]
        public void _44ContributeToUsersWalletWithSharesByOtherContributors(string walletId, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "wallet",
                    "happy-paths-with-shares-by-contributors"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("WalletId", walletId);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("[4/4] Contribute to user\'s wallet with shares by other contributors", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 96
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 11
    this.FeatureBackground();
#line hidden
#line 97
      testRunner.Given(string.Format("wallet id {0}", walletId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table27 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContributorId",
                            "Currency",
                            "Amount",
                            "Country"});
                table27.AddRow(new string[] {
                            "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                            "EUR",
                            "233.61",
                            "Luxembourg"});
                table27.AddRow(new string[] {
                            "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                            "EUR",
                            "43.20",
                            "Luxembourg"});
                table27.AddRow(new string[] {
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "JPY",
                            "1200.00",
                            "Japan"});
#line 98
      testRunner.And("the following shares to be contributed to user\'s wallet", ((string)(null)), table27, "And ");
#line hidden
#line 103
      testRunner.When("I submit a multi share request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 104
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table28 = new TechTalk.SpecFlow.Table(new string[] {
                            "WalletId",
                            "ContributorId",
                            "ShareCount",
                            "Currency",
                            "TotalSharesAmount"});
                table28.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "06E76F39-79D0-4688-B26E-3B162B3C88FA",
                            "2",
                            "EUR",
                            "276.81"});
                table28.AddRow(new string[] {
                            "3DF951E4-7317-4F17-AB4A-D9EF924D84BF",
                            "A0CB3869-B481-48D8-A452-8E7776F71DF4",
                            "1",
                            "JPY",
                            "1200.00"});
#line 105
      testRunner.And("the wallet should have", ((string)(null)), table28, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                ContributeWalletFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                ContributeWalletFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
