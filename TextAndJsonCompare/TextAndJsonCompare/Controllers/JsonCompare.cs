using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextAndJsonCompare.Controllers
{
    public class JsonCompareController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Compare(IFormFile file1, string text1, IFormFile file2, string text2)
        {
            try
            {
                var json1 = GetJsonText(file1, text1);
                var json2 = GetJsonText(file2, text2);

                if (json1 == null || json2 == null)
                {
                    return BadRequest("Invalid JSON input.");
                }

                var differences = FindDifferences(json1, json2);

                ViewBag.Json1 = json1;
                ViewBag.Json2 = json2;
                ViewBag.Differences = differences;

                return View("JsonResult");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private string GetJsonText(IFormFile file, string text)
        {
            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            else if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            else
            {
                return null;
            }
        }

        private List<string> FindDifferences(string json1, string json2)
        {
            var diffList = new List<string>();

            JToken token1 = JToken.Parse(json1);
            JToken token2 = JToken.Parse(json2);

            // Check if the root object names are different
            if (token1.Type == JTokenType.Object && token2.Type == JTokenType.Object)
            {
                var rootName1 = ((JObject)token1).Properties().FirstOrDefault()?.Name;
                var rootName2 = ((JObject)token2).Properties().FirstOrDefault()?.Name;

                if (rootName1 != rootName2)
                {
                    // If root object names are different, return entire JSON strings
                    diffList.Add(json1);
                    diffList.Add(json2);
                    return diffList;
                }
            }

            CompareJson(token1, token2, "", diffList);
            //List<string> processedDiff = GetUniqueParts(diffList);

            return diffList;
        }

        private void CompareJson(JToken token1, JToken token2, string ancestry, List<string> diffList)
        {
            if (diffList == null)
            {
                // Initialize the diffList if it's null
                diffList = new List<string>();
            }

            if (!JToken.DeepEquals(token1, token2))
            {
                if (token1.Type == JTokenType.Object && token2.Type == JTokenType.Object)
                {
                    var props1 = (JObject)token1;
                    var props2 = (JObject)token2;

                    // Check for properties present in props1 but not in props2
                    foreach (var prop in props1)
                    {
                        string key = prop.Key;
                        JToken val1 = prop.Value;
                        JToken val2 = props2.GetValue(key);

                        if (val2 == null)
                        {
                            diffList.Add($"{ancestry}.{key}");
                        }
                    }

                    // Check for properties present in props2 but not in props1
                    foreach (var prop in props2)
                    {
                        string key = prop.Key;
                        JToken val2 = prop.Value;
                        JToken val1 = props1.GetValue(key);

                        if (val1 == null)
                        {
                            diffList.Add($"{ancestry}.{key}");
                        }
                    }

                    // Recursively compare nested objects
                    foreach (var prop in props1)
                    {
                        string key = prop.Key;
                        JToken val1 = prop.Value;
                        JToken val2 = props2.GetValue(key);

                        string newAncestry = string.IsNullOrEmpty(ancestry) ? key : $"{ancestry}.{key}";

                        if (val2 != null)
                        {
                            CompareJson(val1, val2, newAncestry, diffList);
                        }
                    }
                }
                else if (token1.Type == JTokenType.Array && token2.Type == JTokenType.Array)
                {
                    var arr1 = (JArray)token1;
                    var arr2 = (JArray)token2;

                    // Check if the arrays have different lengths
                    if (arr1.Count != arr2.Count)
                    {
                        diffList.Add(ancestry);
                    }
                    else
                    {
                        // Recursively compare array elements
                        for (int i = 0; i < arr1.Count; i++)
                        {
                            JToken item1 = arr1[i];
                            JToken item2 = arr2[i];

                            string newAncestry = $"{ancestry}[{i}]";

                            CompareJson(item1, item2, newAncestry, diffList);
                        }
                    }
                }
                else
                {
                    // If the tokens are different, add the full path to the differences list
                    diffList.Add(ancestry);
                }
            }
        }









    }
}
