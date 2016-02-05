using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;

namespace JugdeStudentInfo
{
    /// <summary>
    /// Interaction logic for JugdeInformationCollector.xaml
    /// </summary>
    public partial class JugdeInformationCollector : Window
    {
        public JugdeInformationCollector()
        {
            InitializeComponent();
        }

        private void AddToFileBox_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".html",
                Filter = "HTML Files (*.html)|*.html",
                Multiselect = true
                
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string[] fileNamesWithPath = dlg.FileNames;

                foreach (var fileNameWithPath in fileNamesWithPath)
                {
                    string fileName =
                    fileNameWithPath.Substring(fileNameWithPath.IndexOf('\\', fileNameWithPath.Length / 2) + 1);
                    this.FileBox.Items.Add(fileName);
                }
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
            throw new NotImplementedException();
        }

        private void SaveUsersToFile_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
