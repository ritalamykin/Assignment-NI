using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AnalyzeAPI.AnalyzeAPI;

namespace AnalyzeAPI
{
    [TestClass]
    public class UnitTest1
    {

        // Unit Tests//

        [TestMethod]
        public void TestRemoveSpacesManySpacesEmptyList()
        {
            string[] list = {};
            string[] expected = {};
            string[] actual = AnalyzeAPI.AnalysisFlow.removeSpaces(list);
            CollectionAssert.AreEqual(expected, actual, "TestRemoveSpacesManySpacesEmptyList failed");
        }

        [TestMethod]
        public void TestRemoveSpacesManySpacesBasic()
        {
            string[] list = { "This  is     a", "    fake", " test" };
            string[] expected = { "Thisisa", "fake", "test" };
            string[] actual = AnalyzeAPI.AnalysisFlow.removeSpaces(list);
            CollectionAssert.AreEqual(expected, actual, "RemoveSpacesManySpacesBasic failed");
        }

        [TestMethod]
        public void TestRemoveShortItemsAllShort()
        {
            string[] list = { "a", "b", "c", "dfgh" };
            string[] expected = { };
            string[] actual = AnalyzeAPI.AnalysisFlow.removeShortItems(list);
            CollectionAssert.AreEqual(expected, actual, "TestRemoveShortItemsAllShort failed");
        }

        [TestMethod]
        public void TestRemoveShortItemsAllLong()
        {
            string[] list = { "September", "October"};
            string[] expected = { "September", "October" };
            string[] actual = AnalyzeAPI.AnalysisFlow.removeShortItems(list);
            CollectionAssert.AreEqual(expected, actual, "TestRemoveShortItemsAllLong failed");
        }

        [TestMethod]
        public void TestCreateListForSourceNameGit()
        {
            string json = "[{\"commit\":{\"message\":\"message1\"}}]";
            List<string> expected = new List<string> { "message1" };
            List<string> actual = AnalyzeAPI.createListForSourceName(json, "Github");
            CollectionAssert.AreEqual(expected, actual, "TestCreateListForSourceNameGit failed");
        }

        [TestMethod]
        public void TestCreateListForSourceNameStack()
        {
            string json = "{\"items\": [{ \"not_title\":1371709490,\"title\":\"title1\"},{\"title\":\"title2\", \"not_title\":1371709490}]}";
            List<string> expected = new List<string> { "title1", "title2" };
            List<string> actual = AnalyzeAPI.createListForSourceName(json, "Stackoverflow");
            CollectionAssert.AreEqual(expected, actual, "TestCreateListForSourceNameStack failed");
        }

        // API Tests // 
        [TestMethod]
        public void TestAnalyzeAPI()
        {
            string[] result = AnalyzeAPI.Analyze("Stackoverflow");
            Assert.AreEqual(result[1], "Set Additional Data to highcharts series", "TestCreateListForSourceNameStack failed");
        }
    }
}
