using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Jumper.models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Management;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

using System.IO;
using AutomatedTester.BrowserMob;
using System.Diagnostics;

namespace Jumper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Manga CurrentManga;
        PluginLoader Download;
        Downloader CurrentDownloader;
        ChromeDriver web;
        //Server server;
        //Client client;
        string path;
        bool subfolder;
        models.Archiver.Type archtype;
        Manga SelectedManga;

        public MainWindow()
        {
            InitializeComponent();
            Download = new PluginLoader();
             
            Closing += Closer;
            System.Windows.Application.Current.Exit += Closer;
            if (String.IsNullOrEmpty(Settings.Default.Path))
            {
                Settings.Default.Path = System.IO.Path.GetFullPath(".\\tmp\\");
            }
            path = Settings.Default.Path;
            subfolder = Settings.Default.SubFolder;
            archtype = (models.Archiver.Type)Settings.Default.ArchiveType;
            Settings.Default.Save();
            DownloadButton.IsEnabled = false;
            AddButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
            RemoveAllButton.IsEnabled = false;
            Progressbar.Value = 0;
            Progressbar.Visibility = Visibility.Hidden;
            TotalPercent.Content = "";
            MangaName.Content = "";
            if (SelectedManga == null)
            {
                SelectedManga = new Manga("", new List<Chapter>());
               
                
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentManga != null)
            {

                AddChapters a = new AddChapters(CurrentManga);
                a.ShowDialog();
                SelectedManga.Name = CurrentManga.Name;
                    for(int i = a.EndIndx; i <= a.StartIndx; i++)
                    {
                        SelectedManga.Chapters.Add(CurrentManga.Chapters[i]);
                    }
                    if(a.Excludes.Count > 0)
                    {
                        foreach(int i in a.Excludes)
                        {
                            SelectedManga.Chapters.Remove(CurrentManga.Chapters[i]);
                        }
                    }
                QueueGrid.DataContext = SelectedManga.Chapters;
                QueueGrid.Columns[0].Header = "Manga";
                QueueGrid.Columns[1].Header = "Chapters";
                if(QueueGrid.Columns.Count >= 3)
                {
                    QueueGrid.Columns.RemoveAt(2);
                }
                RemoveButton.IsEnabled = true;
                RemoveAllButton.IsEnabled = true;
                DownloadButton.IsEnabled = true;

            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedManga.Chapters != null)
            {
                List<Chapter> x = new List<Chapter>();
                foreach(Chapter c in QueueGrid.SelectedItems)
                {
                    x.Add(c);
                }
                foreach (Chapter c in x)
                {
                    SelectedManga.Chapters.Remove(c);
                }
            }
            if(SelectedManga.Chapters.Count == 0)
            {
                RemoveButton.IsEnabled = false;
                RemoveAllButton.IsEnabled = false;
            }
        }

        private void RemoveAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedManga.Chapters.Clear();
            if (SelectedManga.Chapters.Count == 0)
            {
                RemoveButton.IsEnabled = false;
                RemoveAllButton.IsEnabled = false;
            }
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsDialog o = new OptionsDialog();
            o.ShowDialog();
            path = Settings.Default.Path;
            subfolder = Settings.Default.SubFolder;
            archtype = (models.Archiver.Type)Settings.Default.ArchiveType;
            Settings.Default.Save();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Thread download = new Thread(DownloadThread);
            download.Start();
            DownloadButton.IsEnabled = false;
            AddButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
            RemoveAllButton.IsEnabled = false;
            GetManga.IsEnabled = false;
            OptionsButton.IsEnabled = false;
        }
        private void DownloadThread()
        {
            if (SelectedManga.Chapters != null)
            {
                try
                {
                    Dispatcher.Invoke(new Action(() => { Progressbar.Visibility = Visibility.Visible; Loading.Visibility = Visibility.Visible; }));
                    GetImagesThread();
                    Dispatcher.Invoke(new Action(() => { Progressbar.Visibility = Visibility.Visible; }));
                    foreach (var l in SelectedManga.Chapters.GroupBy(x => x.Source))
                    {
                        List<Chapter> cl = l.ToList();
                        CurrentDownloader = Download.plugins.Where(x => x.Stringvalue == cl[0].Source).First();
                        CurrentDownloader.DownloadAllImages(cl, path, Settings.Default.SubFolder, new Action<double, string>((i, s) =>
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                Progressbar.Value = i;
                                TotalPercent.Content = s;
                            }), DispatcherPriority.Background);
                        }), archtype);
                    }
                }

                catch (Exception e)
                {
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        MakeMessage(e.Message);
                    }
                    else
                    {
                        throw e;
                    }
                    
                }
                finally
                {
                    try {
                        web.Quit();
                        web.Dispose();

                        Dispatcher.Invoke(new Action(() => {
                            DownloadButton.IsEnabled = false;
                            AddButton.IsEnabled = false;
                            RemoveButton.IsEnabled = false;
                            RemoveAllButton.IsEnabled = false;
                            GetManga.IsEnabled = true;
                            OptionsButton.IsEnabled = true;
                            Loading.Visibility = Visibility.Hidden;
                            SelectedManga.Chapters.Clear();
                            SelectedManga.Name = "";
                            CurrentManga = null;
                            Progressbar.Value = 0;
                            Progressbar.Visibility = Visibility.Hidden;
                            TotalPercent.Content = "";
                            MangaName.Content = "";


                        }), DispatcherPriority.Background);

                    } catch (Exception) { };
                }

                
                
            }
        }

        private void GetManga_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(AddressBar.Text) && !string.IsNullOrWhiteSpace(AddressBar.Text))
            {
                Thread gm = new Thread(GetMangaThread);
                gm.Start();
            }
           
        }

        private void GetMangaThread()
        {
            web = ChromeDriverInit();
            try
            {
                string s = "";
                Dispatcher.Invoke(new Action(() => { s = AddressBar.Text; Loading.Visibility = Visibility.Visible; GetManga.IsEnabled = false; }), DispatcherPriority.Background);
                web = ChromeDriverInit();
                Uri url = new Uri(s);
                if (SiteAvailable(url))
                {
                    web.Navigate().GoToUrl(url);

                    string dname = url.Host.Split('.')[url.Host.Split('.').Length - 2].ToUpper();
                    if (Download.plugins.Where(x => x.Stringvalue == dname).Count() >= 1)
                    {
                        CurrentDownloader = Download.plugins.Where(x => x.Stringvalue == dname).First();
                        CurrentManga = CurrentDownloader.GetManga(url, web);
                        Dispatcher.Invoke(new Action(() => { MangaName.Content = CurrentManga.Name; }), DispatcherPriority.Background);
                    }

                }

            }
            catch (Exception e)
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    MakeMessage(e.Message);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {

                    web.Quit();
                    web.Dispose();
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        DownloadButton.IsEnabled = true;
                        AddButton.IsEnabled = true;
                        RemoveButton.IsEnabled = false;
                        RemoveAllButton.IsEnabled = false;
                        GetManga.IsEnabled = true;
                        Progressbar.Visibility = Visibility.Hidden;
                        Loading.Visibility = Visibility.Hidden;
                        TotalPercent.Content = "";
                    }), DispatcherPriority.Background);
                }
                catch (Exception) { }
            }
            
        }
        public ChromeDriver ChromeDriverInit(bool proxy = false)
        {
            
            ChromeOptions co = new ChromeOptions();
            co.AddArgument("headless");
            co.AddArgument("incognito");
            co.AddArguments("no-sandbox");
            if (!proxy)
            { 
            //    server = new Server(System.IO.Path.GetFullPath(@".\plugins\browsermob-proxy-2.1.4\bin\browsermob-proxy.bat"));
            //    server.Start();

            //    client = server.CreateProxy();
            //    co.AddArgument("--proxy-server="+ client.SeleniumProxy);
                
            }
            //co.AddArgument("enable-extensions");
            //co.AddUserProfilePreference("download.default_directory", System.IO.Path.GetFullPath(@".\tmp\"));
            //co.AddUserProfilePreference("extensions.commands.windows:Ctrl+Q.command_name", "_execute_browser_action");
            //co.AddUserProfilePreference("extensions.commands.windows:Ctrl+Q.extension", "fdpohaocaechififmbbbbbknoalclacl");
            //co.AddUserProfilePreference("extensions.commands.windows:Ctrl+Q.global", "false");
            
            co.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            ChromeDriverService c = ChromeDriverService.CreateDefaultService("C:\\Users\\Banan\\workspace\\Capstone\\Jumper\\Jumper\\bin\\Debug\\plugins\\chromedriver", "chromedriver.exe");
            c.HideCommandPromptWindow = true;
           
            
            ChromeDriver web = new ChromeDriver(c, co);

            return web;
        }

                    
        
        public void GetImagesThread()
        {
            web = ChromeDriverInit();
            try
            {
                int i = 0;
                lock (SelectedManga)
                {
                    Dispatcher.Invoke(new Action(() => { Progressbar.Visibility = Visibility.Visible; }));
                    
                    List<Chapter> clist = SelectedManga.Chapters.Reverse().ToList();
                    foreach (Chapter c in clist)
                    {
                        
                        CurrentDownloader = Download.plugins.Where(x => x.Stringvalue == c.Source).First();
                        double pv = ((double)i / SelectedManga.Chapters.Count) * 100;
                        double px = (1d / SelectedManga.Chapters.Count) * 100;
                        
                        List<string> imgurl = CurrentDownloader.GetImageURLs(c, web, new Action<double,string>((p,n) => {
                            Dispatcher.Invoke(new Action(() => {
                                double v = (p * px) + pv;
                                Progressbar.Value = v;
                                TotalPercent.Content = "Scraping Images for Chapter: "+i +" of " + SelectedManga.Chapters.Count;
                            }), DispatcherPriority.Background);
                        }));
                        if(imgurl == null)
                        {
                            SelectedManga.Chapters.Remove(c);
                            i++;
                            continue;
                        }
                        else
                        {
                            foreach(string s in imgurl)
                            {
                                SelectedManga.Chapters.Where(x => x.Name == c.Name && x.MName == c.MName).First().ImageLocations.Add(s);
                            }
                            if(i%50 == 0 && i != 0)
                            {
                                if (web != null)
                                {

                                    web.Quit();
                                    web.Dispose();
                                    Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

                                    foreach (var chromeDriverProcess in chromeDriverProcesses)
                                    {
                                        chromeDriverProcess.Kill();
                                    }
                                    web = ChromeDriverInit();
                                }
                            }
                        
                            i++;
                        }
                        
                    }
                }
            }
            finally
            {
                Dispatcher.Invoke(new Action(() => {
                    Progressbar.Value = 0;
                    TotalPercent.Content = "";
                    Progressbar.Visibility = Visibility.Hidden;
                }), DispatcherPriority.Background);
                web.Dispose();
            }

        }

        public void Closer(object sender, CancelEventArgs e)
        {
            MangaName.Content = "Closing....";
            //Clean up so it doesn't leave chrome or chromedriver open when exiting
            //An amalgamation of answers from https://stackoverflow.com/questions/21320837/release-selenium-chromedriver-exe-from-memory
            if (web != null)
            {
                
                web.Quit();
                web.Dispose();
                Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    foreach (var child in GetChildProcesses(chromeDriverProcess))
                    {
                        child.Kill();
                    }
                    chromeDriverProcess.Kill();
                }
               

                

            }
            //if(client != null && server != null)
            //{
            //    client.Close();
            //    server.Stop();
            //}
            
            
        }
        public void Closer(object sender, ExitEventArgs e)
        {
            MangaName.Content = "Closing....";
            if (web != null)
            {
                //Clean up so it doesn't leave chrome or chromedriver open when exiting
                //An amalgamation of answers from https://stackoverflow.com/questions/21320837/release-selenium-chromedriver-exe-from-memory
                
                web.Quit();
                web.Dispose();
                Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    foreach(var child in GetChildProcesses(chromeDriverProcess))
                    {
                        child.Kill();
                    }
                    chromeDriverProcess.Kill();
                }
                
               

               
            }

            //if (client != null && server != null)
            //{
            //    client.Close();
            //    server.Stop();
            //}
        }
        public static IEnumerable<Process> GetChildProcesses(Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
        private bool SiteAvailable(Uri url)
        {
            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.AllowAutoRedirect = false;
            HttpWebResponse res;
            int sc;
            string sd;

            try
            {
                res = (HttpWebResponse)req.GetResponse();
                sc = (int)res.StatusCode;
                sd = res.StatusDescription;
                //Debug Purposes Only Messagebox for 3xx status codes
                //MangaName.Content = sc + ": " + sd;
                return sc < 400 || sc == 503;
            }
            catch(WebException w)
            {
                res = (HttpWebResponse)w.Response;
                sc = (int)res.StatusCode;
                sd = res.StatusDescription;
                //Debug Purposes Only Messagebox for 3xx status codes
                //MangaName.Content = sc + ": " + sd;
                return sc < 400 || sc == 503;
            }
            catch(NullReferenceException n)
            {
                res = null;
                MakeMessage("You are probably not connected to the internet. \nPlease Double Check Your Connection");
                return false;
            }

        }

        private void MakeMessage(string s)
        {
            System.Windows.MessageBox.Show(s);
        }

        
    }
}
