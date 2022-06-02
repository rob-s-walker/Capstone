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

namespace DynastyScansDownloader
{
    public class JDynastyScans : Downloader
    {
        public override string Stringvalue { get => "DYNASTY-SCANS"; set { } }

        public override List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a)
        {
            List<string> ulist = new List<string>();
            web.Navigate().GoToUrl(chapter.ChapterURL);
            WaitForElement(web, By.ClassName("pages-list"));
            List<IWebElement> plink = web.FindElementByClassName("pages-list").FindElements(By.TagName("a")).Where(x => x.GetAttribute("class").Contains("page")).ToList();
            for(int i = 0; i < plink.Count; i++)
            {
                double percent = ((double)i / plink.Count);
                a.Invoke(percent, "");
                web.Navigate().GoToUrl(plink[i].GetAttribute("href"));
                WaitForElement(web, By.Id("download_page"));
                string url = web.FindElementById("download_page").GetAttribute("href");
                ulist.Add(url);
            }
            return ulist;
        }

        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            List<Chapter> clist = new List<Chapter>();
            WaitForElement(web, By.ClassName("chapter-list"));
            List<IWebElement> links = web.FindElementByClassName("chapter-list").FindElements(By.TagName("a")).Where(x => x.GetAttribute("href").Contains("/chapters/")).ToList();
            string mname = web.FindElementByClassName("tag-title").FindElement(By.TagName("b")).Text;
            foreach(IWebElement link in links)
            {
                string uri = link.GetAttribute("href");
                string ch = GetChapterNumber(link.Text);
                Chapter c = new Chapter(ch, uri);
                c.MName = mname;
                c.Source = Stringvalue;
                clist.Add(c);
            }
            return new Manga(mname, clist);
        }
    }
}
