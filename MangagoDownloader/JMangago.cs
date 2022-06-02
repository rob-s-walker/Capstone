using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Jumper;
using Jumper.models;
using System.Threading;
using System.Drawing;
using System.IO;
using OpenQA.Selenium.Interactions;
using AutomatedTester.BrowserMob;
using AutomatedTester.BrowserMob.HAR;

namespace MangagoDownloader
{
    public class JMangago : Jumper.models.Downloader
    {
          private List<string> pageimage = new List<string>();
        public override string Stringvalue { get { return "MANGAGO"; } set { } }

        public override List<string> GetImageURLs(Chapter ch, ChromeDriver web, Client c)
        {
            c.NewHar("page");
            Uri chapter = (ch.ChapterURL);
            List<string> imgurl = new List<string>();
            web.Url = chapter.OriginalString;
            web.Navigate();
            List<string> pages = new List<string>();
            WaitForElement(web, By.Id("dropdown-menu-page"));
            pages = web.FindElementById("dropdown-menu-page").FindElements(By.TagName("a")).Select(x => x.GetAttribute("href")).ToList();
            for(int i = 0; i < pages.Count; i++)
            {
                string page = pages[i];
                web.Url = page;
                string cname = "page"+(i+1);
               
                try
                {
                    WaitForElement(web, By.TagName("img"), 1);
                    List<IWebElement> imgs = web.FindElementsByTagName("img").ToList();
                    List<IWebElement> byc = imgs.Where(x => x.GetAttribute("id") == cname).ToList();
                    string img = byc.
                    First().GetAttribute("src");
                    imgurl.Add(img);
                }
                catch (Exception)
                {
                    fiximage f = new fiximage();
                    Actions a = new Actions(web).MoveToElement(web.FindElementByTagName("canvas")).SendKeys(OpenQA.Selenium.Keys.Control + "q");
                    
                    
                    a.Build().Perform();

                    WaitForElement(web, By.Id("btn-download"));

                    string u = web.FindElementById("btn-download").GetAttribute("href");

                    Image ix = f.GetImageFromURL(u);
                    ix.Save(@".\tmp\imgtmp\" + ch.Name + "\\" + cname + ".jpeg");

                    imgurl.Add(@".\tmp\imgtmp\" + ch.Name + "\\" + cname + ".jpeg");
                }
            }
            return imgurl;

        }
        private string GetImgURL(List<Entry> entries, List<string> imgurls)
        {
            string s = "";

            Entry e = entries.Where(x => CorrectURL(x.Request.Url, imgurls)).First();
            s = e.Request.Url;
            return s;
        }

        private bool CorrectURL(string url, List<string> imgurls)
        {
            Uri u = new Uri(url);

            if (u.Host.Split('.')[u.Host.Split('.').Length-2] == "mangapicgallery")
            { 
                if(Path.GetExtension(url) != ".gif")
                {
                    return !imgurls.Contains(url);
                }
                
            }
            return false;

        }

       

        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            
            Manga m;
            List<Chapter> chapters = new List<Chapter>();
                       
            web.Url = manga.OriginalString;
            WaitForElement(web, By.Id("chapter_table"));
            web.FindElementById("showallpanel").FindElement(By.TagName("a")).Click();
            

            if(web.FindElementById("chapter_table").FindElements(By.TagName("a")).Count == 0)
            {
                throw new Exception("Website Unreachable");
            }
            List<IWebElement> links = web.FindElementById("chapter_table").FindElements(By.TagName("a")).ToList();

            foreach (IWebElement link in links)
            {
                string text = link.Text;
                string url = link.GetAttribute("href");
                Chapter c = new Chapter(text,url);
                chapters.Add(c);
            }
            string manganame = web.Title.Replace(" manga - Mangago","");
            m = new Manga(manganame, chapters);

            return m;
        }

        public override void DownloadAllImages(List<Chapter> Clist,Action<double,string> a)
        {
            int i = 1;
            foreach (Chapter c in Clist)
            {
                double ix = ((double)i / Clist.Count);
                a.Invoke(ix, i + " of " + Clist.Count);
                DownloadImageFromChapter(c);
                Archiver.ArchiveFolder(@".\tmp\"+c.Name,Archiver.Type.ZIP);
                i++;
            }
            a.Invoke(0, "Download Complete");
        }

       private void DownloadImageFromChapter(Chapter c)
        {
            fiximage fi = new fiximage();
            Directory.CreateDirectory(@".\tmp\" + c.Name);
            for(int i =0; i < c.ImageLocations.Count;i++)
            {
                Uri u = c.ImageLocations[i];
                Image img;
                if (u.AbsolutePath.Contains("cspiclink"))
                {
                    byte[] b = fi.DescrambleImage(u.ToString());
                
                    MemoryStream ms = new MemoryStream(b);
                    img = Image.FromStream(ms);
                    int pagenum = c.ImageLocations.IndexOf(u) + 1;
                    int pagemax = c.ImageLocations.Count;
                    string format = "{0:D" + getNumLZero(pagemax) + "}";
                    string fname = String.Format(format, pagenum);
                    img.Save(@".\tmp\" + c.Name + "\\" + fname + ".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }else if (u.IsFile)
                {
                    img = fi.GetImageFromURL(u.ToString());
                    int pagenum = c.ImageLocations.IndexOf(u) + 1;
                    int pagemax = c.ImageLocations.Count;
                    string format = "{0:D" + getNumLZero(pagemax) + "}";
                    string fname = String.Format(format, pagenum);
                    img.Save(@".\tmp\" + c.Name + "\\" + fname + ".jpeg");
                    File.Delete(u.ToString());
                }
                else
                {
                    img = fi.GetImageFromURL(u.ToString());
                    int pagenum = c.ImageLocations.IndexOf(u) + 1;
                    int pagemax = c.ImageLocations.Count;
                    string format = "{0:D" + getNumLZero(pagemax) + "}";
                    string fname = String.Format(format, pagenum);
                    img.Save(@".\tmp\" + c.Name + "\\" + fname + ".jpeg");
                }
                if (File.Exists(@".\tmp\tmpscrambled.jpeg"))
                {
                    File.Delete(@".\tmp\tmpscrambled.jpeg");
                }
            }
        }
        

        int getNumLZero(int n)
        {
            int lz = 0;
            while (Math.Pow(10, lz) < n)
            {
                lz++;
            }
            return lz;
        }
    }
}
