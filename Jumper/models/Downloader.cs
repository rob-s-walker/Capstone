using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using AutomatedTester.BrowserMob;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Jumper.models
{
    public abstract class Downloader
    {   
        public string DriverPath;
        public abstract string Stringvalue { get; set; }
        public List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Client c) { return null; }
        public abstract List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a);
        public void DownloadAllImages(List<Chapter> Clist, string patha , bool subfolder, Action<double, string> a, Jumper.models.Archiver.Type Atype = Archiver.Type.ZIP)
        {
            for (int x = 0; x < Clist.Count; x++)
            {
                string s = Clist[x].Name;
                string f = "{0:D" + getNumLZero(Parser(s)) + "}";
                string dirname = string.Format(f, (int)Parser(s));
                if(s.Split('.').Count() > 1)
                {
                    dirname = string.Format(f, (int)Parser(s)) + '.' + s.Split('.')[1];
                }
                string path = patha;
                if (subfolder)
                {
                    path = patha + @"\" + Clist[x].MName.Replace(':', '-');
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                
                Directory.CreateDirectory(path + @"\" + dirname);
                double ppv = (1d / Clist.Count) * 100;
                double p = ((double)(x) / Clist.Count) * 100;
                string o = "Downloading Chapter..."+(x) + " of " + Clist.Count;
                a.Invoke(p, o);

                for (int y = 0; y < Clist[x].ImageLocations.Count; y++)
                {
                    double pc = Clist[x].ImageLocations.Count;
                    double ppp = ((y / pc)*ppv)+p;
                    a.Invoke(ppp, o);
                    string format = "{0:D" + getNumLZero(Clist[x].ImageLocations.Count) + "}";
                    string fname = String.Format(format, y);
                    string ext = Path.GetExtension(Path.GetFileName(Clist[x].ImageLocations[y]));
                    ext = ext.Split('?')[0];
                    if (string.IsNullOrEmpty(ext))
                    {
                        ext = ".png";
                    }
                    GetImageFromURL(Clist[x].ImageLocations[y]).Save(path + @"\" + dirname + @"\" + fname + ext);
                }
                if(Directory.GetFiles(path + @"\" + dirname).Length == 0)
                {
                    Directory.Delete(path + @"\" + dirname);
                    return;
                }
                
                Archiver.ArchiveFolder(path + @"\" + dirname, Atype);

            }
        }
        private double Parser(string s)
        {
            double d = 0;
            int i = 0;
            if(double.TryParse(s,out d))
            {
                d = double.Parse(s);
            }else if(int.TryParse(s,out i))
            {
                d = (double)int.Parse(s);
            }
            else
            {
                throw new InvalidOperationException("String not parsable");
            }

            return d;
        }
            
            
        public abstract Manga GetManga(Uri manga, ChromeDriver web) ;

        
        private bool WaitBool(IWebDriver web, By by)
        {
            return web.FindElements(by).Count > 0;
        }
        public void WaitForElement(IWebDriver web, By by, int waittime = 90)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(web, TimeSpan.FromSeconds(waittime));
                wait.Until(new Func<IWebDriver, bool>((IWebDriver) => WaitBool(web, by)));
            }
            catch (WebDriverException)
            {

            }
            
        }

       
        private void WaitForFile(string path)
        {
            while(!File.Exists(path)){

            }
        }

       protected bool ElementExists(ChromeDriver web, By by)
        {
            try
            {
               IWebElement e = web.FindElement(by);
                return e != null;
            }
            catch (Exception)
            {
                return false;
            }
        }



        protected string GetChapterNumber(string linktext)
        {
            string s;
            string lt = linktext;
            if(lt.ToUpper() == "ONESHOT")
            {
                return 1d.ToString();
            }
            if (lt.Contains("Ch."))
            {
                lt = lt.Replace("Ch.", "");
            }
            if (lt.Contains("ch."))
            {
                lt = lt.Replace("ch.", "");
            }
            if (lt.Contains("Vol."))
            {
                lt = lt.Replace("Vol.", "");
            }
            if (lt.Contains("vol."))
            {
                lt = lt.Replace("vol.", "");
            }
            if (lt.Contains("..."))
            {
                lt = lt.Replace("...", "");
            }
            if (lt.Contains("V"))
            {
                lt = lt.Replace("V", "");
            }
            List<string> sx = Regex.Split(lt, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList();

            if (sx.Count > 1)
            {
                List<double> ld = new List<double>();
                foreach (string x in sx)
                {
                    ld.Add(double.Parse(x));
                }
                if (ld.Contains(0d) && ld.Max() == 1)
                {
                    return 0d.ToString();
                }
                return ld.Max().ToString();
            }

            s = sx[0];

            return s;
        }
        public Image GetImageFromURL(string url)
        {
            WebClient w = new WebClient();
            byte[] bytes;
            if (url.StartsWith("data:image/"))
            {
                bytes = GetImageFromB64Canvas(url);
            }
            else
            {
                bytes = w.DownloadData(url);
            }
            MemoryStream m = new MemoryStream(bytes);
            Image img = Image.FromStream(m);
            w.Dispose();
            return img;

        }
        protected int getNumLZero(double n)
        {
            int lz = 0;
            while (Math.Pow(10, lz) < n)
            {
                lz++;
            }
            return lz;
        }

        private byte[] GetImageFromB64Canvas(string url)
        {
            string u = url.Split(',')[1];
            byte[] b = Convert.FromBase64String(u);
            return b;
        }

    }

    public class DownloaderFactory
    {
        private PluginLoader Plugins = new PluginLoader();
        public Downloader GetDownloader(string d)
        {
            Downloader c = Plugins.plugins.Where(x => x.Stringvalue == d).First();
            return c;
        }
    }

    public class PluginLoader
    {
        //Mostly from https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62
        //May be further changed time permitting.
        //StopGap
        public ICollection<Downloader> plugins;
        public ICollection<Type> pluginTypes;

        public PluginLoader()
        {
            pluginTypes = new List<Type>();
            plugins = new List<Downloader>();
            LoadPlugins();
           
        }
        private void LoadPlugins()
        {
            string[] dllFileNames = null;
            if (Directory.Exists("./plugins"))
            {
                dllFileNames = Directory.GetFiles("./plugins", "*.dll");
                
                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                
                foreach (string dllFile in dllFileNames)
                {
                    if (dllFile.Contains("7z"))
                    {
                        continue;
                    }
                    else
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                        Assembly assembly = Assembly.Load(an);
                        assemblies.Add(assembly);
                    }
                    
                }
                Type pluginType = typeof(Downloader);
                
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.BaseType.FullName == (pluginType.FullName))
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }
                
                foreach (Type type in pluginTypes)
                {
                    Downloader plugin = (Downloader)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }
            }
            else
            {
                Directory.CreateDirectory("./plugins");
            }
        }
           
    }
}
