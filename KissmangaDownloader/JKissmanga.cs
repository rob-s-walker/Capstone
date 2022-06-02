using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumper.models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net;
using System.IO;
using System.Drawing.Imaging;

namespace KissmangaDownloader
{
    public class JKissmanga : Downloader
    {
        
        public override string Stringvalue { get => "KISSMANGA"; set { } }

        public override List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a)
        {
            List<string> ulist = new List<string>();
            web.Navigate().GoToUrl(chapter.ChapterURL);
            WaitForElement(web, By.Id("selectReadType"));
            SelectElement s = new SelectElement(web.FindElementById("selectReadType"));
            s.SelectByValue("1");
            
            List<IWebElement> imgs = web.FindElementById("divImage").FindElements(By.TagName("img")).ToList();
            while(imgs.Count < 2)
            {
                imgs = web.FindElementById("divImage").FindElements(By.TagName("img")).ToList();
                if (web.FindElementById("divImage").FindElements(By.TagName("p")).Count == 1) {
                    return null;
                }
            }
            for(int i = 0; i < imgs.Count; i++)
            {
                double percent = ((double)i / imgs.Count);
                a.Invoke(percent, "");
                Uri u = new Uri(imgs[i].GetAttribute("src"));
                if(u.Host.Split('.')[u.Host.Split('.').Length-2] == "blogspot")
                {
                    ulist.Add(imgs[i].GetAttribute("src"));
                }
                
            }

            return ulist;
        }


        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            List<Chapter> clist = new List<Chapter>();
            WaitForElement(web, By.ClassName("listing"),10);
            List<IWebElement> links = web.FindElementByClassName("listing").FindElements(By.TagName("a")).ToList();
            string name = web.FindElementByClassName("bigChar").Text;

            for(int i = 0; i < links.Count; i++)
            {
                string url = links[i].GetAttribute("href");
                string lt = links[i].Text.Replace(name, "");
                lt = lt.Replace(" online", "");
                
                string n = GetChapterNumber(lt);
                Chapter c = new Chapter(n, url);
                c.MName = name;
                c.Source = Stringvalue;
                if(clist.Where(x=> x.Name == n).Count() == 0)
                {
                    clist.Add(c);
                }
                
            }

            return new Manga(name, clist);
        }

        
    }
}
