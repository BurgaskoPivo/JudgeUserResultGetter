using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JugdeStudentInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = this.UserNameBox.Text;
            string password = this.PasswordBox.Password;

            using (var webClient = new CookieAwareWebClient())
            {
                try
                {
                    NameValueCollection collection = new NameValueCollection
                    {
                        {"UserName", userName},
                        {"Password", password}
                    };

                    webClient.UploadValues("https://judge.softuni.bg/Account/Login", "POST", collection);

                    
                    string result = webClient.DownloadString("https://judge.softuni.bg/Contests/Compete/Results/Simple/104");

                }
                catch (WebException webException)
                {
                    MessageBox.Show(webException.Status.ToString());
                }
            }

           var jugdeInformationGetter = new JugdeInformationCollector();
           this.Close();
           jugdeInformationGetter.ShowDialog();

        }

    }
}
