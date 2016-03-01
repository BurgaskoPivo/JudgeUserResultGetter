using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;

namespace JugdeStudentInfo
{
    public partial class JugdeInformationCollector
    {
        public JugdeInformationCollector()
        {
            InitializeComponent();
        }

        private void AddToFileBox_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".html",
                Filter = "HTML Files (*.html)|*.html",
                Multiselect = true

            };

            bool? showDialog = dlg.ShowDialog();

            if (showDialog == null || !(bool)showDialog)
            {
                return;
            }

            string[] fileNamesWithPath = dlg.FileNames;

            foreach (var fileNameWithPath in fileNamesWithPath)
            {
                string fileName =
                fileNameWithPath.Substring(fileNameWithPath.LastIndexOf('\\') + 1);
                this.FileBox.Items.Add(fileName);
            }

        }

        private void RemoveFromFileBox_Click(object sender, RoutedEventArgs e)
        {
            this.FileBox.Items.Remove(this.FileBox.SelectedItem);
        }

        private void RemoveFromUserBox_Click(object sender, RoutedEventArgs e)
        {
            this.UserBox.Items.Remove(this.UserBox.SelectedItem);
        }

        private void AddToUserBoxFromTextBox_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UserNameBox.Text))
            {
                return;
            }

            this.UserBox.Items.Add(this.UserNameBox.Text);
            this.UserNameBox.Clear();
        }

        private void UserNameBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    e.Handled = true;
                    break;
                case Key.Enter:
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(this.AddToUserBoxFromTextBox);

                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

                    invokeProv?.Invoke();
                    break;
            }
        }

        private void AddToUserBoxFromFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt"
            };

            bool? showDialog = dlg.ShowDialog();

            if (showDialog == null || !(bool)showDialog)
            {
                return;
            }

            string[] userNames = File.ReadAllLines(dlg.FileName);

            foreach (var userName in userNames)
            {
                this.UserBox.Items.Add(userName);
            }

        }

        private void SaveUsersToFile_OnClick(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream("JudgeUsers.txt", FileMode.Append)))
            {
                foreach (var user in this.UserBox.Items)
                {
                    sw.WriteLine(user);
                }
            }
        }

        private void SaveResultsToFile_Click(object sender, RoutedEventArgs e)
        {

            foreach (var name in this.UserBox.Items)
            {
                string pattern = @"\b" + name + @"\b(?s)(?:.*?)(<\/tr>)";
                Regex regex = new Regex(pattern);

                foreach (var item in this.FileBox.Items)
                {
                    string html = File.ReadAllText(item.ToString());

                    var results = regex.Matches(html);

                    foreach (Match match in results)
                    {
                        string[] points = Regex.Replace(match.Groups[0].Value, @"\D+<td", "")
                        .Split(new char[] { '>', '<', '\r', '\t', 'd', '\n', 'r', 't', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        using (StreamWriter sw = new StreamWriter(
                            new FileStream("results/judge results.txt", FileMode.OpenOrCreate)))
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

        private void SaveResultsToTable_Click(object sender, RoutedEventArgs e)
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("results/table result.xlsx", SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Insert Data
            for (int i = 0; i < this.UserBox.Items.Count; i++)
            {
                string pattern = @"\b" + this.UserBox.Items[i] + @"\b(?s)(?:.*?)(<\/tr>)";
                Regex regex = new Regex(pattern);

                foreach (var item in this.FileBox.Items)
                {
                    string html = File.ReadAllText(item.ToString());

                    var results = regex.Matches(html);

                    foreach (Match match in results)
                    {
                        spreadsheetDocument.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());

                        string[] points = Regex.Replace(match.Groups[0].Value, @"\D+<td", "")
                            .Split(new char[] { '>', '<', '\r', '\t', 'd', '\n', 'r', 't', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        spreadsheetDocument.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(
                            new Cell { CellValue = new CellValue(this.UserBox.Items[i].ToString()) });

                        foreach (var point in points)
                        {
                            spreadsheetDocument.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(
                                new Cell { CellValue = new CellValue(point) });
                        }
                    }
                }
            }

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Judge User Results"
            };

            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }
    }
}
