using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JudgeResults
{
    public class Launcher
    {
        public static void Main()
        {
            List<string> names = new List<string>();

            using (StreamReader sr = new StreamReader(
                new FileStream("../../users.txt", FileMode.Open)))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    names.Add(line);
                    line = sr.ReadLine();
                }
            }

            foreach (var name in names)
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
