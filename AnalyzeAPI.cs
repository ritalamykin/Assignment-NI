using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace AnalyzeAPI
{
    // AnalyzeAPI has two functions:
    // 1. Analyze(string) which will return an array of strings according to the data source string input.
    // 2. Analyze(string, long) which will call Analyze(string) and then perform filters according to predefined flows.
    public class AnalyzeAPI
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler();
        private static readonly HttpClient client = new HttpClient(handler);
        private static Dictionary<string, string> sourceURLMap = createMap();
        private static Dictionary<long, AnalysisFlow> flows = createFlows();

        static AnalyzeAPI()
        {
            setUp();
        }

        // AnalysisFlow
        public class AnalysisFlow {
            List<Func<string[], string[]>> filters;

            public AnalysisFlow(List<Func<string[], string[]>> filters) {
                this.filters = filters;
            }
            //Returns a list with the same elements as list, but without spaces
            public static string[] removeSpaces(string[] list)
            {
                List<String> res = new List<string>();
                foreach (string item in list)
                {
                    string s = item.Replace(" ", "");
                    res.Add(s);
                }
                return res.ToArray();
            }

            public List<Func<string[], string[]>> getFilters()
            {
                return this.filters;
            }

            // Returns a modified list that contains all items in the list containing 5 or more characters.
            public static string[] removeShortItems(string[] list)
            {
                List<string> tempList = new List<string>();
                foreach(string item in list)
                {
                    if(item.Length >= 5) {
                        tempList.Add(item);
                    }
                }
                return tempList.ToArray();
            }

            //Returns a list with the same elements as list, but all elements in lower case
            public static string[] toLowerCase(string[] list)
            {
                for(int i = 0; i < list.Length; i++)
                {
                    list[i] = list[i].ToLower();
                }
                return list;
            }
        }

        public static Dictionary<long, AnalysisFlow> createFlows() {
            Dictionary<long, AnalysisFlow> flows = new Dictionary<long, AnalysisFlow>();
            flows[1] = new AnalysisFlow(new List<Func<string[], string[]>> { AnalysisFlow.removeSpaces });
            flows[2] = new AnalysisFlow(new List<Func<string[], string[]>> { AnalysisFlow.removeShortItems });
            flows[3] = new AnalysisFlow(new List<Func<string[], string[]>> { AnalysisFlow.removeShortItems, AnalysisFlow.toLowerCase});
            flows[4] = new AnalysisFlow(new List<Func<string[], string[]>> { AnalysisFlow.removeShortItems, AnalysisFlow.toLowerCase, AnalysisFlow.removeSpaces });
            return flows;
        }

        public static string[] Analyze(string dataSourceName, long id)
        {
            string[] res = Analyze(dataSourceName);
            // run all the functions under flows[id];
            List<Func<string[], string[]>> filters = flows[id].getFilters();
            foreach (var func in filters)
            {
                res = func(res);
            }
            return res;
        }

        public static string[] Analyze(string dataSourceName)
        {
            string json = AnalyzeAPI.getResponseString(dataSourceName);
            List<string> retList = createListForSourceName(json, dataSourceName);
            return retList.ToArray();
        }


        public static void setUp()
        {
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Rita's Assignment for NI");
        }




        //Initializes the dictionary that maps Source name --> URL
        public static Dictionary<string, string> createMap()
        {
            Dictionary<string, string> sourceURLMap = new Dictionary<string, string>();
            sourceURLMap.Add("Stackoverflow", "https://api.stackexchange.com/2.2/tags/highcharts/faq?site=stackoverflow");
            sourceURLMap.Add("Github", "https://api.github.com/repos/highcharts/highcharts/commits");
            return sourceURLMap;
        }

        // Returns a json string that is returned from calling the API in the
        // sourceURLMap[dataSourceName].
        private static string getResponseString(string dataSourceName)
        {
            string URL = AnalyzeAPI.sourceURLMap[dataSourceName];
            var responseString = client.GetAsync(URL).Result;
            string res = responseString.Content.ReadAsStringAsync().Result;
            return res;
        }

        
        public static List<string> createListForSourceName(string JSonObj, string dataSourceName)
        {
            List<string> retList = new List<string>();
            var data = JsonConvert.DeserializeObject<dynamic>(JSonObj);
            if (dataSourceName.Equals("Stackoverflow"))
            {
                foreach (var item in data["items"])
                {
                    string s = item["title"];
                    retList.Add(s);
                }
            }
            else if (dataSourceName.Equals("Github"))
            {
                foreach (var item in data)
                {
                    string s = item["commit"]["message"];
                    retList.Add(s);
                }
            }
            
            return retList;
        }

        public static void Main(string[] args)
        {
            //Main for debugging
        }
    }

}

