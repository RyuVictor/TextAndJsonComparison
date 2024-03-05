using Microsoft.AspNetCore.Mvc;
using TextAndJsonCompare.Models;
using System;
using System.Collections.Generic;

namespace TextAndJsonCompare.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new TextAndJsonCompare.Models.TextToCompare();
            return View(model);
        }

        [HttpPost]
        public IActionResult Compare(TextToCompare model)
        {
            // Split the text by line breaks and remove empty entries
            string[] lines1 = model.Alpha.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines2 = model.Beta.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Compare the arrays line by line
            List<int> differentLineNumbers = new List<int>();
            for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
            {
                if (lines1[i].Trim() != lines2[i].Trim())
                {
                    differentLineNumbers.Add(i + 1); // Add 1 because line numbers start from 1
                }
            }

            // If the number of lines is different, consider the extra lines as dissimilar
            for (int i = Math.Min(lines1.Length, lines2.Length); i < Math.Max(lines1.Length, lines2.Length); i++)
            {
                differentLineNumbers.Add(i + 1);
            }

            ViewBag.DifferentLineNumbers = differentLineNumbers;

            // Pass the lines as arrays to the view
            ViewBag.Lines1 = lines1;
            ViewBag.Lines2 = lines2;

            return View("Result");
        }
    }
}
