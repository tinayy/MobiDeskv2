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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mobideskv2
{
    /// <summary>
    /// Interaction logic for NotificationTray.xaml
    /// </summary>
    public partial class NotificationTray : Window
    {
        public NotificationTray()
        {
            InitializeComponent();

            var desktopworkingarea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopworkingarea.Right - this.Width;
            this.Top = desktopworkingarea.Bottom - this.Height;
            var settings = Properties.Settings.Default;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeOutStoryboard") as Storyboard;
            Storyboard.SetTarget(sb, this);
            sb.Begin();
        }
        
        public void msg(String msg)
        {
            message.Text = msg;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeOutStoryboard") as Storyboard;
            Storyboard.SetTarget(sb, this);
            sb.Begin();
        }
    }
}
