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
    public partial class GetCommonFriendsFeature : object, Xunit.IClassFixture<GetCommonFriendsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "GetCommonFriends.feature"
#line hidden
        
        public GetCommonFriendsFeature(GetCommonFriendsFeature.FixtureData fixtureData, Payment_Application_AcceptanceTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Users", "Get Common Friends", "![User] (../../../assets/user.svg)\n\n    In order to support users in the system\n\n" +
                    "    As a user service\n\n    I want to facilitate the retrieval of common friends " +
                    "between two users", ProgrammingLanguage.CSharp, featureTags);
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
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "UserId",
                        "FullName"});
            table3.AddRow(new string[] {
                        "73AC6B13-780F-4395-B468-506904422719",
                        "John Doe"});
            table3.AddRow(new string[] {
                        "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF",
                        "Samantha James"});
            table3.AddRow(new string[] {
                        "639A1CDD-7915-47E5-AA2B-263873542B28",
                        "Dr Andrew Martin"});
#line 12
      testRunner.Given("following users in the system", ((string)(null)), table3, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "UserId",
                        "FriendId"});
            table4.AddRow(new string[] {
                        "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF",
                        "73AC6B13-780F-4395-B468-506904422719"});
            table4.AddRow(new string[] {
                        "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF",
                        "639A1CDD-7915-47E5-AA2B-263873542B28"});
            table4.AddRow(new string[] {
                        "73AC6B13-780F-4395-B468-506904422719",
                        "639A1CDD-7915-47E5-AA2B-263873542B28"});
#line 17
      testRunner.And("the users has the following friends in the system", ((string)(null)), table4, "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Get Common friends")]
        [Xunit.TraitAttribute("FeatureTitle", "Get Common Friends")]
        [Xunit.TraitAttribute("Description", "Get Common friends")]
        [Xunit.TraitAttribute("Category", "user")]
        [Xunit.TraitAttribute("Category", "happy-paths")]
        [Xunit.InlineDataAttribute("\"73AC6B13-780F-4395-B468-506904422719\"", "\"FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF\"", new string[0])]
        public void GetCommonFriends(string userId1, string userId2, string[] exampleTags)
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
            argumentsOfScenario.Add("UserId1", userId1);
            argumentsOfScenario.Add("UserId2", userId2);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get Common friends", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 28
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
#line 29
      testRunner.Given(string.Format("a user1 with id {0}", userId1), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 30
      testRunner.And(string.Format("a user2 with id {0}", userId2), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 31
      testRunner.When("I submit the request to get common friends", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 32
      testRunner.Then("the response should be successful", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "UserId",
                            "FullName"});
                table5.AddRow(new string[] {
                            "639A1CDD-7915-47E5-AA2B-263873542B28",
                            "Dr Andrew Martin"});
#line 33
      testRunner.And("the response should have following common friend", ((string)(null)), table5, "And ");
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
                GetCommonFriendsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                GetCommonFriendsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
