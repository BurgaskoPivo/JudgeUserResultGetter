using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JugdeStudentInfo
{
    public class HtmlJudgeUserResultExtractor
    {
        public void GetResults(IEnumerable<string> userNames)
        {
            foreach (var name in userNames)
            {
                string pattern = @"\b" + name + @"\b(?s)(?:.*?)(<\/tr>)";
                Regex regex = new Regex(pattern);
                string html = File.ReadAllText("../../html.html");

                var results = regex.Matches(html);

                foreach (Match match in results)
                {
                    string[] points = Regex.Replace(match.Groups[0].Value, @"\D+<td", "")
                        .Split(new char[] { '>', '<', '\r', '\t', 'd', '\n', 'r', 't', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    using (StreamWriter sw = new StreamWriter(
                        new FileStream("../../results.txt", FileMode.Append)))
                    {
                        sw.Write($"{name} ");

                        foreach (var point in points)
                        {
                            sw.Write($"{point} ");
                        }
                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
