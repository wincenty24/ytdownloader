using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using VideoLibrary;
namespace ytdownloaderbywincenty
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public bool Downloadingpermission = true;
        public bool progressbar_bool = false;
        public bool progresbar_worker = false;
        public bool Convert_downloaded_video_to_mp3_bool;
        public bool Delete_the_video_path_after_downloading_bool;
        public string path_download_name="";
        public string URL_string = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Download_button_Click(object sender, RoutedEventArgs e)
        {
            Convert_downloaded_video_to_mp3_Checkbox.IsEnabled = false;
            Delete_the_video_path_after_downloading_checkbox.IsEnabled = false;
            Choose_path_Button.IsEnabled = false;
            if (Downloadingpermission == true)
            {
                Download_button.IsEnabled = false;
                Downloadingpermission = false;
                
            }
            if (Directory.Exists(path_download_name))
            {
                
                URL_string = Url_textbox.Text.ToString();
                URL_string = Regex.Replace(URL_string, @"\s", "");

                Thread t = new Thread(new ThreadStart(ThreadProc));
                t.Start();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    download_progress_bar.Visibility = Visibility.Visible;
                }));
                BackgroundWorker worker = worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerAsync();
                progressbar_bool = true;
                

            }
            else
            {
                System.Windows.MessageBox.Show("This path does not exist! ");
            }
            
        }

        private void ThreadProc()
        {
            //   string x = Url_textbox.Text.ToString();
            try
            {
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(URL_string);
                try
                {
                    what_label.Dispatcher.Invoke(delegate { what_label.Content = "Downloading"; });
                    File.WriteAllBytes(path_download_name + @"\" + video.FullName, video.GetBytes());
                }
                catch
                {
                    System.Windows.MessageBox.Show("Wystąpił problem z zapisem pliku, sprawdź czy ustawiony przez Ciebie floder istnieje!");
                }             
                if (Convert_downloaded_video_to_mp3_bool == true)
                {
                    string music_namee = video.FullName.Replace("- YouTube", "").Replace(".mp4","");
                    var convert = new NReco.VideoConverter.FFMpegConverter();
                    try
                    {                  
                        what_label.Dispatcher.Invoke(delegate { what_label.Content = "Converting to mp3"; });
                        convert.ConvertMedia(path_download_name + @"\" + video.FullName, path_download_name + @"\" + music_namee + ".mp3", "mp3");
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Convert error");
                        //  MessageBoxResult result = MessageBox.Show("Wystąpił problem z konwersją pliku mp.4 na mp.3, spokojnie plik video istnieje w folderze!", "Alert", MessageBoxButton.OKCancel);
                        //switch (result)
                        //{
                        //  case MessageBoxResult.OK:

                        //    break;
                        //case MessageBoxResult.Cancel:

                        //  break;

                        //}

                    }

                }
                if (Delete_the_video_path_after_downloading_bool == true)
                {
                    try
                    {


                        if (File.Exists(System.IO.Path.Combine(path_download_name, video.FullName)))
                        {
                            what_label.Dispatcher.Invoke(delegate { what_label.Content = "Deleting"; });
                            File.Delete(System.IO.Path.Combine(path_download_name, video.FullName));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.ToString());
                    }
                }


            }
            catch
            {
                /*
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
  DispatcherPriority.Background,
  new Action(() => this.Download_button.IsEnabled = true));
                Downloadingpermission = true;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    download_progress_bar.Visibility = Visibility.Hidden;
                }));
                worker.CancelAsync();
                */
                System.Windows.MessageBox.Show("Download error");
            }
            finally
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    download_progress_bar.Visibility = Visibility.Hidden;
                }));
                progressbar_bool = false;
                what_label.Dispatcher.Invoke(delegate { what_label.Content = "Done"; });
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Download_button.IsEnabled = true));
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Convert_downloaded_video_to_mp3_Checkbox.IsEnabled = true));
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Delete_the_video_path_after_downloading_checkbox.IsEnabled = true));
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Choose_path_Button.IsEnabled = true));
                
            }
            /*
            if (Delete_the_video_path_after_downloading_checkbox.IsChecked==true)
            {
                
                try
                {
                    if (File.Exists(Path.Combine(rootFolder, authorsFile)))
                    {
                    }

                }
                catch (IOException ioExp)
                {
                    System.Windows.MessageBox.Show(ioExp.ToString());
                }
                
            }
            */
        }

        private void Choose_path_Button_Click(object sender, RoutedEventArgs e)
        {

            
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            DialogResult result = folderDlg.ShowDialog();
            if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Windows.MessageBox.Show(folderDlg.SelectedPath);
                Name_path_download_label.Content = folderDlg.SelectedPath;
                path_download_name= folderDlg.SelectedPath;
                // textBox1.Text = folderDlg.SelectedPath;
                //Environment.SpecialFolder root = folderDlg.RootFolder;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Delete_the_video_path_after_downloading_bool= Properties.Settings.Default.save_Delete_the_video_path_after_downloading_checkbox_bool;
            Convert_downloaded_video_to_mp3_bool = Properties.Settings.Default.save_Convert_downloaded_video_to_mp3_Checkbox_bool;
            path_download_name = Properties.Settings.Default.save_path_download_string.ToString();
            Name_path_download_label.Content = path_download_name;
            Convert_downloaded_video_to_mp3_Checkbox.IsChecked = Properties.Settings.Default.save_Convert_downloaded_video_to_mp3_Checkbox_bool;
            Delete_the_video_path_after_downloading_checkbox.IsChecked = Properties.Settings.Default.save_Delete_the_video_path_after_downloading_checkbox_bool;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bool b1,b2,b3 = false;

            if (Properties.Settings.Default.save_path_download_string.ToString() != path_download_name)
            {
                Properties.Settings.Default.save_path_download_string = path_download_name.ToString();
                b1 = true;
            }
            else
            {
                b1 = false;
            }
            if (Properties.Settings.Default.save_Convert_downloaded_video_to_mp3_Checkbox_bool != (bool)Convert_downloaded_video_to_mp3_Checkbox.IsChecked)
            {
                Properties.Settings.Default.save_Convert_downloaded_video_to_mp3_Checkbox_bool = (bool)Convert_downloaded_video_to_mp3_Checkbox.IsChecked;
                b2 = true;
            }
            else
            {
                b2 = false;
            }
            if (Properties.Settings.Default.save_Delete_the_video_path_after_downloading_checkbox_bool != (bool)Delete_the_video_path_after_downloading_checkbox.IsChecked)
            {
                Properties.Settings.Default.save_Delete_the_video_path_after_downloading_checkbox_bool = (bool)Delete_the_video_path_after_downloading_checkbox.IsChecked;
                b3 = true;
            }
            else
            {
                b3 = false;
            }
           if(b1==true || b2==true || b3 == true)
            {
                Properties.Settings.Default.Save();
            }
                

            }

        private void Delete_the_video_path_after_downloading_checkbox_Click(object sender, RoutedEventArgs e)
        {
            Delete_the_video_path_after_downloading_bool = (bool)Delete_the_video_path_after_downloading_checkbox.IsChecked;
            if (Convert_downloaded_video_to_mp3_Checkbox.IsChecked == false)
            {
                Delete_the_video_path_after_downloading_checkbox.IsChecked = false;
            }
        }

        private void Convert_downloaded_video_to_mp3_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            Convert_downloaded_video_to_mp3_bool = (bool)Convert_downloaded_video_to_mp3_Checkbox.IsChecked;
            if (Convert_downloaded_video_to_mp3_Checkbox.IsChecked == false)
            {
                Delete_the_video_path_after_downloading_checkbox.IsChecked = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            
           
        }
       private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (progressbar_bool)
            {

                for (int i = 0; i < 100; i++)
                {

                    (sender as BackgroundWorker).ReportProgress(i);
                    if (i < 25)
                    {

                        Thread.Sleep(20);
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
                if (progressbar_bool == false)
                {
                    break;
                }
            }
           
            
        }
        private void delate_file()
        {

        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            download_progress_bar.Value = e.ProgressPercentage;
        }
    }
}
