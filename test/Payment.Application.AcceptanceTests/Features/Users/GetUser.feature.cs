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
namespace Payment.Application.AcceptanceTests.Features.Users
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class GetUserFeature : object, Xunit.IClassFixture<GetUserFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "GetUser.feature"
#line hidden
        
        public GetUserFeature(GetUserFeature.FixtureData fixtureData, Payment_Application_AcceptanceTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Users", "Get User", "![User] (../../../assets/user.svg)\n\n    In order to support users in the system\n\n" +
                    "    As a user service\n\n    I want to facilitate the retrieval of users", ProgrammingLanguage.CSharp, featureTags);
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
            TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                        "UserId",
                        "FullName"});
            table15.AddRow(new string[] {
                        "73AC6B13-780F-4395-B468-506904422719",
                        "John Doe"});
            table15.AddRow(new string[] {
                        "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF",
                        "Guillaume Reynard"});
            table15.AddRow(new string[] {
                        "639A1CDD-7915-47E5-AA2B-263873542B28",
                        "Dr Andrew Martin"});
#line 12
      testRunner.Given("following users in the system", ((string)(null)), table15, "Given ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Get a user")]
        [Xunit.TraitAttribute("FeatureTitle", "Get User")]
        [Xunit.TraitAttribute("Description", "Get a user")]
        [Xunit.TraitAttribute("Category", "user")]
        [Xunit.TraitAttribute("Category", "happy-paths")]
        [Xunit.InlineDataAttribute("73AC6B13-780F-4395-B468-506904422719", new string[0])]
        public void GetAUser(string userId, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "user",
                    "happy-paths"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("UserId", userId);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get a user", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 24
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
#line 25
      testRunner.Given(string.Format("a user with id {0}", userId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 26
      testRunner.When("I submit the query to get the user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 27
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 28
      testRunner.And(string.Format("the response should be a user with id {0}", userId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Get a user with friends")]
        [Xunit.TraitAttribute("FeatureTitle", "Get User")]
        [Xunit.TraitAttribute("Description", "Get a user with friends")]
        [Xunit.TraitAttribute("Category", "user")]
        [Xunit.TraitAttribute("Category", "happy-paths")]
        [Xunit.InlineDataAttribute("73AC6B13-780F-4395-B468-506904422719", "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF", "\"Guillaume Reynard\"", new string[0])]
        [Xunit.InlineDataAttribute("73AC6B13-780F-4395-B468-506904422719", "639A1CDD-7915-47E5-AA2B-263873542B28", "\"Dr Andrew Martin\"", new string[0])]
        public void GetAUserWithFriends(string userId, string friendId, string friendFullName, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "user",
                    "happy-paths"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("UserId", userId);
            argumentsOfScenario.Add("FriendId", friendId);
            argumentsOfScenario.Add("FriendFullName", friendFullName);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get a user with friends", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 36
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
#line 37
      testRunner.Given(string.Format("a user with id {0}", userId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                            "UserId",
                            "FriendId"});
                table16.AddRow(new string[] {
                            "73AC6B13-780F-4395-B468-506904422719",
                            "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF"});
                table16.AddRow(new string[] {
                            "73AC6B13-780F-4395-B468-506904422719",
                            "639A1CDD-7915-47E5-AA2B-263873542B28"});
#line 38
      testRunner.And("following friendships for the user exist in the system", ((string)(null)), table16, "And ");
#line hidden
#line 42
      testRunner.When("I submit the query to get the user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 43
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 44
      testRunner.And(string.Format("the response should be a user with id {0}", userId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 45
      testRunner.And(string.Format("the user should have a friend with id {0} and fullname {1}", friendId, friendFullName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
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
                GetUserFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                GetUserFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
