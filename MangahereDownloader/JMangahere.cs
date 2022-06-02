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
namespace MangahereDownloader
{
    public class JMangahere : Downloader
    {
        public override string Stringvalue { get => "MANGAHERE"; set { } }

        public override List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a)
        {
            List<string> ulist = new List<string>();
            web.Navigate().GoToUrl(chapter.ChapterURL);
            WaitForElement(web, By.ClassName("wid60"));
            SelectElement s = new SelectElement(web.FindElementByClassName("wid60"));
            int count = s.Options.Count;
            for (int i = 1; i < count; i++)
            {
                double percent = ((double)i / count);
                a.Invoke(percent, "");
                WaitForElement(web, By.ClassName("wid60"));
                s = new SelectElement(web.FindElementByClassName("wid60"));
                s.SelectByText(i.ToString());
                WaitForElement(web, By.ClassName("wid60"));
                s = new SelectElement(web.FindElementByClassName("wid60"));
                string link = web.FindElementById("image").GetAttribute("src");
                while (ulist.Contains(link)) { };
                ulist.Add(link);
            }
            return ulist;
        }

        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            List<Chapter> clist = new List<Chapter>();
            WaitForElement(web, By.ClassName("detail_list"));
            List<IWebElement> links = web.FindElementByClassName("detail_list").FindElements(By.TagName("a")).ToList();
            string[] nx = web.Title.Split(new string[] { " Manga -" },StringSplitOptions.None);
            string name = nx[0];

            for (int i = 0; i < links.Count; i++)
            {
                string url = links[i].GetAttribute("href");
                string lt = links[i].Text.Replace(name, "");

                string n = GetChapterNumber(lt);
                Chapter c = new Chapter(n, url);
                c.MName = name;
                c.Source = Stringvalue;
                if (clist.Where(x => x.Name == n).Count() == 0)
                {
                    clist.Add(c);
                }

            }

            return new Manga(name, clist);
        }
    }
}
