using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.ComponentModel;

namespace Mobideskv2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
           
            verifyuser.DoWork += verifyuser_DoWork;
            verifyuser.RunWorkerCompleted += verifyuser_RunWorkerCompleted;
            verifyuser.WorkerSupportsCancellation = true;
            
            if(Properties.Settings.Default.uid=="" || Properties.Settings.Default.uid == null){
                this.Show();
            }
            else
            {
                new MainWindow().Show();
                this.Hide();
            }

        }
   
        String usr_email;
        String usr_pword;
        String text;

        private BackgroundWorker verifyuser = new BackgroundWorker();      
        userlogin userlogin = new userlogin();
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void verifyuser_DoWork(object sender, DoWorkEventArgs e)
        {
            if (userlogin.verifyuser(usr_email, usr_pword))
            {
                text = "valid";
            }
            else
            {
                text = "invalid";
            }
            
        }
        private void verifyuser_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            loader.Visibility = Visibility.Hidden;
            email.IsEnabled = false;
            pword.IsEnabled = false;
            login_btn.IsEnabled = false;
            switch(text){
                case "valid":
                    MessageBox.Show(String.Format("User type: {0}\nMax size: {1}",Properties.Settings.Default.usertype,Properties.Settings.Default.maxsize));
                    MainWindow win2 = new MainWindow();
                    win2.Show();
                    this.Close();
                break;

                case "invalid":
                    prompt.Visibility = Visibility.Visible;
                    prompt.Content = "Email or password is invalid";                   
                break;
            }
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
                gologin();
        }

        private void email_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter){
                gologin();
            }
        }

        private void pword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter){
                gologin();
            }
        }

        private void gologin()
        {
            prompt.Visibility = Visibility.Hidden;
            usr_email = email.Text;
            usr_pword = pword.Password;
            email.IsEnabled = false;
            pword.IsEnabled = false;
            login_btn.IsEnabled = false;

            if (usr_email.Equals("") || usr_pword.Equals(""))
            {
                prompt.Visibility = Visibility.Visible;
                prompt.Content = "Email-address and password are required";
            }
            else
            {
                loader.Visibility = Visibility.Visible;
                verifyuser.RunWorkerAsync();
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            //change button background property
        }
    }
}
