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

namespace MangadexDownloader
{
    public class JMangadex : Downloader
    {
        public override string Stringvalue { get => "MANGADEX"; set { } }

        public override List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a)
        {
            List<string> ulist = new List<string>();
            web.Navigate().GoToUrl(chapter.ChapterURL);
            WaitForElement(web, By.Name("jump-page"));
            int pages = new SelectElement(web.FindElementByName("jump-page")).Options.Count;
            while(pages < 1)
            {
                pages = new SelectElement(web.FindElementByName("jump-page")).Options.Count;
            }
            for (int i = 1; i < pages+1; i++)
            {
                double percent = ((double)i / pages);
                a.Invoke(percent, "");
                string purl = chapter.ChapterURL + "/" + i;
                web.Navigate().GoToUrl(purl);
                WaitForElement(web, By.CssSelector("img[data-page='" + i + "']"));
                IWebElement q = web.FindElementByCssSelector("img[data-page='" + i + "']");
                while (q == null)
                {
                    q = web.FindElementByCssSelector("img[data-page='" + i + "']");
                }
                string iurl = q.GetAttribute("src");
                ulist.Add(iurl);
            }
            return ulist;
        }

        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            List<Chapter> clist = new List<Chapter>();
            WaitForElement(web, By.CssSelector("div[data-lang = '1'][data-chapter = '1'] a.text-truncate"));

            List<IWebElement> links = web.FindElementsByCssSelector("div[data-lang='1']").ToList();
            string mname = web.FindElementByClassName("card-header").Text;
            foreach(IWebElement div in links)
            {
                WaitForElement(web, By.ClassName("text-truncate"));
                IWebElement a = div.FindElement(By.ClassName("text-truncate"));
                string ch = GetChapterNumber(a.Text);
                string link = "https://mangadex.org/chapter/" + div.GetAttribute("data-id");
                Chapter c = new Chapter(ch, link);
                c.MName = mname;
                c.Source = Stringvalue;
                clist.Add(c);
            }
            return new Manga(mname, clist);

        }
    }
}
