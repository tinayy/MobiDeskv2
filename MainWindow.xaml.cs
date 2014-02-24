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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Net.NetworkInformation;





namespace Mobideskv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
            
            settings.DoWork += settings_DoWork;
            settings.RunWorkerCompleted += settings_RunWorkerCompleted;
            settings.WorkerSupportsCancellation = true;

            update.DoWork += update_DoWork;
            update.RunWorkerCompleted += update_RunWorkerCompleted;
            update.ProgressChanged += update_ProgessChanged;
            update.WorkerReportsProgress = true;
            update.WorkerSupportsCancellation = true;

            queue.DoWork += update_DoWork;
            queue.RunWorkerCompleted += update_RunWorkerCompleted;
            queue.WorkerReportsProgress = true;
            queue.WorkerSupportsCancellation = true;


            watcher.IncludeSubdirectories = true;
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Created);
            watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            //watcher.EnableRaisingEvents = true;
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
           //NotifyIcon.Icon = new System.Drawing.Icon("imgs/mobidesk_icon.ico");
            var path = "../../imgs/mobidesk_icon.ico";
            NotifyIcon.BalloonTipText = "HAHAHA";
            NotifyIcon.Icon = new System.Drawing.Icon(path);
            NotifyIcon.Click += new EventHandler(NotifyIcon_Click);
            
            
            

            _rcs = new bind();
            this.DataContext = _rcs;
            //checkConn.startConnWatch();
            
            
        }
        private bind _rcs;
        private FileSystemWatcher watcher = new FileSystemWatcher();
        private BackgroundWorker settings = new BackgroundWorker();
        private BackgroundWorker queue = new BackgroundWorker();
        private BackgroundWorker update = new BackgroundWorker();
        private System.Windows.Forms.FolderBrowserDialog browserdialog = new System.Windows.Forms.FolderBrowserDialog();
        private messageBox msgbox = new messageBox();
        private System.Windows.Forms.NotifyIcon NotifyIcon;
      
        private initset ini = new initset();
        private userdevice dvc = new userdevice();
        private objects obj = new objects();
        private sync sync = new sync();

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("Watcher Path: " + Properties.Settings.Default.directorypath.Replace("\\", "\\\\"));
            checkConn.StartCheck();
            
            var desktopworkingarea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopworkingarea.Right - this.Width;
            this.Top = desktopworkingarea.Bottom - this.Height;
            var settings = Properties.Settings.Default;

            if(settings.directorypath=="" || settings.computername==""){
                directorypath.Text = dvc.getdefaultloc();
                compname.Text = dvc.getcompname();
                pane.SelectedItem = settings_pane;
                
            }
            else {               
                directorypath.Text = Properties.Settings.Default.directorypath;
                compname.Text = Properties.Settings.Default.computername;
                pane.SelectedItem = overview_pane;
                MessageBox.Show(Properties.Settings.Default.totalb+" bytes");
                MessageBox.Show(Properties.Settings.Default.total + " "+Properties.Settings.Default.fsizeunit);
                update.RunWorkerAsync("0");
                watcher.Path = Properties.Settings.Default.directorypath.Replace("\\", "\\\\");
                
                //pane.SelectedItem = settings_pane;
            }
           
            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(5000);
            
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if(this.WindowState== WindowState.Minimized){
                this.WindowState = WindowState.Normal;
            }
        }

        private void hide_Click(object sender, RoutedEventArgs e)
        {
           this.WindowState = WindowState.Minimized;
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            pane.SelectedItem = overview_pane;
        }
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            pane.SelectedItem = settings_pane;
        }
        private void account_Click(object sender, RoutedEventArgs e)
        {
            pane.SelectedItem = account_pane;
        }
        private void help_Click(object sender, RoutedEventArgs e)
        {
            pane.SelectedItem = settings_pane;
        }

        private void unlink_Click(object sender, RoutedEventArgs e)
        {
            ini.settings("clearSet","");
            dvc.unlinkDevice();
            new Login().Show();
            this.Close();
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void menu_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
           
           browserdialog.Description = "Mobidesk Location";
           System.Windows.Forms.DialogResult dialog = browserdialog.ShowDialog();
           
           if (dialog == System.Windows.Forms.DialogResult.OK)
           {
               directorypath.Text = browserdialog.SelectedPath;
           }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            directorypath.IsEnabled = false;
            compname.IsEnabled = false;
            Save.IsEnabled = false;
            loader.Visibility = Visibility.Visible;
            
            //if selected path is different from saved path prompt user files will be moved
            bind();
            settings.RunWorkerAsync();          
        }
        
        private void settings_DoWork(object sender, DoWorkEventArgs e)
        {
           
            if(ini.settings("createSet","")){
                userLocalFiles create = new userLocalFiles();
                create.createlocalFolder();
            }

            
            
        }

        private void settings_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            directorypath.IsEnabled = true;
            compname.IsEnabled = true;
            Save.IsEnabled = true;
            loader.Visibility = Visibility.Collapsed;
            prompt.Visibility = Visibility.Visible;
            watcher.Path = Properties.Settings.Default.directorypath.Replace("\\", "\\\\");
            Thread.Sleep(2000);
            pane.SelectedItem = overview_pane;
            update.RunWorkerAsync("0");
        }

        private void update_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            if(bwAsync.CancellationPending){
                Thread.Sleep(1200);
                e.Cancel = true;
                return;
            }

            try
            {
                String op = (String)e.Argument;
                _rcs.status = "Updating Files";
                sync.update(op);
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
           
        }

        private void update_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            _rcs.status = "Uploading";
            status.Content = e.UserState;
            Console.WriteLine(e.UserState);
        }

        private void update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled){
                _rcs.status = "Paused Syncing";
            }

            updateCompleted();
           
        }
        
        private void updateCompleted()
        {
            _rcs.status = "Sync Completed";
            MessageBox.Show("Done");
            watcher.EnableRaisingEvents = true;
            //updatePanelVisible(0);
            sync.date();
            
            if(!monitorChanges.isServerMonitoringEnabled){
               // Console.WriteLine("Server monitoring is not enabled. Enable timer");
                monitorChanges.start_srv();
            }
            else
            {
                Console.WriteLine("Server monitoring is currently enabled");
            }

            if (!monitorChanges.isLocalMonitoringEnabled)
            {
                Console.WriteLine("Local monitoring is not enabled. Enable timer");
                monitorChanges.start_loc();
            }
            else
            {
                Console.WriteLine("Local monitoring is currently enabled");
            }
          
        }

        private void bind()
        {
            if(directorypath.Text == "" || compname.Text==""){

            }
            else {
                String dir = directorypath.Text;
                if(dir.EndsWith("\\Mobidesk")){
                    Properties.Settings.Default.directorypath = directorypath.Text;
                }
                else
                {
                    Properties.Settings.Default.directorypath = directorypath.Text + "\\Mobidesk";
                }
               
                Properties.Settings.Default.computername = compname.Text;
                Properties.Settings.Default.Save();
            }
            
        }


        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            objects.queue.Enqueue("chg?" + e.FullPath + "?" + "");
            Console.WriteLine("Changes in directory " + e.FullPath);
            //updatePanelVisible(1);
            
        }

        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            objects.queue.Enqueue("ctd?"+ e.FullPath + "?" + "");
            Console.WriteLine("ADDED TO QUEUE: ctd|" + e.FullPath + "|" + "");

            //updatePanelVisible(1);
        }

        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            objects.queue.Enqueue("dlt?" + e.FullPath + "?" + "");
            Console.WriteLine("ADDED TO QUEUE: dlt|" + e.FullPath + "|" + "");

            //updatePanelVisible(1);
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            objects.queue.Enqueue("rnm?" + e.OldFullPath + "?" + e.FullPath);
            Console.WriteLine("ADDED TO QUEUE:  rnm|" + e.OldFullPath + "|" + e.FullPath);

           // updatePanelVisible(1);
        }

        public  void updatePanelVisible(int inv)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                try
                {
                    if(inv==1){
                        updatePanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        updatePanel.Visibility = Visibility.Collapsed;
                    }
                   
                }
                catch
                {
                   updatePanel.Visibility = Visibility.Collapsed;
                }
            }));
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            if(objects.queueFrmServer.Count>0){
                objects.processQueue("stl");
            }
            updatePanelVisible(0);
            //update.RunWorkerAsync("3");
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pane.SelectedItem = overview_pane;
        }

        private void enablePause()
        {
            //stat_action.Content = "Pause ||";
            //stat_action.Visibility = Visibility.Visible;
        }

        private void stat_action_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(this.Content.Equals("Pause ||")){
                if(update.IsBusy){
                update.CancelAsync();
                }
            }

        }

        private void status_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
                MessageBox.Show("HEHE");
//stat_action.Content = "Pause ||";
              //  stat_action.Visibility = Visibility.Visible;
            
        }
        
    }
}
